/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.io.folder
{
    /// <summary>
    /// [io.path.get-folder] slot for making it easier to retrieve only the
    /// folder parts of some specified path.
    /// </summary>
    [Slot(Name = "io.path.get-folder")]
    public class GetPathFolder : ISlot
    {
        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var path = input.GetEx<string>();
            if (path.EndsWith("/"))
            {
                input.Value = path;
            }
            else
            {
                var entities = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                input.Value =
                    "/" +
                    string.Join(
                        "/",
                        entities.Take(entities.Length - 1)) +
                    "/";
            }
        }
    }
}
