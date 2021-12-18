/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io.file
{
    /// <summary>
    /// [io.file.list] slot for listing files on server.
    /// </summary>
    [Slot(Name = "io.file.list")]
    public class ListFiles : ISlot
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
            // Checking if we should display hidden files (files starting with ".").
            var displayHiddenFiles = input.Children
                .FirstOrDefault(x => x.Name == "display-hidden")?
                .GetEx<bool>() ?? false;

            var root = PathResolver.Normalize(_rootResolver.RootFolder);
            var folder = input.GetEx<string>();
            var files = _service
                .ListFiles(
                    PathResolver.CombinePaths(
                        _rootResolver.RootFolder,
                        folder))
                .ToList();

            // Sorting and returning files to caller as lambda children.
            files.Sort();
            input.Clear();
            foreach (var idx in files)
            {
                // Making sure we don't show hidden operating system files by default.
                if (displayHiddenFiles ||
                    !Path.GetFileName(idx).StartsWith(".", StringComparison.InvariantCulture))
                    input.Add(new Node("", idx.Substring(root.Length)));
            }
        }
    }
}
