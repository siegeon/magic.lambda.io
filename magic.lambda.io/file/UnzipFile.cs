/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.IO;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Threading.Tasks;
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
    public class UnzipFile : ISlot, ISlotAsync
    {
        readonly IRootResolver _rootResolver;
        readonly IFolderService _folderService;
        readonly IStreamService _streamService;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        /// <param name="folderService">Needed to be able to create folders in file system.</param>
        /// <param name="streamService">Needed to be able to save unzipped files.</param>
        public UnzipFile(
            IRootResolver rootResolver,
            IFolderService folderService,
            IStreamService streamService)
        {
            _rootResolver = rootResolver;
            _folderService = folderService;
            _streamService = streamService;
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            // Retrieving arguments to invocation.
            var args = GetArgs(input);

            // Making sure destination folder exists.
            if (!_folderService.Exists(_rootResolver.AbsolutePath(args.DestinationFolder)))
                throw new HyperlambdaException($"Destination folder '{args.DestinationFolder}' for [io.file.unzip] does not exist.");

            // Invoking implementation method.
            Unzip(args.ZipFilePath, args.DestinationFolder);
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        /// <returns>Awaitabale task</returns>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            // Retrieving arguments to invocation.
            var args = GetArgs(input);

            // Making sure destination folder exists.
            if (!await _folderService.ExistsAsync(_rootResolver.AbsolutePath(args.DestinationFolder)))
                throw new HyperlambdaException($"Destination folder '{args.DestinationFolder}' for [io.file.unzip] does not exist.");

            // Invoking implementation method.
            await UnzipAsync(args.ZipFilePath, args.DestinationFolder);
        }

        #region [ -- Private helper methods -- ]

        /*
         * Helper method to unzip file.
         */
        void Unzip(string zipFilePath, string destinationFolder)
        {
            // Loading entire ZIP file into memory stream
            using (var sourceStream = _streamService.OpenFile(_rootResolver.AbsolutePath(zipFilePath)))
            {
                // Creating a ZIP archive wrapping memory stream.
                using (var archive = new ZipArchive(sourceStream))
                {
                    // Looping through each entry in archive, ignoring garbage OS X "special files".
                    foreach (var idxEntry in archive.Entries)
                    {
                        // Verifying this is a file.
                        if (idxEntry.FullName.Split('/').Last().IndexOf(".") != -1)
                        {
                            // This is a file, opening it up and saving it.
                            using (var srcStream = idxEntry.Open())
                            {
                                // Saving currently iterated file.
                                SaveFile(destinationFolder, idxEntry.FullName.Replace("\\", "/"), srcStream);
                            }
                        }
                    }
                }
            }
        }

        /*
         * Helper method to unzip file.
         */
        async Task UnzipAsync(string zipFilePath, string destinationFolder)
        {
            // Loading entire ZIP file into memory stream
            using (var sourceStream = await _streamService.OpenFileAsync(_rootResolver.AbsolutePath(zipFilePath)))
            {
                // Creating a ZIP archive wrapping memory stream.
                using (var archive = new ZipArchive(sourceStream))
                {
                    // Looping through each entry in archive, ignoring garbage OS X "special files".
                    foreach (var idxEntry in archive.Entries)
                    {
                        // Verifying this is a file.
                        if (idxEntry.FullName.Split('/').Last().IndexOf(".") != -1)
                        {
                            // This is a file, opening it up and saving it.
                            using (var srcStream = idxEntry.Open())
                            {
                                // Saving currently iterated file.
                                await SaveFileAsync(destinationFolder, idxEntry.FullName.Replace("\\", "/"), srcStream);
                            }
                        }
                    }
                }
            }
        }

        /*
         * Saves a single file from ZIP file archive.
         */
        void SaveFile(string destinationFolder, string filename, Stream contentStream)
        {
            // Making sure we create currently iterated destination folder unless it already exists.
            var entities = filename.Split('/');
            var currentFolder = new StringBuilder(_rootResolver.AbsolutePath(destinationFolder));
            foreach (var idx in entities.Take(entities.Length - 1))
            {
                if (idx == "__MACOSX" || idx == ".DS_Store")
                    return; // Ignoring garbage OS X files

                currentFolder.Append(idx).Append("/");
                if (!_folderService.Exists(currentFolder.ToString()))
                    _folderService.Create(currentFolder.ToString());
            }

            // Figuring out full filename of current entry and saving it.
            var fullFileName = currentFolder + entities.Last();
            _streamService.SaveFile(contentStream, fullFileName, true);
        }

        /*
         * Saves a single file from ZIP file archive.
         */
        async Task SaveFileAsync(string destinationFolder, string filename, Stream contentStream)
        {
            // Making sure we create currently iterated destination folder unless it already exists.
            var entities = filename.Split('/');
            var currentFolder = new StringBuilder(_rootResolver.AbsolutePath(destinationFolder));
            foreach (var idx in entities.Take(entities.Length - 1))
            {
                if (idx == "__MACOSX" || idx == ".DS_Store")
                    return; // Ignoring garbage OS X files

                currentFolder.Append(idx).Append("/");
                if (!await _folderService.ExistsAsync(currentFolder.ToString()))
                    await _folderService.CreateAsync(currentFolder.ToString());
            }

            // Figuring out full filename of current entry and saving it.
            var fullFileName = currentFolder + entities.Last();
            await _streamService.SaveFileAsync(contentStream, fullFileName, true);
        }

        /*
         * Helper method to retrieve arguments.
         */
        (string ZipFilePath, string DestinationFolder) GetArgs(Node input)
        {
            // Figuring out zip file's full path.
            var zipFilePath = input.GetEx<string>();

            // Figuring out destination folder caller wants to use, defaulting to folder of ZIP file if not specified.
            var destinationFolder = input.Children
                .FirstOrDefault(x => x.Name == "folder")?
                .GetEx<string>() ?? Path.GetDirectoryName(zipFilePath) + "/";

            // House cleaning.
            input.Clear();
            input.Value = null;

            return (zipFilePath, destinationFolder);
        }

        #endregion
    }
}
