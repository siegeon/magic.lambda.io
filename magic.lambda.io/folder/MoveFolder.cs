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

namespace magic.lambda.io.folder
{
    /// <summary>
    /// [io.folder.move] slot for moving a folder on your server.
    /// </summary>
    [Slot(Name = "io.folder.move")]
    public class MoveFolder : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;
        readonly IFolderService _service;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="service">Underlaying file service implementation.</param>
        public MoveFolder(IRootResolver rootResolver, IFolderService service)
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
            SanityCheckInvocation(input);
            signaler.Signal("eval", input);
            MoveImplementation(input);
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            SanityCheckInvocation(input);
            await signaler.SignalAsync("eval", input);
            MoveImplementation(input);
        }

        #region [ -- Private helper methods -- ]

        void SanityCheckInvocation(Node input)
        {
            if (!input.Children.Any())
                throw new ArgumentException("No destination provided to [io.folder.move]");
        }

        void MoveImplementation(Node input)
        {
            // Retrieving source path.
            string sourcePath = PathResolver.CombinePaths(
                _rootResolver.RootFolder,
                input.GetEx<string>());

            // Retrieving destination path.
            var destinationPath = PathResolver
                .CombinePaths(
                    _rootResolver.RootFolder,
                    input.Children.First().GetEx<string>());

            // Sanity checking arguments.
            if (sourcePath == destinationPath)
                throw new ArgumentException("You cannot move a file using the same source and destination path");

            // Verifying folder doesn't exist from before.
            if (_service.Exists(destinationPath))
                throw new ArgumentException("Cannot move folder, destination folder already exists");

            _service.Move(sourcePath, destinationPath);
        }

        #endregion
    }
}
