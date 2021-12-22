/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;

namespace magic.lambda.io.folder
{
    internal static class Utilities
    {
        /*
         * Sanity checks arguments for copy and move file/folder.
         */
        internal static void SanityCheckArguments(Node input)
        {
            if (!input.Children.Any())
                throw new HyperlambdaException("No destination provided to [io.file.copy]");
        }

        /*
         * Retrieves source and destination path for copy/move file/folder.
         */
        internal static (string SourcePath, string DestinationPath) GetPaths(Node input, IRootResolver rootResolver)
        {
            // Finding absolute paths.
            var sourcePath = rootResolver.AbsolutePath(input.GetEx<string>());
            var destinationPath = rootResolver.AbsolutePath(input.Children.First().GetEx<string>());

            // Defaulting filename to the filename of the source file, unless another filename is explicitly given.
            if (destinationPath.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                destinationPath += Path.GetFileName(sourcePath);

            // Sanity checking arguments.
            if (sourcePath == destinationPath)
                throw new HyperlambdaException($"You cannot [{input.Name}] a file using the same source and destination path");

            // Returning arguments to caller.
            return (sourcePath, destinationPath);
        }
    }
}
