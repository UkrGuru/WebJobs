﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using UkrGuru.SqlJson;

namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static bool InitDb(this Assembly assembly)
        {
            assembly.ThrowIfNull(nameof(assembly));

            var product_name = assembly.GetName().Name;
            var product_version = assembly.GetName().Version.ToString();
            // var product_version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            var db_version = "1.0.0.0";
            try { db_version = DbHelper.FromProc($"WJbSettings_Get", product_name); } catch { }

            if (db_version.CompareTo(product_version) < 0)
            {
                var version_file = $"{product_name}.Resources.{db_version ?? "1.0.0.0"}.sql";

                var resourceNames = assembly.GetManifestResourceNames()
                    .Where(s => s.EndsWith(".sql") && s.CompareTo(version_file) >= 0)
                    .OrderBy(s => s);

                foreach (var resourceName in resourceNames) assembly.ExecResource(resourceName);

                try { DbHelper.ExecProc($"WJbSettings_Set", new { Name = product_name, Value = product_version }); } catch { }
            }

            //try { DbHelper.ExecProc($"WJbQueue_FinishAll"); } catch { }

            return true;
        }
    }
}