
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace 公告UI
{
    public class Beta:RocketPlugin<Config>
    {
        DateTime time = new DateTime();
        int index = 0;
        List<CSteamID> players = new List<CSteamID> { };

        protected override void Load()
        {
            Logger.LogWarning($"欢迎使用 {Assembly.GetName().Name} 版本：{Assembly.GetName().Version}");
            Logger.LogWarning("Matt插件群：656525189");

            time = DateTime.Now;
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            base.Load();
        }



        private void FixedUpdate()
        {
            if (players.Count != 0)
            {
                TimeSpan span = DateTime.Now - time;
                if (span.TotalSeconds >= 30)
                {
                    time = DateTime.Now;
                    index++;
                    if (index >= Configuration.Instance.msg.Count)
                    {
                        index = 0;
                    }
                    for(int i = 0; i < players.Count; i++)
                    {
                        EffectManager.askEffectClearByID(Configuration.Instance.id, players[i]);
                        EffectManager.sendUIEffect(Configuration.Instance.id, (short)Configuration.Instance.id, players[i], false);
                        EffectManager.sendUIEffectText((short)Configuration.Instance.id, players[i], false, "通知内容_txt", Configuration.Instance.msg[index]);
                    }
                }
            }
        }
        private void Events_OnPlayerDisconnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            if (players.Contains(player.CSteamID)) players.Remove(player.CSteamID);
        }

        private void Events_OnPlayerConnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            players.Add(player.CSteamID);
            EffectManager.sendUIEffect(Configuration.Instance.id, (short)Configuration.Instance.id, player.CSteamID, false);
            EffectManager.sendUIEffectText((short)Configuration.Instance.id, player.CSteamID, false, "通知内容_txt", Configuration.Instance.msg[index]);
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            base.Unload();
        }
    }
    public class Config : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            id = 2001;
            msg.Add("Night Studio Msg 1 第一条公告第一条公告第一条公告第一条公告第一条公告第一条公告第一条公告");
            msg.Add("Night Studio Msg 2 第二条公告第二条公告第二条公告第二条公告第二条公告第二条公告第二条公告");
            msg.Add("Night Studio Msg 3 第三条公告  第三条公告   第三条公告  第三条公告  第三条公告");
        }
        [XmlElement("公告UI_ID")]
        public ushort id;
        [XmlArrayItem("公告内容"),XmlArray("公告信息列表")]
        public List<string> msg = new List<string> { };
    }

}
