/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io.stream
{
    /// <summary>
    /// [io.stream.open-file] slot for opening a file in read only mode
    /// and returning it as a stream to caller.
    /// </summary>
    [Slot(Name = "io.stream.open-file")]
    public class OpenFileStream : ISlot
    {
        readonly IRootResolver _rootResolver;
        readonly IStreamService _service;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="service">Service implementation.</param>
        public OpenFileStream(IRootResolver rootResolver, IStreamService service)
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
            // Figuring out filename.
            var filename = PathResolver.CombinePaths(
                _rootResolver.RootFolder,
                input.GetEx<string>());

            // Opening file and returning to caller.
            input.Value = _service.OpenFile(filename);
        }
    }
}
