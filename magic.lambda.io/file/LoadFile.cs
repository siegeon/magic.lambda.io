﻿/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Threading.Tasks;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.io.file
{
    /// <summary>
    /// [io.file.load] slot for loading a file on your server.
    /// </summary>
    [Slot(Name = "io.file.load")]
    [Slot(Name = "io.file.load.binary")]
    public class LoadFile : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;
        readonly IFileService _service;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="service">Underlaying file service implementation.</param>
        public LoadFile(IRootResolver rootResolver, IFileService service)
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
            if (input.Name == "io.file.load.binary")
                input.Value = _service.LoadBinary(_rootResolver.AbsolutePath(input.GetEx<string>()));
            else
                input.Value = _service.LoadBinary(_rootResolver.AbsolutePath(input.GetEx<string>()));
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            if (input.Name == "io.file.load.binary")
                input.Value = await _service.LoadBinaryAsync(_rootResolver.AbsolutePath(input.GetEx<string>()));
            else
                input.Value = await _service.LoadBinaryAsync(_rootResolver.AbsolutePath(input.GetEx<string>()));
        }
    }
}
