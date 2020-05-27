/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.contracts;
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

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        public FolderExists(IRootResolver rootResolver)
        {
            _rootResolver = rootResolver ?? throw new ArgumentNullException(nameof(rootResolver));
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            input.Value = Directory.Exists(PathResolver.CombinePaths(_rootResolver.RootFolder, input.GetEx<string>()));
        }
    }
}
