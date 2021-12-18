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
                throw new HyperlambdaException($"No destination provided to [{slot}]");
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
            // Sanity checking arguments.
            functor(rootResolver.AbsolutePath(input.GetEx<string>()), rootResolver.AbsolutePath(input.Children.First().GetEx<string>()));
        }

        #endregion
    }
}
