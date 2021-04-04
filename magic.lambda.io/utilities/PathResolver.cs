/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

namespace magic.lambda.io.utilities
{
    internal static class PathResolver
    {
        public static string CombinePaths(string root, string path)
        {
            return root.Replace("\\", "/").TrimEnd('/') + "/" + path.Replace("\\", "/").TrimStart('/');
        }

        public static string Normalize(string path)
        {
            return path.Replace("\\", "/").TrimEnd('/');
        }
    }
}
