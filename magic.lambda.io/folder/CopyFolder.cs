/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Threading.Tasks;
using magic.node;
using magic.node.contracts;
using magic.lambda.io.helpers;
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
            Utilities.CopyMoveHelper(signaler, _rootResolver, input, _folderService, true);
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            await Utilities.CopyMoveHelperAsync(signaler, _rootResolver, input, _folderService, true);
        }
    }
}
