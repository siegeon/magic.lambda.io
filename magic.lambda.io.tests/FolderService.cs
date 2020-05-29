/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Collections.Generic;
using magic.lambda.io.contracts;

namespace magic.lambda.io.tests
{
    public class FolderService : IFolderService
    {
        public Action<string> CreateAction { get; set; }

        public Func<string, IEnumerable<string>> ListAction { get; set; }

        public Action<string> DeleteAction { get; set; }

        public Func<string, bool> ExistsAction { get; set; }

        public void Create(string path)
        {
            CreateAction(path);
        }

        public void Delete(string path)
        {
            DeleteAction(path);
        }

        public bool Exists(string path)
        {
            return ExistsAction(path);
        }

        public IEnumerable<string> ListFolders(string folder)
        {
            return ListAction(folder);
        }
    }
}
