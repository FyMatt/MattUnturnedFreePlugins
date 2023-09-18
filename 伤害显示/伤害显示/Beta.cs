
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using static SDG.Unturned.WeatherAsset;

namespace 伤害显示
{


    public class Beta:RocketPlugin<Config>
    {
        public static Beta instance;
        List<CSteamID> onlinePlayers = new List<CSteamID> { };

        List<short> key = new List<short> { };
        Dictionary<CSteamID, int> index = new Dictionary<CSteamID, int> { };
        protected override void Load()
        {
            Logger.LogWarning($"欢迎使用 {Assembly.GetName().Name} 版本：{Assembly.GetName().Version}");
            Logger.LogWarning("Matt插件群：656525189");

            instance = this;
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDeath += UnturnedPlayerEvents_OnPlayerDeath;
            DamageTool.damageAnimalRequested += DamageTool_damageAnimalRequested;
            DamageTool.damageZombieRequested += DamageTool_damageZombieRequested;
            DamageTool.damagePlayerRequested += DamageTool_damagePlayerRequested;

            int b = 10000;
            for(int i = 0; i < 50; i++)
            {
                key.Add(Convert.ToInt16(b + i));
            }




            base.Load();
        }

        private void UnturnedPlayerEvents_OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            sendPlayerDamageUI(player, false);
        }

        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            onlinePlayers.Remove(player.CSteamID);
            index.Remove(player.CSteamID);
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            onlinePlayers.Add(player.CSteamID);
            index.Add(player.CSteamID, 0);
        }

        /// <summary>
        /// 发送玩家伤害UI
        /// </summary>
        /// <param name="player">玩家信息</param>
        /// <param name="enable">是否可见</param>
        /// <param name="limb">击中部位</param>
        /// <param name="damage">攻击的伤害</param>
        public void sendPlayerDamageUI(UnturnedPlayer player, bool enable, ELimb limb = ELimb.SPINE, float damage = 0,string rpgColor="")
        {
            if (enable)
            {
                string msg = damage.ToString();
                if (rpgColor.Length != 0) msg = $"<color={rpgColor}>{damage}</color>";
                EffectManager.sendUIEffect(Configuration.Instance.effectId, key[index[player.CSteamID]], player.CSteamID, false);
                if (damage >= 90 && limb == ELimb.SKULL)
                {
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "big_red_text", true);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "sma_red_text", false);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "sma_white_text", false);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "big_white_text", false);
                    EffectManager.sendUIEffectText(key[index[player.CSteamID]], player.CSteamID, false, "big_red_text", $"{msg}");
                }
                else if (damage < 90 && limb == ELimb.SKULL)
                {
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "sma_red_text", true);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "big_red_text", false);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "sma_white_text", false);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "big_white_text", false);
                    EffectManager.sendUIEffectText(key[index[player.CSteamID]], player.CSteamID, false, "sma_red_text", $"{msg}");
                }
                else if (limb != ELimb.SKULL && damage < 50)
                {
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "sma_red_text", false);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "big_red_text", false);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "sma_white_text", true);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "big_white_text", false);
                    EffectManager.sendUIEffectText(key[index[player.CSteamID]], player.CSteamID, false, "sma_white_text", $"{msg}");
                }
                else if (limb != ELimb.SKULL && damage >= 50)
                {
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "sma_red_text", false);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "big_red_text", false);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "sma_white_text", false);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "big_white_text", true);
                    EffectManager.sendUIEffectText(key[index[player.CSteamID]], player.CSteamID, false, "big_white_text", $"{msg}");
                }
                else
                {
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "sma_red_text", false);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "big_red_text", false);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "sma_white_text", false);
                    EffectManager.sendUIEffectVisibility(key[index[player.CSteamID]], player.CSteamID, false, "big_white_text", false);
                }
                index[player.CSteamID]++;
                if (index[player.CSteamID] >= 50)
                {
                    index[player.CSteamID] = 0;
                    sendPlayerDamageUI(player, false);
                }
            }
            else
            {
                EffectManager.askEffectClearByID(Configuration.Instance.effectId, player.CSteamID);
            }
        }
        private void DamageTool_damageAnimalRequested(ref DamageAnimalParameters parameters, ref bool shouldAllow)
        {
            if (parameters.instigator is Player player)
            {
                if (onlinePlayers.Contains(player.channel.owner.playerID.steamID))
                {
                    float num = Mathf.Floor(parameters.damage * parameters.times);
                    float damage = Mathf.Min(float.MaxValue, num);
                    ELimb limb = parameters.limb;
                    sendPlayerDamageUI(UnturnedPlayer.FromPlayer(player), true, limb, damage);
                }
            }
        }

        private void DamageTool_damagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (onlinePlayers.Contains(parameters.killer))
            {
                float num = Mathf.Floor(parameters.damage * parameters.times);
                float damage = Mathf.Min(float.MaxValue, num);
                ELimb limb = parameters.limb;
                sendPlayerDamageUI(UnturnedPlayer.FromCSteamID(parameters.killer), true, limb, damage);
            }
        }

        private void DamageTool_damageZombieRequested(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            if (parameters.instigator is Player player)
            {
                if (onlinePlayers.Contains(player.channel.owner.playerID.steamID))
                {
                    float num = Mathf.Floor(parameters.damage * parameters.times);
                    float damage = Mathf.Min(float.MaxValue, num);
                    ELimb limb = parameters.limb;
                    sendPlayerDamageUI(UnturnedPlayer.FromPlayer(player), true, limb, damage);
                }
            }
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDeath -= UnturnedPlayerEvents_OnPlayerDeath;
            DamageTool.damageAnimalRequested -= DamageTool_damageAnimalRequested;
            DamageTool.damageZombieRequested -= DamageTool_damageZombieRequested;
            DamageTool.damagePlayerRequested -= DamageTool_damagePlayerRequested;
            base.Unload();
        }
    }
    public class Config : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            effectId = 2013;
            t = 5;
        }
        [XmlElement("特效ID")]
        public ushort effectId;
        [XmlElement("特效暂留时长_秒")]
        public int t;
    }
}
