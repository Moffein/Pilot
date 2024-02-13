using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pilot.Modules
{
    internal static class SoundBanks
    {
        private static bool initialized = false;
        public static string SoundBankDirectory
        {
            get
            {
                return Path.Combine(Files.assemblyDir, "SoundBanks");
            }
        }

        public static void Init()
        {
            if (initialized) return;
            initialized = true;
            AKRESULT akResult = AkSoundEngine.AddBasePath(SoundBankDirectory);

            AkSoundEngine.LoadBank("PilotSoundbank.bnk", out _);
        }
    }
}
