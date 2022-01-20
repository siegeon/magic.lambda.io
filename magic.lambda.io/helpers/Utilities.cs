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
            var paths = Utilities.GetCopyMovePathsPaths(input, rootResolver, isFolder);

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
            var paths = GetCopyMovePathsPaths(input, rootResolver, isFolder);

            // Checking if file, and file exists, at which point we delete it before copying.
            if (!isFolder && await service.ExistsAsync(paths.Destination))
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
        static (string Source, string Destination) GetCopyMovePathsPaths(
            Node input,
            IRootResolver rootResolver,
            bool isFolder)
        {
            // Retrieving source and destination paths as specified by caller.
            var src = input.GetEx<string>();
            var dest = input.Children.First().GetEx<string>();

            // Normalising paths for folders.
            if (isFolder)
            {
                // Folders.
                if (!src.EndsWith("/"))
                    src += "/";
                if (!dest.EndsWith("/"))
                    dest += "/";
            }
            else
            {
                // Notice, we default the filename of destination to source's filename unless explicitly specified by caller.
                if (dest.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                    dest += Path.GetFileName(src);
            }

            // Transforming relative paths to absolute paths.
            var source = rootResolver.AbsolutePath(src);
            var destination = rootResolver.AbsolutePath(dest);

            // Sanity checking arguments.
            if (source == destination)
                throw new HyperlambdaException($"You cannot [{input.Name}] a file using the same source and destination path");

            // Returning arguments to caller.
            return (source, destination);
        }

        #endregion
    }
}
