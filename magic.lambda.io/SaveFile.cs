/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.IO;
using System.Linq;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io
{
    /// <summary>
    /// [io.file.save] slot for saving a file on your server.
    /// </summary>
    [Slot(Name = "io.file.save")]
    public class SaveFile : ISlot
    {
        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            if (input.Children.Count() != 1)
                throw new ApplicationException("[save-file] requires exactly one child node");

            signaler.Signal("eval", input);

            var filename = RootResolver.Root + input.GetEx<string>();
            File.WriteAllText(filename, input.Children.First().GetEx<string>());
        }
    }
}
