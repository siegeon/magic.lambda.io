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
         * Commonalities between the move/copy folder/file async/sync implementations.
         */
        public static void Execute(
            ISignaler signaler,
            IRootResolver resolver,
            Node input,
            string slot,
            Action<string, string> functor)
        {
            SanityCheckInvocation(input, slot);
            signaler.Signal("eval", input);
            ExecuteImplementation(resolver, input, functor);
        }

        /*
         * Commonalities between the move/copy folder/file async/sync implementations.
         */
        public static async Task ExecuteAsync(
            ISignaler signaler,
            IRootResolver resolver,
            Node input,
            string slot,
            Action<string, string> functor)
        {
            SanityCheckInvocation(input, slot);
            await signaler.SignalAsync("eval", input);
            ExecuteImplementation(resolver, input, functor);
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
        static void ExecuteImplementation(
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
