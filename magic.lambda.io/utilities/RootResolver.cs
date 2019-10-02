
/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

namespace magic.lambda.io.utilities
{
    /*
     * Utilitiy class for resolving the root folder of system.
     */
    internal static class RootResolver
    {
        /// <summary>
        /// Returns the absolute root position on disc where files are allowed to be manipulated.
        /// </summary>
        public static string Root { get; internal set; }
    }
}
