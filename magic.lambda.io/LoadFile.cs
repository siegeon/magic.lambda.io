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
    [Slot(Name = "load-file")]
    public class LoadFile : ISlot
    {
        public void Signal(ISignaler signaler, Node input)
        {
            var filename = RootResolver.Root + input.GetEx<string>();
            input.Value = File.ReadAllText(filename, Encoding.UTF8);
        }
    }
}
