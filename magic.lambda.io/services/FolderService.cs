/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.IO;
using magic.lambda.io.contracts;
using System.Collections.Generic;

namespace magic.lambda.io.folder.services
{
    /// <inheritdoc/>
    public class FolderService : IFolderService
    {
        /// <inheritdoc/>
        public void Create(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <inheritdoc/>
        public void Delete(string path)
        {
            Directory.Delete(path, true);
        }

        /// <inheritdoc/>
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        /// <inheritdoc/>
        public void Move(string source, string destination)
        {
            Directory.Move(source, destination);
        }

        /// <inheritdoc/>
        public void Copy(string source, string destination)
        {
            // Sanity checking invocation, and verifying source folder exists.
            var sourceFolder = new DirectoryInfo(source);
            if (!sourceFolder.Exists)
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found '{source}'");
            Directory.CreateDirectory(destination);

            foreach (var file in dir.GetFiles())
            {
                file.CopyTo(Path.Combine(destination, file.Name), false);
            }
            foreach (var idxSub in dir.GetDirectories())
            {
                Copy(idxSub.FullName, Path.Combine(destination, idxSub.Name));
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> ListFolders(string folder)
        {
            return Directory.GetDirectories(folder);
        }
    }
}
