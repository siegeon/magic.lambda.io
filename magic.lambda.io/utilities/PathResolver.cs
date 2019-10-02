/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.IO;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.contracts;

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
