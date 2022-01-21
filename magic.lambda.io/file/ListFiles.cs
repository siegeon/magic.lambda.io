/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.io.file
{
    /// <summary>
    /// [io.file.list] slot for listing files on server.
    /// </summary>
    [Slot(Name = "io.file.list")]
    [Slot(Name = "io.file.list-recursively")]
    public class ListFiles : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;
        readonly IFileService _service;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="service">Underlaying file service implementation.</param>
        public ListFiles(IRootResolver rootResolver, IFileService service)
        {
            _rootResolver = rootResolver;
            _service = service;
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            // Retrieving arguments to slot.
            var args = GetArgs(input);

            // Retrieving files.
            var files = input.Name == "io.file.list" ?
                _service.ListFiles(_rootResolver.AbsolutePath(args.Folder)) : 
                _service.ListFilesRecursively(_rootResolver.AbsolutePath(args.Folder));

            // Returning files as lambda to caller.
            foreach (var idx in files)
            {
                // Adds currently iterated file to result.
                AddFile(input, idx, args.ShowHidden);
            }
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            // Retrieving arguments to slot.
            var args = GetArgs(input);

            // Retrieving files.
            var files = input.Name == "io.file.list" ?
                await _service.ListFilesAsync(_rootResolver.AbsolutePath(args.Folder)) : 
                await _service.ListFilesRecursivelyAsync(_rootResolver.AbsolutePath(args.Folder));

            // Returning files as lambda to caller.
            foreach (var idx in files)
            {
                // Adds currently iterated file to result.
                AddFile(input, idx, args.ShowHidden);
            }
        }

        #region [ -- Private helper methods -- ]

        /*
         * Returns arguments to slot invocation.
         */
        (string Folder, bool ShowHidden) GetArgs(Node input)
        {
            // Checking if we should display hidden files (files starting with ".").
            var displayHiddenFiles = input.Children
                .FirstOrDefault(x => x.Name == "display-hidden")?
                .GetEx<bool>() ?? false;

            // Figuring out folder to list files from within.
            var folder = input.GetEx<string>();

            // House cleaning
            input.Clear();
            input.Value = null;

            // Returning results to caller.
            return (folder, displayHiddenFiles);
        }

        /*
         * Adds one file to result.
         */
        void AddFile(Node result, string filename, bool showHidden)
        {
            // Making sure we don't show hidden operating system files by default.
            if (showHidden ||
                !Path.GetFileName(filename).StartsWith(".", StringComparison.InvariantCulture))
                result.Add(new Node("", _rootResolver.RelativePath(filename)));
        }

        #endregion
    }
}
