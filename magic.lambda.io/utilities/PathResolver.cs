/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

namespace magic.lambda.io.utilities
{
    /*
     * Helper class to handle paths
     */
    internal static class PathResolver
    {
        /*
         * Combines the two specified paths.
         */
        public static string CombinePaths(string root, string path)
        {
            return root.Replace("\\", "/").TrimEnd('/') + "/" + path.Replace("\\", "/").TrimStart('/');
        }

        /*
         * Makes sure paths hare handled similarly on Windows and Unix like systems.
         */
        public static string Normalize(string path)
        {
            return path.Replace("\\", "/").TrimEnd('/');
        }
    }
}
