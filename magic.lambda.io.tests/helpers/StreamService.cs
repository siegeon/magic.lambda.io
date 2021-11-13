/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.IO;
using System.Threading.Tasks;
using magic.lambda.io.contracts;

namespace magic.lambda.io.tests.helpers
{
    public class StreamService : IStreamService
    {
        public Func<string, Stream> OpenFileAction { get; set; }
        public Action<Stream, string> SaveFileAction { get; set; }
        public Action<Stream, string> SaveFileActionAsync { get; set; }

        public Stream OpenFile(string path)
        {
            return OpenFileAction(path);
        }

        public void SaveFile(Stream stream, string path)
        {
            SaveFileAction(stream, path);
        }

        public Task SaveFileAsync(Stream stream, string path)
        {
            SaveFileActionAsync(stream, path);
            return Task.CompletedTask;
        }
    }
}
