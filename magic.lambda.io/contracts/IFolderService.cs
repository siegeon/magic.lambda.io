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
    public interface IFolderService
    {
        void Create(string path);

        bool Exists(string path);

        void Delete(string path);

        IEnumerable<string> ListFolders(string folder);
    }
}
