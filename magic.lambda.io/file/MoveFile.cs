/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Threading.Tasks;
using magic.node;
using magic.node.contracts;
using magic.lambda.io.folder;
using magic.signals.contracts;

namespace magic.lambda.io.file
{
    /// <summary>
    /// [io.file.move] slot for moving a file on your server.
    /// </summary>
    [Slot(Name = "io.file.move")]
    public class MoveFile : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;
        readonly IFileService _fileService;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="fileService">Underlaying file service implementation.</param>
        public MoveFile(IRootResolver rootResolver, IFileService fileService)
        {
            _rootResolver = rootResolver;
            _fileService = fileService;
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            // Sanity checking arguments and evaluating them.
            Utilities.SanityCheckArguments(input);
            signaler.Signal("eval", input);

            // Retrieving source and destination path.
            var paths = Utilities.GetPaths(input, _rootResolver);

            // For simplicity, we're deleting any existing files with the path of the destination file.
            if (_fileService.Exists(paths.DestinationPath))
                _fileService.Delete(paths.DestinationPath);

            // Actual move implementation.
            _fileService.Move(
                paths.SourcePath,
                paths.DestinationPath);
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            // Sanity checking arguments and evaluating them.
            Utilities.SanityCheckArguments(input);
            await signaler.SignalAsync("eval", input);

            // Retrieving source and destination path.
            var paths = Utilities.GetPaths(input, _rootResolver);

            // For simplicity, we're deleting any existing files with the path of the destination file.
            if (await _fileService.ExistsAsync(paths.DestinationPath))
                await _fileService.DeleteAsync(paths.DestinationPath);

            // Actual move implementation.
            await _fileService.MoveAsync(
                paths.SourcePath,
                paths.DestinationPath);
        }
    }
}
