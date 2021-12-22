/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.io.helpers
{
    internal static class Utilities
    {
        /*
         * Commonalities between copy and move slots for both files and folders.
         */
        internal static void CopyMoveHelper(
            ISignaler signaler,
            IRootResolver rootResolver,
            Node input,
            IIOService service,
            bool copy,
            bool isFolder)
        {
            // Sanity checking arguments and evaluating them.
            Utilities.SanityCheckArguments(input);
            signaler.Signal("eval", input);

            // Retrieving source and destination path.
            var paths = Utilities.GetPaths(input, rootResolver, isFolder);

            // Checking if IO object exists, at which point we delete it.
            if (service.Exists(paths.Destination))
                service.Delete(paths.Destination);

            // Copying or moving file depending upon caller's needs.
            if (copy)
                service.Copy(
                    paths.Source,
                    paths.Destination);
            else
                service.Move(
                    paths.Source,
                    paths.Destination);
        }

        /*
         * Commonalities between copy and move slots for both files and folders.
         */
        internal static async Task CopyMoveHelperAsync(
            ISignaler signaler,
            IRootResolver rootResolver,
            Node input,
            IIOService service,
            bool copy,
            bool isFolder)
        {
            // Sanity checking arguments and evaluating them.
            Utilities.SanityCheckArguments(input);
            await signaler.SignalAsync("eval", input);

            // Retrieving source and destination path.
            var paths = GetPaths(input, rootResolver, isFolder);

            // Checking if IO object exists, at which point we delete it.
            if (await service.ExistsAsync(paths.Destination))
                await service.DeleteAsync(paths.Destination);


            // Copying or moving file depending upon caller's needs.
            if (copy)
                await service.CopyAsync(
                    paths.Source,
                    paths.Destination);
            else
                await service.MoveAsync(
                    paths.Source,
                    paths.Destination);
        }

        #region [ -- Private helper methods -- ]

        /*
         * Sanity checks arguments for copy and move file/folder.
         */
        static void SanityCheckArguments(Node input)
        {
            if (!input.Children.Any())
                throw new HyperlambdaException("No destination provided to [io.file.copy]");
        }

        /*
         * Retrieves source and destination path for copy/move file/folder.
         */
        static (string Source, string Destination) GetPaths(Node input, IRootResolver rootResolver, bool isFolder)
        {
            // Finding absolute paths.
            var sourcePath = rootResolver.AbsolutePath(input.GetEx<string>());
            var destinationPath = rootResolver.AbsolutePath(input.Children.First().GetEx<string>());

            // Defaulting filename to the filename of the source file, unless another filename is explicitly given.
            if (!isFolder && destinationPath.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                destinationPath += Path.GetFileName(sourcePath);

            // Sanity checking arguments.
            if (sourcePath == destinationPath)
                throw new HyperlambdaException($"You cannot [{input.Name}] a file using the same source and destination path");

            // Returning arguments to caller.
            return (sourcePath, destinationPath);
        }

        #endregion
    }
}
