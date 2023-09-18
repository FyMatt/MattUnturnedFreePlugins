using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Matt.查看建筑所有者
{
    public static class RequestUtil
    {
        private static string GetBetween(string strSource, string strStart, string strEnd)
        {
            if (!strSource.Contains(strStart) || !strSource.Contains(strEnd)) return "";
            var start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
            var end = strSource.IndexOf(strEnd, start, StringComparison.Ordinal);
            return strSource.Substring(start, end - start);
        }

        public static async Task<KeyValuePair<string, string>> GetNameAndGroup(CSteamID steamP, CSteamID steamG)
        {
            var nameUrl = $"http://steamcommunity.com/profiles/{steamP}?xml=1";
            var groupUrl = $"http://steamcommunity.com/gid/{steamG}/memberslistxml/?xml=1";
            using (var client = new WebClient())
            {
                var html = await client.DownloadStringTaskAsync(nameUrl);

                var nick = GetBetween(html, "<steamID>", "</steamID>").Replace(" ", "");
                nick = nick.Replace("<![CDATA[", "").Replace("]]>", "");

                var group = GetBetween(html, "<groupName>", "</groupName>").Replace(" ", "");
                group = group.Replace("<![CDATA[", "").Replace("]]>", "");

                if (!string.IsNullOrEmpty(group)) return new KeyValuePair<string, string>(nick, group);

                html = await client.DownloadStringTaskAsync(groupUrl);

                group = GetBetween(html, "<groupName>", "</groupName>").Replace(" ", "");
                group = group.Replace("<![CDATA[", "").Replace("]]>", "");

                return new KeyValuePair<string, string>(nick, group);
            }
        }
    }
}
