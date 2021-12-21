/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.IO;
using System.Linq;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.io.file
{
    /// <summary>
    /// [io.file.unzip] slot for unzipping a previously zipped file.
    /// </summary>
    [Slot(Name = "io.file.unzip")]
    public class UnzipFile : ISlot
    {
        readonly IRootResolver _rootResolver;
        readonly IFileService _fileService;
        readonly IFolderService _folderService;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="fileService">Needed to be able to write ZIP file's content.</param>
        /// <param name="folderService">Needed to be able to create folders in file system.</param>
        public UnzipFile(IRootResolver rootResolver, IFileService fileService, IFolderService folderService)
        {
            _rootResolver = rootResolver;
            _fileService = fileService;
            _folderService = folderService;
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            // Figuring out zip file's full path.
            var zipFilePath = input.GetEx<string>();

            // Figuring out destination folder caller wants to use, defaulting to folder of ZIP file if not specified.
            var destinationFolder = input.Children
                .FirstOrDefault(x => x.Name == "folder")?
                .GetEx<string>() ?? Path.GetDirectoryName(zipFilePath) + "/";

            if (!_folderService.Exists(_rootResolver.AbsolutePath(destinationFolder)))
                throw new HyperlambdaException($"Destination folder '{destinationFolder}' for [io.file.unzip] does not exist.");

            // Loading entire ZIP file into memory stream
            using (var memoryStream = new MemoryStream(_fileService.LoadBinary(_rootResolver.AbsolutePath(zipFilePath))))
            {
                // Creating a ZIP archive wrapping memory stream.
                using (var archive = new ZipArchive(memoryStream))
                {
                    // Looping through each entry in archive, ignoring garbage OS X "special files".
                    foreach (var idxEntry in archive.Entries.Where(x => x.Name != "__MACOSX" && x.Name != ".DS_Store"))
                    {
                        // Opening up currently iterated ZIP entry
                        using (var srcStream = idxEntry.Open())
                        {
                            // Copying currently iterated ZIP entry to memory stream for simplicity.
                            using (var memSrcStream = new MemoryStream())
                            {
                                srcStream.CopyTo(memSrcStream);
                                var idxContent = memSrcStream.ToArray();

                                // Saving currently iterated file.
                                SaveFile(destinationFolder, idxEntry.FullName.Replace("\\", "/"), idxContent);
                            }
                        }
                    }
                }
            }
        }

        #region [ -- Private helper methods -- ]

        /*
         * Saves a single file from ZIP file archive.
         */
        void SaveFile(string destinationFolder, string filename, byte[] content)
        {
            // Making sure we create currently iterated destination folder unless it already exists.
            var entities = filename.Split('/');
            var currentFolder = _rootResolver.AbsolutePath(destinationFolder);
            foreach (var idx in entities.Take(entities.Length - 1))
            {
                currentFolder += idx + "/";
                if (!_folderService.Exists(currentFolder))
                    _folderService.Create(currentFolder);
            }

            // Figuring out full filename of current entry and saving it.
            var fullFileName = currentFolder + entities.Last();
            _fileService.Save(fullFileName, content);
        }

        #endregion
    }
}
