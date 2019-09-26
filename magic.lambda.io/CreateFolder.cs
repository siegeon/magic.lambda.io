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
    [Slot(Name = "create-folder")]
    public class CreateFolder : ISlot
    {
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
