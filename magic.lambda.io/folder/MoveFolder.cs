/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Threading.Tasks;
using magic.node;
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
            // Invoking helper method containing commonalities.
            Helpers.Move(signaler, _rootResolver, input, "io.folder.move", Move);
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            // Invoking helper method containing commonalities.
            await Helpers.MoveAsync(signaler, _rootResolver, input, "io.folder.move", Move);
        }

        #region [ -- Private helper methods -- ]

        /*
         * Commonalities between async and sync version to keep code DRY.
         */
        void Move(string source, string destination)
        {
            // Sanity checking arguments.
            if (source == destination)
                throw new ArgumentException("You cannot move a folder using the same source and destination path");

            /*
             * Verifying folder doesn't exist from before.
             *
             * Notice, contrary to the move file version, we cannot delete any
             * existing folders here, since it might include deleting a lot of
             * files unintentionally.
             */
            if (_service.Exists(destination))
                throw new ArgumentException("Cannot move folder, destination folder already exists");

            _service.Move(source, destination);
        }

        #endregion
    }
}
