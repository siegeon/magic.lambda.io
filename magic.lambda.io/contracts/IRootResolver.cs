/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

namespace magic.lambda.io.contracts
{
    /// <summary>
    /// Contract for resolving root folder on disc for magic.lambda.io
    /// </summary>
    public interface IRootResolver
    {
        /// <summary>
        /// Returns the root folder that magic.lambda.io should treat as the root folder for its IO operations.
        /// </summary>
        string RootFolder { get; }
    }
}
