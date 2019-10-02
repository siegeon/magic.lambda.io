/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.IO;
using System.Text;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io
{
    /// <summary>
    /// [io.file.load] slot for loading a file on your server.
    /// </summary>
    [Slot(Name = "io.file.load")]
    public class LoadFile : ISlot
    {
        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var filename = RootResolver.Root + input.GetEx<string>();
            input.Value = File.ReadAllText(filename, Encoding.UTF8);
        }
    }
}
