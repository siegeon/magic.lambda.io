/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.IO;
using System.Text;
using System.Linq;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.contracts;
using magic.lambda.io.utilities;

namespace magic.lambda.io.files
{
    /// <summary>
    /// [io.files.load] slot for loading a file on your server.
    /// </summary>
    [Slot(Name = "io.files.eval")]
    public class EvalFile : ISlot
    {
        readonly IRootResolver _rootResolver;

        /// <summary>
        /// Constructs a new instance of your type.
        /// </summary>
        /// <param name="rootResolver">Instance used to resolve the root folder of your app.</param>
        public EvalFile(IRootResolver rootResolver)
        {
            _rootResolver = rootResolver ?? throw new ArgumentNullException(nameof(rootResolver));
        }

        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            // Loading file and converting to lambda.
            var content = File.ReadAllText(PathResolver.CombinePaths(_rootResolver.RootFolder, input.GetEx<string>()), Encoding.UTF8);
            var lambda = new Node("", content);
            signaler.Signal("lambda", lambda);

            // Removing any [.arguments] declaration nodes in file, if there are any.
            foreach (var idx in lambda.Children.Where(x => x.Name == ".arguments").ToList())
            {
                idx.UnTie();
            }

            // Adding arguments to lambda, if there are any.
            if (input.Children.Any())
                lambda.Insert(0, new Node(".arguments", null, input.Children.ToList()));

            // Making sure we can trap return value.
            var result = new Node();
            signaler.Scope("slots.result", result, () =>
            {
                // Evaluating lambda of slot.
                signaler.Signal("eval", lambda);

                // Clearing Children collection, since it might contain input parameters.
                input.Clear();

                /*
                * Simply setting value and children to "slots.result" stack object, since its value
                * was initially set to its old value during startup of method.
                */
                input.Value = result.Value;
                input.AddRange(result.Children.ToList());
            });
        }
    }
}
