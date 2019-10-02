/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.Linq;
using Xunit;
using magic.node.extensions;
using magic.signals.contracts;
using magic.node;

namespace magic.lambda.io.tests
{
    public class IOTests
    {
        [Fact]
        public void SaveFile()
        {
            var lambda = Common.Evaluate(@"
io.files.save:existing.txt
   .:foo
io.files.exists:/existing.txt
");
            Assert.True(lambda.Children.Skip(1).First().Get<bool>());
        }

        [Fact]
        public void SaveAndLoadFile()
        {
            var lambda = Common.Evaluate(@"
io.files.save:existing.txt
   .:foo
io.files.load:/existing.txt
");
            Assert.Equal("foo", lambda.Children.Skip(1).First().Get<string>());
        }

        [Slot(Name = "foo")]
        public class EventSource : ISlot
        {
            public void Signal(ISignaler signaler, Node input)
            {
                input.Value = "success";
            }
        }

        [Fact]
        public void SaveWithEventSourceAndLoadFile()
        {
            var lambda = Common.Evaluate(@"
io.files.save:existing.txt
   foo:error
io.files.load:/existing.txt
");
            Assert.Equal("success", lambda.Children.Skip(1).First().Get<string>());
        }

        [Fact]
        public void SaveOverwriteAndLoadFile()
        {
            var lambda = Common.Evaluate(@"
io.files.save:existing.txt
   .:foo
io.files.save:existing.txt
   .:foo1
io.files.load:/existing.txt
");
            Assert.Equal("foo1", lambda.Children.Skip(2).First().Get<string>());
        }

        [Fact]
        public void EnsureFileDoesntExist()
        {
            var lambda = Common.Evaluate(@"
io.files.exists:/non-existing.txt
");
            Assert.False(lambda.Children.First().Get<bool>());
        }

        [Fact]
        public void ListFiles()
        {
            var lambda = Common.Evaluate(@"
io.files.list:/
");
            Assert.True(lambda.Children.First().Children.Count() > 5);
            Assert.True(lambda.Children.First().Children
                .Where(x => x.Get<string>().StartsWith("/")).Count() == lambda.Children.First().Children.Count());
        }

        [Fact]
        public void ListFolders()
        {
            var lambda = Common.Evaluate(@"
io.folders.list:/
");
        }

        [Fact]
        public void CreateFolderListFolders()
        {
            var lambda = Common.Evaluate(@"
io.folders.create:/foo
io.folders.list:/
");
            Assert.Single(lambda.Children.Skip(1).First().Children);
            Assert.Equal("/foo/", lambda.Children.Skip(1).First().Children.First().Get<string>());
        }
    }
}
