﻿using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MoffeinPilot.Modules
{
    internal class Files
    {
        public static PluginInfo PluginInfo;

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(PluginInfo.Location);
            }
        }

        internal static void Init(PluginInfo info)
        {
            PluginInfo = info;
        }

        internal static string GetPathToFile(string folderName, string fileName)
        {
            return Path.Combine(assemblyDir, folderName, fileName);
        }
    }
}
