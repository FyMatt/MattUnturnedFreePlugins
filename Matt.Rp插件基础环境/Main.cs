using Matt.Rp插件基础环境.LoaderConfig;
using FreeSql.DataAnnotations;
using Rocket.Core.Plugins;
using System;
using Matt.Rp插件基础环境.Structs;
using Rocket.Unturned;
using FreeSql;
using SDG.Unturned;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using System.Linq;
using Steamworks;
using Matt.Rp插件基础环境.Methods;
using UnityEngine;
using Matt.Rp插件基础环境.Events;
using Rocket.Unturned.Chat;
using Rocket.API.Collections;
using Logger = Rocket.Core.Logging.Logger;
using System.IO;
using Rocket.Core;
using Rocket.Unturned.Permissions;
using System.Xml.Linq;
using System.Numerics;
using System.Reflection;

namespace Matt.Rp插件基础环境
{
    public class Main : RocketPlugin<Config>
    {
        public static Main instance;
        public static IFreeSql sql;
        private Config config;
        /// <summary>
        /// 邮箱服务
        /// </summary>
        private EmailStampServer email;
        /// <summary>
        /// 在线玩家信息
        /// </summary>
        private static List<NsRpplayerInfo> _players;
        /// <summary>
        /// 玩家输入的验证码
        /// </summary>
        private Dictionary<string, string> _playerAdmittion = new Dictionary<string, string> { };
        /// <summary>
        /// 发送的验证码
        /// </summary>
        private Dictionary<string, string> _playerCaptcha = new Dictionary<string, string> { };
        public static List<NsRpplayerInfo> playersInfos => _players;
        public static NsRpplayerInfo GetNsRpplayerInfo(string id) => _players.Where((e) => e.steamId == id).FirstOrDefault();
        public static void sendUINotice(CSteamID player, string title, string msg)
        {
            EffectManager.sendUIEffect(Main.instance.config.NoticeEffectId, (short)Main.instance.config.NoticeEffectId, player, false);
            EffectManager.sendUIEffectText((short)Main.instance.config.NoticeEffectId, player, false, "通知ui内容", title);
            EffectManager.sendUIEffectText((short)Main.instance.config.NoticeEffectId, player, false, "通知内容_txt", msg);
        }

     
        protected override void Load()
        {
            Logger.LogWarning($"欢迎使用 {Assembly.GetName().Name} 版本：{Assembly.GetName().Version}");
            Logger.LogWarning("Matt插件群：656525189");

            try
            {
                var ad = new Uri(Assembly.CodeBase).LocalPath;
                if (!System.IO.Directory.Exists(Path.Combine(ad.Remove(ad.Length - 4, 4), "RpPlugins")))
                {
                    System.IO.Directory.CreateDirectory(Path.Combine(ad.Remove(ad.Length - 4, 4), "RpPlugins"));
                }
                config = Configuration.Instance;
                instance = this;
                sql = new FreeSql.FreeSqlBuilder()
        .UseConnectionString(FreeSql.DataType.MySql, config.getMySQLConnectionString)
        .UseAutoSyncStructure(true)
        .Build();
                _players = new List<NsRpplayerInfo> { };
                sql.Select<NsRpplayerInfo>();//检查表结构  不存在则创建

                email = new EmailStampServer(
                    config.emailAddress,
                    config.emailPassword,
                    config.stmpIp,
                    config.sendAddress,
                    config.sendName
                    );

            //事件订阅
            U.Events.OnBeforePlayerConnected += 玩家进服;
            U.Events.OnPlayerDisconnected += 玩家退服;
            EffectManager.onEffectTextCommitted += ui文本提交;
            EffectManager.onEffectButtonClicked += ui按钮点击;
            UnturnedPermissions.OnJoinRequested += 效验请求;
            base.Load();
            }
            catch (Exception ex) { Logger.LogException(ex); }
        }

        private void 效验请求(CSteamID player, ref ESteamRejection? rejectionReason)
        {
            try
            {
                var info = sql
                    .Select<NsRpplayerInfo>()
                    .Where((e) => e.steamId == player.ToString())
                    .First();
                if (!string.IsNullOrWhiteSpace(info.displayName))
                {
                    try
                    {
                        SteamPending steamPending = Provider.pending.FirstOrDefault((SteamPending x) => x.playerID.steamID == player);
                        steamPending.playerID.characterName = info.displayName;
                        steamPending.playerID.nickName = info.displayName;
                    }
                    catch { return; }
                }
            }
            catch { return; }
        }

