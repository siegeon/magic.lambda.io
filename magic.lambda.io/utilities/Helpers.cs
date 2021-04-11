/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.contracts;

namespace magic.lambda.io.utilities
{
    /*
     * Common helpers to keep code DRY.
     */
    internal static class Helpers
    {
        /*
         * Commonalities between the move folder and move file signals.
         */
        public static void Move(
            ISignaler signaler,
            IRootResolver resolver,
            Node input,
            string slot,
            Action<string, string> functor)
        {
            SanityCheckInvocation(input, slot);
            signaler.Signal("eval", input);
            MoveImplementation(resolver, input, functor);
        }

        /*
         * Async commonalities between the move folder and move file signals.
         */
        public static async Task MoveAsync(
            ISignaler signaler,
            IRootResolver resolver,
            Node input,
            string slot,
            Action<string, string> functor)
        {
            SanityCheckInvocation(input, slot);
            await signaler.SignalAsync("eval", input);
            MoveImplementation(resolver, input, functor);
        }

        #region [ -- Private helper methods -- ]

        /*
         * Sanity checks invocation to make sure its correctly parametrised.
         */
        static void SanityCheckInvocation(Node input, string slot)
        {
            if (!input.Children.Any())
                throw new ArgumentException($"No destination provided to [{slot}]");
        }

        /*
         * Actual implementation retrieving source and destination,
         * for the to invoke lambda callback supplied by caller.
         */
        static void MoveImplementation(
            IRootResolver rootResolver,
            Node input,
            Action<string, string> functor)
        {
            // Retrieving source path.
            string sourcePath = PathResolver.CombinePaths(
                rootResolver.RootFolder,
                input.GetEx<string>());

            // Retrieving destination path.
            var destinationPath = PathResolver
                .CombinePaths(
                    rootResolver.RootFolder,
                    input.Children.First().GetEx<string>());

            // Sanity checking arguments.
            functor(sourcePath, destinationPath);
        }

        #endregion
    }
}
