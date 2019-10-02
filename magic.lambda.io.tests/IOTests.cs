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
io.file.save:existing.txt
   .:foo
io.file.exists:/existing.txt
");
            Assert.True(lambda.Children.Skip(1).First().Get<bool>());
        }

        [Fact]
        public void SaveAndLoadFile()
        {
            var lambda = Common.Evaluate(@"
io.file.save:existing.txt
   .:foo
io.file.load:/existing.txt
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
io.file.save:existing.txt
   foo:error
io.file.load:/existing.txt
");
            Assert.Equal("success", lambda.Children.Skip(1).First().Get<string>());
        }

        [Fact]
        public void SaveOverwriteAndLoadFile()
        {
            var lambda = Common.Evaluate(@"
io.file.save:existing.txt
   .:foo
io.file.save:existing.txt
   .:foo1
io.file.load:/existing.txt
");
            Assert.Equal("foo1", lambda.Children.Skip(2).First().Get<string>());
        }

        [Fact]
        public void EnsureFileDoesntExist()
        {
            var lambda = Common.Evaluate(@"
io.file.exists:/non-existing.txt
");
            Assert.False(lambda.Children.First().Get<bool>());
        }
    }
}