        private void 玩家退服(UnturnedPlayer player)
        {
            if (_players
                .Where((e) => e.steamId == player.CSteamID.ToString())
                .Any())
            {
                var info = _players
                    .Where((e) => e.steamId == player.CSteamID.ToString())
                    .FirstOrDefault();
                if (info != new NsRpplayerInfo() && info != null)
                    _players.Remove(info);
            }
        }

        public override TranslationList DefaultTranslations => new TranslationList
        {
            {"点击开始游戏","欢迎你, {0} 你的身份为：{1}" },
            {"RP用户注册邮箱标题" ,"NsRP系统用户注册"},
            {"RP用户注册邮箱格式","您正在注册NS技术的RP插件用户，您的验证码：{0}" }
        };

        private void ui按钮点击(Player player, string buttonName)
        {
            string id = player.channel.owner.playerID.steamID.ToString();
            if (_players
                .Where((e) => e.steamId == id)
                .Any())
            {
                var infoIndex = _players
                    .FindIndex((e) => e.steamId == id);
                var info = _players[infoIndex];
                switch (buttonName)
                {
                    case "发送验证码_bt":
                        if (!string.IsNullOrWhiteSpace(info.qq) && !string.IsNullOrWhiteSpace(info.emailAddress)&&!string.IsNullOrWhiteSpace(info.displayName))
                        {
                            EffectManager.sendUIEffectVisibility((short)config.MaineffectId, player.channel.owner.playerID.steamID, false, "提示界面", false);
                            if (!sendCaptcha(info)) EffectManager.sendUIEffectVisibility((short)config.MaineffectId, player.channel.owner.playerID.steamID, false, "提示界面", true);
                        }
                        else EffectManager.sendUIEffectVisibility((short)config.MaineffectId, player.channel.owner.playerID.steamID, false, "提示界面", true);
                        break;
                    case "登录注册_bt":
                        if (_playerAdmittion.ContainsKey(id) && _playerCaptcha.ContainsKey(id))
                        {
                            if (_playerAdmittion[id] == _playerCaptcha[id])
                            {
                                EffectManager.sendUIEffectVisibility((short)config.MaineffectId, player.channel.owner.playerID.steamID, false, "注册界面", false);
                            }
                            else
                            {
                                EffectManager.sendUIEffectVisibility((short)config.MaineffectId, player.channel.owner.playerID.steamID, false, "提示界面", true);
                                return;
                            }
                            info.createTime = DateTime.Now;
                            _players[infoIndex] = info;
                            sql.Insert(info).ExecuteAffrows();
                            if (MattRPEvents.onPlayerRegistered != null) MattRPEvents.onPlayerRegistered(UnturnedPlayer.FromPlayer(player));
                        }
                        else EffectManager.sendUIEffectVisibility((short)config.MaineffectId, player.channel.owner.playerID.steamID, false, "提示界面", true);
                        break;
                    case "开始游戏_bt":
                        UnturnedPlayer p = UnturnedPlayer.FromPlayer(player);
                        玩家界面隐藏操作(p, false);
                        UnturnedChat.Say(p, Translate("点击开始游戏", new object[] { info.displayName, info.career.ToString() }), Color.yellow);
                        if (MattRPEvents.onPlayerStartPlaying != null) MattRPEvents.onPlayerStartPlaying(UnturnedPlayer.FromPlayer(player));
                        break;
                }
            }
        }

