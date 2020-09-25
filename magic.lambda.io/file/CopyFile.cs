/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
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
            // Sanity checking invocation.
            SanityCheckArguments(input);

            // Making sure we evaluate any children, to make sure any signals wanting to retrieve our destination is evaluated.
            signaler.Signal("eval", input);

            // Finding absolute paths.
            string sourcePath = PathResolver
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
                throw new ArgumentException("You cannot copy a file using the same source and destination path");

            // For simplicity, we're deleting any existing files with the path of the destination file.
            if (_service.Exists(destinationPath))
                _service.Delete(destinationPath);

            _service.Copy(
                sourcePath,
                destinationPath);
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            // Sanity checking invocation.
            SanityCheckArguments(input);

            // Making sure we evaluate any children, to make sure any signals wanting to retrieve our destination is evaluated.
            await signaler.SignalAsync("eval", input);

            // Making sure we evaluate any children, to make sure any signals wanting to retrieve our source is evaluated.
            string sourcePath = PathResolver.CombinePaths(
                _rootResolver.RootFolder,
                input.GetEx<string>());

            var destinationPath = PathResolver.CombinePaths(
                _rootResolver.RootFolder,
                input.Children.First().GetEx<string>());

            // Defaulting filename to the filename of the source file, unless another filename is explicitly given.
            if (destinationPath.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                destinationPath += Path.GetFileName(sourcePath);

            // Sanity checking arguments.
            if (sourcePath == destinationPath)
                throw new ArgumentException("You cannot copy a file using the same source and destination path");

            // For simplicity, we're deleting any existing files with the path of the destination file.
            if (_service.Exists(destinationPath))
                _service.Delete(destinationPath);

            await _service.CopyAsync(sourcePath, destinationPath);
        }

        #region [ -- Private helper methods -- ]

        void SanityCheckArguments(Node input)
        {
            if (!input.Children.Any())
                throw new ArgumentException("No destination provided to [io.file.copy]");
        }

        #endregion
    }
}
