/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io.file
{
    /// <summary>
    /// [io.file.unzip] slot for unzipping a previously zipped file.
    /// </summary>
    [Slot(Name = "io.file.unzip")]
    public class UnzipFile : ISlot
    {
        readonly IRootResolver _rootResolver;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        public UnzipFile(IRootResolver rootResolver)
        {
            _rootResolver = rootResolver;
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var zipFile = PathResolver.CombinePaths(
                _rootResolver.RootFolder,
                input.GetEx<string>());

            var targetFolder = input.Children
                .FirstOrDefault(x => x.Name == "folder")?
                .GetEx<string>() ??
                Path.GetDirectoryName(zipFile);

            var fastZip = new FastZip();
            fastZip.ExtractZip(zipFile, targetFolder, null);
        }
    }
}
