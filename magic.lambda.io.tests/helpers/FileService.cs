/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using magic.lambda.io.contracts;

namespace magic.lambda.io.tests.helpers
{
    public class FileService : IFileService
    {
        public Action<string, string> CopyAction { get; set; }

        public Func<string, string, Task> CopyAsyncAction { get; set; }

        public Action<string> DeleteAction { get; set; }

        public Func<string, bool> ExistsAction { get; set; }

        public Func<string, IEnumerable<string>> ListFilesAction { get; set; }

        public Func<string, string> LoadAction { get; set; }

        public Func<string, Task<string>> LoadAsyncAction { get; set; }

        public Action<string, string> MoveAction { get; set; }

        public Action<string, string> SaveAction { get; set; }

        public Func<string, string, Task> SaveAsyncAction { get; set; }

        public void Copy(string source, string destination)
        {
            CopyAction(source, destination);
        }

        public async Task CopyAsync(string source, string destination)
        {
            await CopyAsyncAction(source, destination);
        }

        public void Delete(string path)
        {
            DeleteAction(path);
        }

        public bool Exists(string path)
        {
            return ExistsAction(path);
        }

        public IEnumerable<string> ListFiles(string folder)
        {
            return ListFilesAction(folder);
        }

        public string Load(string path)
        {
            return LoadAction(path);
        }

        public async Task<string> LoadAsync(string path)
        {
            return await LoadAsyncAction(path);
        }

        public void Move(string source, string destination)
        {
            MoveAction(source, destination);
        }

        public void Save(string path, string content)
        {
            SaveAction(path, content);
        }

        public async Task SaveAsync(string filename, string content)
        {
            await SaveAsyncAction(filename, content);
        }
    }
}
