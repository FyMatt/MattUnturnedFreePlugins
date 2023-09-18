
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Core.Serialization;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace 游戏白天黑夜游戏模式切换
{
    public class Main:RocketPlugin<Config>
    {
        private DateTime time = new DateTime();
        protected override void Load()
        {
            Logger.LogWarning($"欢迎使用 {Assembly.GetName().Name} 版本：{Assembly.GetName().Version}");
            Logger.LogWarning("Matt插件群：656525189");

            time = DateTime.Now;
        }

        private void FixedUpdate()
        {
            TimeSpan span = DateTime.Now - time;
            if (span.TotalSeconds >= 3)
            {
                time = DateTime.Now;
                string msg = "PVP";
                if (LightingManager.isDaytime)
                {
                    if (Provider.isPvP != Configuration.Instance.isPvp)
                    {
                        if (Configuration.Instance.isPvp) msg = "PVP";
                        else msg = "PVE";
                        Provider.isPvP = Configuration.Instance.isPvp;
                        UnturnedChat.Say(Translate("进入白天", msg), true);
                    }
                }
                else if (LightingManager.isNighttime)
                {
                    if (Provider.isPvP != !Configuration.Instance.isPvp)
                    {
                        if (!Configuration.Instance.isPvp) msg = "PVP";
                        else msg = "PVE";
                        Provider.isPvP = !Configuration.Instance.isPvp;
                        UnturnedChat.Say(Translate("进入黑夜", msg), true);
                    }
                }
            }
        }


        protected override void Unload()
        {
            base.Unload();  
        }


        public override TranslationList DefaultTranslations => new TranslationList
        {
            {"进入白天","天亮了！进入<color=#ff00ff>{0}</color>模式" },
            {"进入黑夜","天黑了！进入<color=#ff00ff>{0}</color>模式" }
        };
    }
    public class Config : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            isPvp = false;
        }
        [XmlElement("白天是否为pvp")]
        public bool isPvp;
    }
}
