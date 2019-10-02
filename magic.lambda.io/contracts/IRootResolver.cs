/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

namespace magic.lambda.io.contracts
{
    /// <summary>
    /// Contracts for resolving root folder on disc for magic.lambda.io
    /// </summary>
    public interface IRootResolver
    {
        /// <summary>
        /// Returns the root folder that magic.lambda.io should treat as the root folder for its IO operations.
        /// </summary>
        string RootFolder { get; }
    }
}
