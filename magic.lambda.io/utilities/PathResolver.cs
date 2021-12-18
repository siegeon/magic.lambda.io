/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Linq;
using System.Text;

namespace magic.lambda.io.utilities
{
    /*
     * Helper class to handle paths
     */
    internal static class PathResolver
    {
        /*
         * Combines the specified paths into a single path.
         */
        public static string Combine(params string[] args)
        {
            var builder = new StringBuilder();
            foreach (var idxArg in args)
            {
                foreach (var idxEntity in idxArg.Split(new char[] {'/', '\\'}, System.StringSplitOptions.RemoveEmptyEntries))
                {
                    builder.Append("/").Append(idxEntity);
                }
            }
            if (args.Last().EndsWith("/"))
                builder.Append("/");
            return builder.ToString();
        }
    }
}
