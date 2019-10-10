/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
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
    /// [io.files.save] slot for saving a file on your server.
    /// </summary>
    [Slot(Name = "io.files.save")]
    [Slot(Name = "wait.io.files.save")]
    public class SaveFile : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        public SaveFile(IRootResolver rootResolver)
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
            // Making sure we evaluate any children, to make sure any signals wanting to retrieve our source is evaluated.
            signaler.Signal("eval", input);
            File.WriteAllText(PathResolver.CombinePaths(_rootResolver.RootFolder, input.GetEx<string>()), input.Children.First().GetEx<string>());
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            // Making sure we evaluate any children, to make sure any signals wanting to retrieve our source is evaluated.
            await signaler.SignalAsync("eval", input);
            using (var file = File.OpenWrite(PathResolver.CombinePaths(_rootResolver.RootFolder, input.GetEx<string>())))
            {
                using (var writer = new StreamWriter(file))
                {
                    await writer.WriteAsync(input.Children.First().GetEx<string>());
                }
            }
        }
    }
}
