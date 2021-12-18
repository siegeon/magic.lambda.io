/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.Linq;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io.folder
{
    /// <summary>
    /// [io.folder.list] slot for listing folders on server.
    /// </summary>
    [Slot(Name = "io.folder.list")]
    public class ListFolders : ISlot
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
            // Checking if we should display hidden files (files starting with ".").
            var displayHiddenFolders = input.Children
                .FirstOrDefault(x => x.Name == "display-hidden")?
                .GetEx<bool>() ?? false;

            // Figuring out folder to list files from within.
            var folder = input.GetEx<string>();

            // House cleaning
            input.Clear();
            input.Value = null;

            // Returning files as lambda to caller.
            foreach (var idx in _service.ListFolders(_rootResolver.AbsolutePath(folder)))
            {
                // Making sure we don't show hidden operating system folders by default.
                if (displayHiddenFolders ||
                    !idx.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last().StartsWith(".", StringComparison.InvariantCulture))
                    input.Add(new Node("", _rootResolver.RelativePath(idx)));
            }
        }
    }
}
