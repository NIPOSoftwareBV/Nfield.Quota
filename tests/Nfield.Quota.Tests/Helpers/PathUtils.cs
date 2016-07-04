using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Nfield.Quota.Tests.Helpers
{
    public static class PathUtils
    {
        public static string GetTestProjectDirectory()
        {
            var testDir = TestContext.CurrentContext.TestDirectory;

            // Strip \bin\{Configuration} or \bin\x64\{Configuration}
            var projectDir = new DirectoryInfo(testDir).Parent?.Parent;
            if (testDir.Contains("\\x64\\"))
            {
                projectDir = projectDir?.Parent;
            }

            return projectDir?.FullName;
        }

        public static string GetSolutionRootDirectory()
        {
            var projectDir = GetTestProjectDirectory();

            // assume test project dir is place one level higher than solution dir
            return new DirectoryInfo(projectDir).Parent?.FullName;
        }
    }
}
