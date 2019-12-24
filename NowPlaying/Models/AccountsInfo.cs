using System.Collections.Generic;

namespace NowPlaying.Models
{
    public class AccountsInfo
    {
        public IList<int> UserdataNumbers { get; set; }
        public IList<string> AccountNames { get; set; }
        public IDictionary<string, int> AccountNameToSteamId3 { get; set; }
    }
}