/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using magic.node;
using magic.signals.contracts;
using System.IO;
using magic.node.extensions;
using System;
using System.Linq;
using System.Text;

namespace magic.lambda.io.file
{
    /// <summary>
    /// [io.content.zip-stream] slot for zipping a bunch of files into a specified stream.
    /// </summary>
    [Slot(Name = "io.content.zip-stream")]
    public class ZipContent : ISlot
    {
        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            // Evaluating all filenames, in case they're slot invocations.
            signaler.Signal("eval", input);
            var result = new MemoryStream();
            using (var zipStream = new ZipOutputStream(result))
            {
                zipStream.IsStreamOwner = false;
                zipStream.SetLevel(3);
                foreach (var idx in input.Children)
                {
                    // Evaluating content node, in case it's a slot invocation.
                    signaler.Signal("eval", idx);
                    var newEntry = new ZipEntry(ZipEntry.CleanName(idx.GetEx<string>()))
                    {
                        DateTime = DateTime.Now
                    };
                    var content = idx.Children.First().GetEx<string>();
                    newEntry.Size = content.Length;
                    zipStream.PutNextEntry(newEntry);
                    using (var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                    {
                        contentStream.CopyTo(zipStream);
                    }
                }
            }
            result.Position = 0;
            input.Value = result;
        }
    }
}
