﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Fixie.Execution;

namespace Fixie.VisualStudio.TestAdapter
{
    [DefaultExecutorUri(VsTestExecutor.Id)]
    [FileExtension(".exe")]
    [FileExtension(".dll")]
    public class VsTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger log, ITestCaseDiscoverySink discoverySink)
        {
            log.Version();

            RemotingUtility.CleanUpRegisteredChannels();

            foreach (var assemblyPath in sources)
            {
                try
                {
                    if (AssemblyDirectoryContainsFixie(assemblyPath))
                    {
                        log.Info("Processing " + assemblyPath);

                        using (var environment = new ExecutionEnvironment(assemblyPath))
                        {
                            var methodGroups = environment.DiscoverTestMethodGroups(new Options());

                            foreach (var methodGroup in methodGroups)
                                discoverySink.SendTestCase(new TestCase(methodGroup.FullName, VsTestExecutor.Uri, assemblyPath));
                        }
                    }
                    else
                    {
                        log.Info("Skipping " + assemblyPath + " because it is not a test assembly.");
                    }
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                }
            }
        }

        static bool AssemblyDirectoryContainsFixie(string assemblyPath)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(assemblyPath), "Fixie.dll"));
        }
    }
}
