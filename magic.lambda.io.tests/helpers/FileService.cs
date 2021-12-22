/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using magic.node.contracts;

namespace magic.lambda.io.tests.helpers
{
    public class FileService : IFileService
    {
        public Action<string, string> CopyAction { get; set; }

        public Func<string, string, Task> CopyAsyncAction { get; set; }

        public Action<string> DeleteAction { get; set; }

        public Func<string, bool> ExistsAction { get; set; }

        public Func<string, List<string>> ListFilesAction { get; set; }

        public Func<string, string> LoadAction { get; set; }

        public Func<string, Task<string>> LoadAsyncAction { get; set; }

        public Func<string, byte[]> LoadBinaryAction { get; set; }

        public Func<string, Task<byte[]>> LoadBinaryAsyncAction { get; set; }

        public Action<string, string> MoveAction { get; set; }

        public Action<string, string> SaveAction { get; set; }

        public Func<string, string, Task> SaveAsyncAction { get; set; }

        public bool Exists(string path)
        {
            return ExistsAction(path);
        }

        public Task<bool> ExistsAsync(string path)
        {
            return Task.FromResult(ExistsAction(path));
        }

        public void Delete(string path)
        {
            DeleteAction(path);
        }

        public Task DeleteAsync(string path)
        {
            DeleteAction(path);
            return Task.CompletedTask;
        }

        public void Copy(string source, string destination)
        {
            CopyAction(source, destination);
        }

        public Task CopyAsync(string source, string destination)
        {
            return Task.FromResult(CopyAsyncAction(source, destination));
        }

        public void Move(string source, string destination)
        {
            MoveAction(source, destination);
        }

        public Task MoveAsync(string source, string destination)
        {
            MoveAction(source, destination);
            return Task.CompletedTask;
        }

        public string Load(string path)
        {
            return LoadAction(path);
        }

        public Task<string> LoadAsync(string path)
        {
            return LoadAsyncAction(path);
        }

        public byte[] LoadBinary(string path)
        {
            return LoadBinaryAction(path);
        }

        public Task<byte[]> LoadBinaryAsync(string path)
        {
            return LoadBinaryAsyncAction(path);
        }

        public void Save(string path, string content)
        {
            SaveAction(path, content);
        }

        public Task SaveAsync(string filename, string content)
        {
            return Task.FromResult(SaveAsyncAction(filename, content));
        }

        public void Save(string path, byte[] content)
        {
            SaveAction(path, Encoding.UTF8.GetString(content));
        }

        public Task SaveAsync(string filename, byte[] content)
        {
            return Task.FromResult(SaveAsyncAction(filename, Encoding.UTF8.GetString(content)));
        }

        public List<string> ListFiles(string folder, string extension = null)
        {
            return ListFilesAction(folder);
        }

        public Task<List<string>> ListFilesAsync(string folder, string extension = null)
        {
            return Task.FromResult(ListFilesAction(folder));
        }
    }
}
