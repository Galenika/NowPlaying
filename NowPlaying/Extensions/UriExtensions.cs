﻿using System.Linq;

namespace NowPlaying.Extensions
{
    internal static class UriExtensions
    {
        public static string GetPropertyValue(string uri, string propertyName)
        {
            var urlParams = uri.Split(new char[] { '?', '&' });
            return urlParams.FirstOrDefault(p => p.Contains(propertyName + "="))
                                    .Split('=')[1]; // {propertyName}=*text*" split by '=', take *text*
        }
    }
}
