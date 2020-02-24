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

namespace magic.lambda.io.files
{
    /// <summary>
    /// [io.files.copy] slot for moving a file on your server.
    /// </summary>
    [Slot(Name = "io.files.copy")]
    [Slot(Name = "wait.io.files.copy")]
    public class CopyFile : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        public CopyFile(IRootResolver rootResolver)
        {
            _rootResolver = rootResolver ?? throw new ArgumentNullException(nameof(rootResolver));
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            // Sanity checking invocation.
            if (!input.Children.Any())
                throw new ArgumentNullException("No destination provided to [io.files.copy]");

            // Making sure we evaluate any children, to make sure any signals wanting to retrieve our source is evaluated.
            signaler.Signal("eval", input);
            string sourcePath = PathResolver.CombinePaths(_rootResolver.RootFolder, input.GetEx<string>());
            var destinationPath = PathResolver.CombinePaths(_rootResolver.RootFolder, input.Children.First().GetEx<string>());
            if (destinationPath.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                destinationPath += Path.GetFileName(sourcePath);

            // For simplicity, we're deleting any existing files with the path of the destination file.
            if (File.Exists(destinationPath))
                File.Delete(destinationPath);

            File.Copy(
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
            if (!input.Children.Any())
                throw new ArgumentException("No destination provided to [io.files.copy]");

            // Making sure we evaluate any children, to make sure any signals wanting to retrieve our source is evaluated.
            await signaler.SignalAsync("eval", input);
            string sourcePath = PathResolver.CombinePaths(_rootResolver.RootFolder, input.GetEx<string>());
            var destinationPath = PathResolver.CombinePaths(_rootResolver.RootFolder, input.Children.First().GetEx<string>());

            // For simplicity, we're deleting any existing files with the path of the destination file.
            if (File.Exists(destinationPath))
                File.Delete(destinationPath);

            using (Stream source = File.OpenRead(sourcePath))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
        }
    }
}
