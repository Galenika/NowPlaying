﻿namespace NowPlaying.Core.Steam
{
    public sealed class SteamInfo
    {
        public SteamInfo(string fullPath, string lastAccount)
        {
            this.FullPath = fullPath;
            this.LastAccount = lastAccount;
        }

        public string FullPath { get; }

        public string LastAccount { get; }

        public string UserdataPath => FullPath + @"\userdata";

        public string LoginUsersPath => FullPath + @"\config\loginusers.vdf";
    }
}
