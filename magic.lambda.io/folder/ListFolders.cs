/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.io.folder
{
    /// <summary>
    /// [io.folder.list] slot for listing folders on server.
    /// </summary>
    [Slot(Name = "io.folder.list")]
    public class ListFolders : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;
        readonly IFolderService _service;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="service">Underlaying file service implementation.</param>
        public ListFolders(IRootResolver rootResolver, IFolderService service)
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

            // Returning files as lambda to caller.
            foreach (var idx in _service.ListFolders(_rootResolver.AbsolutePath(args.Folder)))
            {
                // Adds currently iterated file to result.
                AddFolder(input, idx, args.ShowHidden);
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

            // Returning files as lambda to caller.
            foreach (var idx in await _service.ListFoldersAsync(_rootResolver.AbsolutePath(args.Folder)))
            {
                // Adds currently iterated file to result.
                AddFolder(input, idx, args.ShowHidden);
            }
        }

        #region [ -- Private helper methods -- ]

        /*
         * Returns arguments to slot invocation.
         */
        (string Folder, bool ShowHidden) GetArgs(Node input)
        {
            // Checking if we should display hidden files (files starting with ".").
            var displayHiddenFolders = input.Children
                .FirstOrDefault(x => x.Name == "display-hidden")?
                .GetEx<bool>() ?? false;

            // Figuring out folder to list files from within.
            var folder = input.GetEx<string>();

            // House cleaning
            input.Clear();
            input.Value = null;

            // Returning results to caller.
            return (folder, displayHiddenFolders);
        }

        /*
         * Adds one file to result.
         */
        void AddFolder(Node result, string folderName, bool showHidden)
        {
            // Making sure we don't show hidden operating system files by default.
            if (showHidden || !new DirectoryInfo(folderName).Name.StartsWith("."))
                result.Add(new Node("", _rootResolver.RelativePath(folderName)));
        }

        #endregion
    }
}
