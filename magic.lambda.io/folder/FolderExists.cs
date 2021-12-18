﻿/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io.folder
{
    /// <summary>
    /// [io.folder.exists] slot for figuring out if a folder exists from before or not.
    /// </summary>
    [Slot(Name = "io.folder.exists")]
    public class FolderExists : ISlot
    {
        readonly IRootResolver _rootResolver;
        readonly IFolderService _service;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="service">Underlaying file service implementation.</param>
        public FolderExists(IRootResolver rootResolver, IFolderService service)
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
            input.Value = _service.Exists(PathResolver.CombinePaths(_rootResolver.RootFolder, input.GetEx<string>()));
        }
    }
}
