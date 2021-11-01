/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using magic.node;
using magic.signals.services;
using magic.signals.contracts;
using magic.lambda.io.contracts;
using magic.node.extensions.hyperlambda;

namespace magic.lambda.io.tests.helpers
{
    public static class Common
    {
        public class RootResolver : IRootResolver
        {
            public string RootFolder => AppDomain.CurrentDomain.BaseDirectory;
        }

        static public Node Evaluate(
            string hl,
            FileService fileService = null,
            FolderService folderService = null,
            StreamService streamService = null)
        {
            var services = Initialize(fileService, folderService, streamService);
            var lambda = HyperlambdaParser.Parse(hl);
            var signaler = services.GetService(typeof(ISignaler)) as ISignaler;
            signaler.Signal("eval", lambda);
            return lambda;
        }

        static public async Task<Node> EvaluateAsync(
            string hl,
            FileService fileService = null,
            FolderService folderService = null,
            StreamService streamService = null)
        {
            var services = Initialize(fileService, folderService, streamService);
            var lambda = HyperlambdaParser.Parse(hl);
            var signaler = services.GetService(typeof(ISignaler)) as ISignaler;
            await signaler.SignalAsync("eval", lambda);
            return lambda;
        }

        #region [ -- Private helper methods -- ]

        static IServiceProvider Initialize(
            FileService fileService = null,
            FolderService folderService = null,
            StreamService streamService = null)
        {
            var configuration = new ConfigurationBuilder().Build();
            var services = new ServiceCollection();
            services.AddTransient<IConfiguration>((svc) => configuration);
            services.AddTransient<ISignaler, Signaler>();
            if (fileService == null)
                services.AddTransient<IFileService, file.services.FileService>();
            else
                services.AddTransient<IFileService>(svc => fileService);
            if (folderService == null)
                services.AddTransient<IFolderService, folder.services.FolderService>();
            else
                services.AddTransient<IFolderService>(svc => folderService);
            if (streamService == null)
                services.AddTransient<IStreamService, stream.services.StreamService>();
            else
                services.AddTransient<IStreamService>(svc => streamService);
            services.AddTransient<IRootResolver, RootResolver>();
            var types = new SignalsProvider(InstantiateAllTypes<ISlot>(services));
            services.AddTransient<ISignalsProvider>((svc) => types);
            var provider = services.BuildServiceProvider();
            return provider;
        }

        static IEnumerable<Type> InstantiateAllTypes<T>(ServiceCollection services) where T : class
        {
            var type = typeof(T);
            var result = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && !x.FullName.StartsWith("Microsoft", StringComparison.InvariantCulture))
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            foreach (var idx in result)
            {
                services.AddTransient(idx);
            }
            return result;
        }

        #endregion
    }
}
