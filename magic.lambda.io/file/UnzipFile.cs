/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;
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
            var zipFile = _rootResolver.AbsolutePath(input.GetEx<string>());

            var folderNode = input.Children
                .FirstOrDefault(x => x.Name == "folder")?
                .GetEx<string>();

            var targetFolder = folderNode == null ?
                Path.GetDirectoryName(zipFile) :
                _rootResolver.AbsolutePath(folderNode);

            var fastZip = new FastZip();
            fastZip.ExtractZip(zipFile, targetFolder, null);
        }
    }
}
