/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.io.folder
{
    /// <summary>
    /// [io.folder.copy] slot for copying a folder on your server.
    /// </summary>
    [Slot(Name = "io.folder.copy")]
    public class CopyFolder : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;
        readonly IFolderService _folderService;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="folderService">Underlaying folder service implementation.</param>
        public CopyFolder(IRootResolver rootResolver, IFolderService folderService)
        {
            _rootResolver = rootResolver;
            _folderService = folderService;
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

            // For simplicity, we're deleting any existing folders with the path of the destination file.
            if (_folderService.Exists(paths.DestinationPath))
                _folderService.Delete(paths.DestinationPath);

            // Actual copy implementation.
            _folderService.Copy(
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

            // For simplicity, we're deleting any existing folders with the path of the destination file.
            if (await _folderService.ExistsAsync(paths.DestinationPath))
                await _folderService.DeleteAsync(paths.DestinationPath);

            // Actual copy implementation.
            await _folderService.CopyAsync(
                paths.SourcePath,
                paths.DestinationPath);
        }
    }
}
