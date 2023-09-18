using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.API.Collections;
using Rocket.Unturned.Chat;
using System;
using SDG.Framework.IO.Streams.BitStreams;
using System.Reflection;
using Rocket.API;
using System.Xml.Serialization;
using Rocket.API.Serialisation;
using Rocket.Core.Permissions;
using Rocket.Core;
using System.Data.SqlTypes;

namespace Matt.查看建筑所有者
{
    public class Main : RocketPlugin<Config>
    {
        public List<CSteamID> effectList = new List<CSteamID> { };
        public static Main instance;
        private DateTime time = new DateTime();
        private Config config;
        protected override void Load()
        {

            Logger.LogWarning($"欢迎使用 {Assembly.GetName().Name} 版本：{Assembly.GetName().Version}");
            Logger.LogWarning("Matt插件群：656525189");

            instance = this;
            config = Configuration.Instance;
            time = DateTime.Now;
            U.Events.OnPlayerConnected += 玩家进服;
            U.Events.OnPlayerDisconnected += 玩家退服;
            base.Load();
        }

        private void 玩家退服(UnturnedPlayer player)
        {
            EffectManager.askEffectClearByID(config.effectId, player.CSteamID);
        }

        private void 玩家进服(UnturnedPlayer player)
        {
            EffectManager.sendUIEffect(config.effectId, (short)config.effectId, player.CSteamID, false);
        }

        private void FixedUpdate()
        {
            try
            {
                TimeSpan span = DateTime.Now - time;
                if (span.TotalSeconds >= 1)
                {
                    time = DateTime.Now;
                    foreach(var v in effectList)
                    {
                        UnturnedPlayer player = UnturnedPlayer.FromCSteamID(v);
                        getAndUpdateUiInfo(player);
                    }
                }
            }
            catch { }
        }

        private async void getAndUpdateUiInfo(UnturnedPlayer player)
        {
            try
            {
                var objTransform = RayCastUtil.GetComponent(player, config.distance);
                var ownerInfo = await objTransform.CheckOwner();
                if (ownerInfo != null)
                {
                    EffectManager.sendUIEffectVisibility((short)config.effectId, player.CSteamID, false, "建筑信息UI", true);
                    EffectManager.sendUIEffectText((short)config.effectId, player.CSteamID, false, "建筑信息_玩家名称", ownerInfo.OwnerName);
                    EffectManager.sendUIEffectText((short)config.effectId, player.CSteamID, false, "建筑信息_玩家17Id", ownerInfo.Owner.ToString());
                    EffectManager.sendUIEffectText((short)config.effectId, player.CSteamID, false, "建筑信息_目标名称", Assets.find(EAssetType.ITEM, ownerInfo.Id).name);
                    EffectManager.sendUIEffectText((short)config.effectId, player.CSteamID, false, "建筑信息_目标ID", ownerInfo.Id.ToString());
                }
                else
                {
                    EffectManager.sendUIEffectVisibility((short)config.effectId, player.CSteamID, false, "建筑信息UI", false);
                }
            }
            catch{  }
        }


        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= 玩家进服;
            U.Events.OnPlayerDisconnected -= 玩家退服;
            base.Unload();
        }
    }


    public class Config : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            effectId = 2007;
            distance = 5f;
        }
        [XmlElement("UI_ID")]
        public ushort effectId;
        [XmlElement("检测距离")]
        public float distance;
    }
}
