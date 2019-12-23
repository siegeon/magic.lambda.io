/*
 * Magic, Copyright(c) Thomas Hansen 2019, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.contracts;

namespace magic.lambda.io.files
{
    /// <summary>
    /// [io.files.zip] slot for zipping a bunch of files into a specified stream.
    /// </summary>
    [Slot(Name = "io.content.zip")]
    public class ZipContent : ISlot
    {
        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            /*
             * TODO: Figure out some intelligent way to handle exceptions, since this might in theory leak a stream,
             * although it's just a MemoryStream, and NOT an actual OS resource, it's still not nice.
             */
            var result = new MemoryStream();
            var outStream = new ZipOutputStream(result);
            var sw = new StreamWriter(outStream);
            foreach (var idx in input.Children)
            {
                outStream.PutNextEntry(new ZipEntry(idx.GetEx<string>()));
                sw.Write(idx.Children.FirstOrDefault()?.GetEx<string>() ?? "");
            }
            sw.Flush();

            // Removing children nodes, just to be on the sure side!
            input.Clear();
            result.Position = 0;
            input.Value = result;
        }
    }
}
