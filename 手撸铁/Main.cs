
using Rocket.API;
using Rocket.Core.Plugins;
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
using UnityEngine.Rendering;
using Logger = Rocket.Core.Logging.Logger;

namespace 手撸铁
{
    public class Main : RocketPlugin<Config>
    {
        public static Main instance;
        internal List<CSteamID> elist = new List<CSteamID> { };//开启效果的玩家
    
        protected override void Load()
        {
            Logger.LogWarning($"欢迎使用 {Assembly.GetName().Name} 版本：{Assembly.GetName().Version}");
            Logger.LogWarning("Matt插件群：656525189");

            instance = this;
            UnturnedPlayerEvents.OnPlayerUpdateGesture += 玩家手势变化;
            BarricadeManager.onDamageBarricadeRequested += 放置物受伤;
            StructureManager.onDamageStructureRequested += 建筑物受伤;
            base.Load();
        }

        private void 建筑物受伤(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (elist.Contains(instigatorSteamID))
            {
                pendingTotalDamage = (ushort)(Configuration.Instance.damage * 10f);
                shouldAllow = true;
            }
        }

        private void 放置物受伤(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (elist.Contains(instigatorSteamID))
            {
                pendingTotalDamage = (ushort)(Configuration.Instance.damage * 10f);
                shouldAllow = true;
            }
        }

        private void 玩家手势变化(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            if (elist.Contains(player.CSteamID))
            {
                if (gesture == UnturnedPlayerEvents.PlayerGesture.PunchLeft || gesture == UnturnedPlayerEvents.PlayerGesture.PunchRight)
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(player.Player.look.GetEyesPositionWithoutLeaning(), player.Player.look.aim.forward, out hit, 2, RayMasks.BARRICADE))
                        if (!Physics.Raycast(player.Player.look.GetEyesPositionWithoutLeaning(), player.Player.look.aim.forward, out hit, 2, RayMasks.STRUCTURE))
                            return;
                    RaycastInfo info = new RaycastInfo(hit);
                    if (info.transform != null)
                    {
                        try
                        {
                            BarricadeManager.damage(info.transform, Configuration.Instance.damage, 10f, false);
                        }
                        catch { }
                        try
                        {
                            StructureManager.damage(info.transform, info.direction, Configuration.Instance.damage, 10f, false);
                        }
                        catch { }
                    }
                }
            }
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerUpdateGesture -= 玩家手势变化;
            BarricadeManager.onDamageBarricadeRequested -= 放置物受伤;
            StructureManager.onDamageStructureRequested -= 建筑物受伤;
            base.Unload();
        }
    }

    public class Config : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            damage = 100000000f;
        }
        [XmlElement("一拳造成的伤害数值")]
        public float damage;
    }

    public class CommandEffect : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "di";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { };

        public List<string> Permissions => new List<string> { };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = caller as UnturnedPlayer;
            if (!Main.instance.elist.Contains(player.CSteamID))
            {
                Main.instance.elist.Add(player.CSteamID);
                UnturnedChat.Say(player.CSteamID, "成功开启了手撸铁特效", Color.yellow);
            }
            else
            {
                Main.instance.elist.Remove(player.CSteamID);
                UnturnedChat.Say(player.CSteamID, "成功关闭了手撸铁特效", Color.red);
            }
        }
    }
}
