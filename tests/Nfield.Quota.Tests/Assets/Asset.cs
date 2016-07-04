using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfield.Quota.Tests.Helpers;

namespace Nfield.Quota.Tests.Assets
{
    internal static class Asset
    {
        public static string GetAbsolutePath(params string[] relativePath)
        {
            var projectDir = PathUtils.GetTestProjectDirectory();
            var assetsDir = Path.Combine(projectDir, "Assets");
            var filePath = Path.Combine(assetsDir, Path.Combine(relativePath));

            if (!File.Exists(filePath))
            {
                var relPath = string.Join(Path.DirectorySeparatorChar.ToString(), relativePath);
                throw new FileNotFoundException($"Asset '{relPath}' was not found under '{assetsDir}'.");
            }

            return filePath;
        }
    }
}
