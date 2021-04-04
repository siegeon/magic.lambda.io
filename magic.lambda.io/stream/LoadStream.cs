/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.IO;
using System.Threading.Tasks;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.contracts;

namespace magic.lambda.io.stream
{
    /// <summary>
    /// [io.stream.load] slot for loading a stream raw as byte[] content.
    /// </summary>
    [Slot(Name = "io.stream.load")]
    public class LoadStream : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        public LoadStream(IRootResolver rootResolver)
        {
            _rootResolver = rootResolver;
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var stream = input.GetEx<Stream>();
            var memory = new MemoryStream();
            stream.CopyTo(memory);
            input.Value = memory.ToArray();
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            var stream = input.GetEx<Stream>();
            var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            input.Value = memory.ToArray();
        }
    }
}
