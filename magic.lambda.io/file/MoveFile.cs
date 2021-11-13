/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.IO;
using System.Threading.Tasks;
using magic.node;
using magic.signals.contracts;
using magic.lambda.io.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io.file
{
    /// <summary>
    /// [io.file.move] slot for moving a file on your server.
    /// </summary>
    [Slot(Name = "io.file.move")]
    public class MoveFile : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;
        readonly IFileService _service;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="service">Underlaying file service implementation.</param>
        public MoveFile(IRootResolver rootResolver, IFileService service)
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
            Helpers.Execute(signaler, _rootResolver, input, "io.file.move", Move);
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            // Invoking helper method containing commonalities.
            await Helpers.ExecuteAsync(signaler, _rootResolver, input, "io.file.move", Move);
        }

        #region [ -- Private helper methods -- ]

        /*
         * Commonalities between async and sync version to keep code DRY.
         */
        void Move(string source, string destination)
        {
            /*
             * Defaulting destination filename to be the same as source filename,
             * unless a different filename is explicitly given.
             *
             * This allows us to do things such as Move("/foo1/bar.txt", "/foo2/") making sure
             * the filename is kept as is, but the file is moved to a different folder.
             */
            if (destination.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                destination += Path.GetFileName(source);

            // Sanity checking arguments.
            if (source == destination)
                throw new ArgumentException("You cannot move a file using the same source and destination path");

            /*
             * For simplicity, we're deleting any existing files
             * with the path of the destination file.
             */
            if (_service.Exists(destination))
                _service.Delete(destination);

            _service.Move(source, destination);
        }

        #endregion
    }
}
