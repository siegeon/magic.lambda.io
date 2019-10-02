/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.IO;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io
{
    /// <summary>
    /// [io.folder.create] slot for creating a new folder on server.
    /// </summary>
    [Slot(Name = "io.folder.create")]
    public class CreateFolder : ISlot
    {
        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var path = RootResolver.Root + input.GetEx<string>();
            if (Directory.Exists(path))
                input.Value = false;

            Directory.CreateDirectory(path);
            input.Value = true;
        }
    }
}
