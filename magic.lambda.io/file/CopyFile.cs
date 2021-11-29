/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io.file
{
    /// <summary>
    /// [io.file.copy] slot for moving a file on your server.
    /// </summary>
    [Slot(Name = "io.file.copy")]
    public class CopyFile : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;
        readonly IFileService _service;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="service">Underlaying file service implementation.</param>
        public CopyFile(IRootResolver rootResolver, IFileService service)
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
            SanityCheckArguments(input);
            signaler.Signal("eval", input);
            var paths = GetPaths(input);
            _service.Copy(
                paths.SourcePath,
                paths.DestinationPath);
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            SanityCheckArguments(input);
            await signaler.SignalAsync("eval", input);
            var paths = GetPaths(input);
            await _service.CopyAsync(
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
            var sourcePath = PathResolver
                .CombinePaths(
                    _rootResolver.RootFolder,
                    input.GetEx<string>());

            var destinationPath = PathResolver
                .CombinePaths(
                    _rootResolver.RootFolder,
                    input.Children.First().GetEx<string>());

            // Defaulting filename to the filename of the source file, unless another filename is explicitly given.
            if (destinationPath.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                destinationPath += Path.GetFileName(sourcePath);

            // Sanity checking arguments.
            if (sourcePath == destinationPath)
                throw new HyperlambdaException("You cannot copy a file using the same source and destination path");

            // For simplicity, we're deleting any existing files with the path of the destination file.
            if (_service.Exists(destinationPath))
                _service.Delete(destinationPath);
            return (sourcePath, destinationPath);
        }

        #endregion
    }
}
