﻿using MoffeinPilot.Modules;
using R2API;
using System;
using System.Linq;

namespace MoffeinPilot.Modules
{
    internal class LanguageTokens
    {
        internal static string languageRoot => System.IO.Path.Combine(LanguageTokens.assemblyDir, "language");

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Files.PluginInfo.Location);
            }
        }
        public LanguageTokens()
        {
            RegisterLanguageTokens();
        }

        public static void RegisterLanguageTokens()
        {
            On.RoR2.Language.SetFolders += fixme;
        }

        //Credits to Anreol for this code
        private static void fixme(On.RoR2.Language.orig_SetFolders orig, RoR2.Language self, System.Collections.Generic.IEnumerable<string> newFolders)
        {
            if (System.IO.Directory.Exists(LanguageTokens.languageRoot))
            {
                var dirs = System.IO.Directory.EnumerateDirectories(System.IO.Path.Combine(LanguageTokens.languageRoot), self.name);
                orig(self, newFolders.Union(dirs));
                return;
            }
            orig(self, newFolders);
        }
    }
}