        private bool sendCaptcha(NsRpplayerInfo info)
        {
            try
            {
                string captcha = string.Empty;
                for (int i = 0; i < 6; i++)
                {
                    captcha += UnityEngine.Random.Range(0, 9);
                }
                if (!_playerCaptcha.ContainsKey(info.steamId.ToString())) _playerCaptcha.Add(info.steamId.ToString(), captcha);
                _playerCaptcha[info.steamId.ToString()] = captcha;
                email.sendTo(info.emailAddress, Translate("RP用户注册邮箱标题"), Translate("RP用户注册邮箱格式", captcha));
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void ui文本提交(Player player, string buttonName, string text)
        {
            string id = player.channel.owner.playerID.steamID.ToString();
            if (_players
                .Where((e) =>e.steamId == id)
                .Any())
            {
                var infoIndex = _players
                    .FindIndex((e) =>e.steamId == id);
                var info = _players[infoIndex];
                switch (buttonName)
                {
                    case "QQ号_txt":
                        if (info.qq != text) 
                        { 
                            info.qq = text;
                            _players[infoIndex] = info;
                            修改文本内容后删除验证码(id);
                        }
                        break;
                    case "邮箱_txt":
                        if (info.emailAddress != text)
                        {
                            info.emailAddress = text;
                            _players[infoIndex] = info;
                            修改文本内容后删除验证码(id);
                        }
                        break;
                    case "角色名称_txt":
                        if (info.displayName != text)
                        {
                            info.displayName = text;
                            _players[infoIndex] = info;
                            修改文本内容后删除验证码(id);
                        }
                        break;
                    case "验证码_txt":
                        if (!_playerAdmittion.ContainsKey(id)) _playerAdmittion.Add(id, text);
                        _playerAdmittion[id] = text;
                        _players[infoIndex] = info;
                        break;
                }
            }
        }
        private void 修改文本内容后删除验证码(string id)
        {
            if (_playerAdmittion.ContainsKey(id)) _playerAdmittion.Remove(id);
            if (_playerCaptcha.ContainsKey(id)) _playerCaptcha.Remove(id);
        }
        private void 玩家界面隐藏操作(UnturnedPlayer player,bool 是否隐藏)
        {
            if (是否隐藏)
            {
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.Default);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowVirus);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowWater);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowOxygen);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowHealth);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowStamina);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowFood);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowUseableGunStatus);
            }
            else
            {
                player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.Default);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowVirus);
                player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowWater);
                player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy);
                player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowOxygen);
                player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowHealth);
                player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowStamina);
                player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowFood);
                player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons);
                player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowUseableGunStatus);
            }
            player.GodMode = 是否隐藏;
            player.VanishMode = 是否隐藏;
        }

        private void 玩家进服(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            EffectManager.sendUIEffect(config.MaineffectId, (short)config.MaineffectId, player.CSteamID, false);
            EffectManager.sendUIEffectImageURL((short)config.MaineffectId, player.CSteamID, false, "游戏界面", config.staringGameback);
            EffectManager.sendUIEffectImageURL((short)config.MaineffectId, player.CSteamID, false, "注册界面", config.registeGameback);
            玩家界面隐藏操作(player, true);
            if (sql
                .Select<NsRpplayerInfo>()
                .Any((e) => e.steamId == player.CSteamID.ToString()))
            {//已经注册则操作
                var info = sql
                    .Select<NsRpplayerInfo>()
                    .Where((e) => e.steamId == player.CSteamID.ToString())
                    .ToOne();
                EffectManager.sendUIEffectVisibility((short)config.MaineffectId, player.CSteamID, false, "注册界面", false);
                _players.Add(
                    new NsRpplayerInfo
                    {
                        career = info.career,
                        steamId = info.steamId,
                        createTime = info.createTime,
                        displayName = info.displayName,
                        emailAddress = info.emailAddress,
                        qq = info.qq
                    });

                if (!string.IsNullOrWhiteSpace(info.displayName) && info.displayName != player.DisplayName) 
                {
                    try
                    {
                        SteamPending steamPending = Provider.pending.FirstOrDefault((SteamPending x) => x.playerID.steamID == player.CSteamID);
                        steamPending.playerID.characterName = info.displayName;
                        steamPending.playerID.nickName = info.displayName;
                        player.Player.channel.owner.playerID.characterName = info.displayName;
                        player.Player.channel.owner.playerID.nickName = info.displayName;
                        Provider.updateRichPresence();
                    }
                    catch { }
                }
            }
            else
            {//未注册则操作
                EffectManager.sendUIEffectVisibility((short)config.MaineffectId, player.CSteamID, false, "注册界面", true);
                var info = new NsRpplayerInfo 
                { 
                    steamId = player.CSteamID.ToString(),
                    career = Types.ECareer.平民
                };
                _players.Add(info);
            }
        }
        protected override void Unload()
        {
            //取消订阅
            U.Events.OnBeforePlayerConnected -= 玩家进服;
            U.Events.OnPlayerDisconnected -= 玩家退服;
            EffectManager.onEffectTextCommitted -= ui文本提交;
            EffectManager.onEffectButtonClicked -= ui按钮点击;
            UnturnedPermissions.OnJoinRequested -= 效验请求;
            base.Unload();
        }
    }
}
