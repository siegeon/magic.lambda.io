/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io.stream
{
    /// <summary>
    /// [io.stream.save-file] slot for saving a stream on your server
    /// to the specified filename.
    /// </summary>
    [Slot(Name = "io.stream.save-file")]
    public class SaveFileStream : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;
        readonly IStreamService _service;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="service">Service implementation.</param>
        public SaveFileStream(IRootResolver rootResolver, IStreamService service)
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
            var args = GetArguments(signaler, input);
            if (args.Overwrite && _service.Exists(args.Destination))
                _service.Delete(args.Destination);
            _service.SaveFile(args.Stream, args.Destination);
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            var args = GetArguments(signaler, input);
            if (args.Overwrite && _service.Exists(args.Destination))
                _service.Delete(args.Destination);
            await _service.SaveFileAsync(args.Stream, args.Destination);
        }

        #region [ -- Private helper methods -- ]

        (string Destination, Stream Stream, bool Overwrite) GetArguments(ISignaler signaler, Node input)
        {
            // Checking if caller wants to overwrite existing file.
            var overwrite = true;
            var overwriteNode = input.Children.FirstOrDefault(x => x.Name == "overwrite");
            if (overwriteNode != null)
            {
                overwrite = overwriteNode.GetEx<bool>();
                overwriteNode.UnTie();
            }

            // Making sure we evaluate any children, to make sure any signals wanting to retrieve our source is evaluated.
            signaler.Signal("eval", input);

            // Figuring out where to save file.
            var destination = _rootResolver.AbsolutePath(input.GetEx<string>());

            // Retrieving stream
            var stream = input.Children.First().GetEx<Stream>();

            // Returning results to caller.
            return (destination, stream, overwrite);
        }

        #endregion
    }
}
