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
            SanityCheckArguments(input);
            signaler.Signal("eval", input);
            var paths = GetPaths(input);
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
            SanityCheckArguments(input);
            await signaler.SignalAsync("eval", input);
            var paths = GetPaths(input);
            await _folderService.CopyAsync(
                paths.SourcePath,
                paths.DestinationPath);
        }

        #region [ -- Private helper methods -- ]

        /*
         * Sanity checks arguments provided.
         */
        static void SanityCheckArguments(Node input)
        {
            if (!input.Children.Any())
                throw new HyperlambdaException("No destination provided to [io.file.copy]");
        }

        /*
         * Retrieves source and destination path for operation.
         */
        (string SourcePath, string DestinationPath) GetPaths(Node input)
        {
            // Finding absolute paths.
            var sourcePath = _rootResolver.AbsolutePath(input.GetEx<string>());
            var destinationPath = _rootResolver.AbsolutePath(input.Children.First().GetEx<string>());

            // Defaulting filename to the filename of the source file, unless another filename is explicitly given.
            if (destinationPath.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                destinationPath += Path.GetFileName(sourcePath);

            // Sanity checking arguments.
            if (sourcePath == destinationPath)
                throw new HyperlambdaException("You cannot copy a file using the same source and destination path");

            // For simplicity, we're deleting any existing files with the path of the destination file.
            if (_folderService.Exists(destinationPath))
                _folderService.Delete(destinationPath);
            return (sourcePath, destinationPath);
        }

        #endregion
    }
}
