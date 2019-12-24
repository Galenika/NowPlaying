using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace NowPlaying.Models
{
    public class SteamIdLooker
    {
        private static string _steamFullPathCached;
        private static string SteamFullPath
        {
            get
            {
                if (_steamFullPathCached != null)
                    return _steamFullPathCached;

                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

                var path = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null) as string;

                if (string.IsNullOrEmpty(path))
                    throw new DirectoryNotFoundException("Unable to locate the steam folder");

                return _steamFullPathCached = path;
            }
        }

        public static string SteamLastLoggedOnAccount
        {
            get
            {
                var account = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "AutoLoginUser", null) as string;

                if (string.IsNullOrEmpty(account))
                    throw new DirectoryNotFoundException("Unable to locate last logged-on account");

                return account;
            }
        }

        public static string UserdataPath => SteamFullPath + @"\userdata";

        private static int ConvertSteamId64ToSteamId32(long steamId64)
        {
            // https://stackoverflow.com/a/23259939/5512932
            return (int)(steamId64 - 76561197960265728);
        }

        public static AccountsInfo GetAccountsInfo()
        {
            var loginUsersPath = SteamFullPath + @"\config\loginusers.vdf";

            var loginUsersFile = File.ReadAllLines(loginUsersPath).Skip(2);

            var regexSteamId64 = new Regex(@"(765611)\d+");
            var regexAcc = new Regex(@"AccountName""\s*""(\w+)");

            var loginUsersFileMatches =
                        loginUsersFile
                            .Select(line => new
                            {
                                SteamId64Match = regexSteamId64.Match(line),
                                AccountNameMatch = regexAcc.Match(line),
                            });

            var userdataNumbers = loginUsersFileMatches
                            .Select(matches => matches.SteamId64Match)
                            .Where(match => match.Success)
                            .Select(match => long.Parse(match.Value))
                            .Select(ConvertSteamId64ToSteamId32)
                            .ToArray();
            
            var accountNames = loginUsersFileMatches
                                    .Select(matches => matches.AccountNameMatch)
                                    .Where(match => match.Success)
                                    .Select(match => match.Groups[1].Value)
                                    .ToArray();

            var accountNameToSteamId3 = accountNames
                                            .Select((x, i) => new { Item = x, Index = i })
                                            .ToDictionary(x => x.Item, x => userdataNumbers[x.Index]);

            return new AccountsInfo
            {
                UserdataNumbers = userdataNumbers,
                AccountNames = accountNames,
                AccountNameToSteamId3 = accountNameToSteamId3,
            };
        }
    }
}
