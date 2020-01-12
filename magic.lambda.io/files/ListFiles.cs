/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io.files
{
    /// <summary>
    /// [io.files.list] slot for listing files on server.
    /// </summary>
    [Slot(Name = "io.files.list")]
    public class ListFiles : ISlot
    {
        readonly IRootResolver _rootResolver;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        public ListFiles(IRootResolver rootResolver)
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
            var displayHiddenFiles = input.Children.FirstOrDefault(x => x.Name == "display-hidden")?.GetEx<bool>() ?? false;
            var root = PathResolver.Normalize(_rootResolver.RootFolder);
            var folder = input.GetEx<string>();
            input.Clear();
            var files = Directory.GetFiles(PathResolver.CombinePaths(_rootResolver.RootFolder, folder)).ToList();
            files.Sort();
            foreach (var idx in files)
            {
                // Making sure we don't show hidden operating system files by default.
                if (displayHiddenFiles || !Path.GetFileName(idx).StartsWith(".", StringComparison.InvariantCulture))
                    input.Add(new Node("", idx.Substring(root.Length)));
            }
        }
    }
}
