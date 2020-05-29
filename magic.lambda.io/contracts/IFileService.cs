/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Threading.Tasks;
using System.Collections.Generic;

namespace magic.lambda.io.contracts
{
    /// <summary>
    /// Contracts for handling files for magic.lambda.io
    /// </summary>
    public interface IFileService
    {
        bool Exists(string path);

        void Delete(string path);

        void Copy(string source, string destination);

        void Move(string source, string destination);

        Task CopyAsync(string source, string detination);

        string Load(string path);

        Task<string> LoadAsync(string path);

        void Save(string filename, string content);

        Task SaveAsync(string filename, string content);

        IEnumerable<string> ListFiles(string folder);
    }
}
