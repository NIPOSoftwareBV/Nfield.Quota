using NUnit.Framework;
using System.IO;

namespace Nfield.Quota.Tests.Helpers
{
    public static class PathUtils
    {
        public static string GetTestProjectDirectory()
        {
            return TestContext.CurrentContext.TestDirectory;
        }

        public static string GetSolutionRootDirectory()
        {
            var projectDir = GetTestProjectDirectory();

            // assume test project dir is place one level higher than solution dir
            return new DirectoryInfo(projectDir).Parent?.FullName;
        }
    }
}
