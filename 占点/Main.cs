using Rocket.Core.Plugins;
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
using Logger = Rocket.Core.Logging.Logger;
using UnityEngine;
using System.ComponentModel;

namespace 占点
{
    public class Main : RocketPlugin<Config>
    {
        private Config config;
        private dian dian = null;//当前活动的据点信息
        private DateTime time;//占点开始的时间
        private List<CSteamID> dianPlayers = null;//当前在点内的玩家id
        private List<CSteamID> playersOnline = new List<CSteamID> { };//当前在线玩家
        private bool cState = false;

        private DateTime st = new DateTime();
        protected override void Load()
        {
         
            Logger.LogWarning($"欢迎使用 {Assembly.GetName().Name} 版本：{Assembly.GetName().Version}");
            Logger.LogWarning("Matt插件群：656525189");

            config = Configuration.Instance;
            dian = null;
            st = DateTime.Now;
            time = DateTime.Now;
            UnturnedPlayerEvents.OnPlayerUpdatePosition += 玩家坐标变化;
            U.Events.OnPlayerConnected += 玩家进服;
            U.Events.OnPlayerDisconnected += 玩家退服;
            base.Load();
        }

        private void updateUiInfo(UnturnedPlayer player)
        {
            try
            {
                string state = "占点尚未开始！";
                string tt = "尚未开始";
                string jj = $"占点开始还差 {config.startNeedPlayerNum - playersOnline.Count} 人";
                if (dian != null)
                {
                    TimeSpan span = DateTime.Now - time;
                    int sec = (int)Math.Round(config.countDownTime - span.TotalSeconds);
                    double distance = 0;
                    state = "当前不在点内";
                    if (dianPlayers != null)
                        if (dianPlayers.Contains(player.CSteamID)) state = "已在点内!";
                    distance = Math.Round((player.Position - new Vector3(dian.position.x, dian.position.y, dian.position.z)).magnitude);
                    if (distance <= dian.range)
                        distance = 0;
                    tt = sec.ToString();
                    jj = $"距离[{dian.name}]还有{distance.ToString()}m";
                }
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "占点_距离信息_txt", jj);
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "占点_检测_txt", state);
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "占点_时间_txt", tt);
            }
            catch (Exception ex) { Logger.LogException(ex); }
        }
        private void 玩家进服(UnturnedPlayer player)
        {
            playersOnline.Add(player.CSteamID);
            EffectManager.sendUIEffect(Configuration.Instance.effectId, (short)Configuration.Instance.effectId, player.CSteamID, false);
            if (dian == null)
            {
                if (playersOnline.Count >= config.startNeedPlayerNum)
                    setRandomNextDian();
            }
            updateUiInfo(player);
        }

        private void 玩家退服(UnturnedPlayer player)
        {
            playersOnline.Remove(player.CSteamID);
            EffectManager.askEffectClearByID(Configuration.Instance.effectId, player.CSteamID);
            if (dianPlayers != null)
                if (dianPlayers.Contains(player.CSteamID)) dianPlayers.Remove(player.CSteamID);
        }

        private void 玩家坐标变化(UnturnedPlayer player, Vector3 position)
        {
            if (dian != null)
            {
                if (dianPlayers == null)
                    dianPlayers = new List<CSteamID> { };
                float distance = (player.Position - new Vector3(dian.position.x, dian.position.y, dian.position.z)).magnitude;
                if (distance <= dian.range)
                {
                    if (dianPlayers != null)
                        if (!dianPlayers.Contains(player.CSteamID)) dianPlayers.Add(player.CSteamID);
                }
                else if (dianPlayers != null)
                    if (dianPlayers.Contains(player.CSteamID))
                        dianPlayers.Remove(player.CSteamID);
                TimeSpan span = DateTime.Now - st;
                if (span.Seconds >= 1)
                {
                    updateUiInfo(player);
                }
            }
            else if (dianPlayers != null)
                dianPlayers = null;
        }

        private void FixedUpdate()
        {
            if (dian != null)
            {
                TimeSpan span = DateTime.Now - st;
                if (span.Seconds >= 1)
                {
                    st = DateTime.Now;
                    TimeSpan sp = DateTime.Now - time;
                    int sec = (int)Math.Round(config.countDownTime - sp.TotalSeconds);
                    if (sec > 0)
                    {
                        if (playersOnline != new List<CSteamID> { })
                        {
                            lock (playersOnline)
                            {
                                if (playersOnline.Count != 0)
                                {
                                    foreach (var v in playersOnline)
                                    {
                                        var info = UnturnedPlayer.FromCSteamID(v);
                                        updateUiInfo(info);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!cState)
                        {
                            cState = true;
                            if (dianPlayers != null)
                            {
                                List<CSteamID> notHaveGroup = new List<CSteamID> { };
                                bool iscontinue = true;
                                #region 检测是否有不同组队的人点内
                                if (dianPlayers.Count > 1)
                                {
                                    lock (dianPlayers)
                                    {
                                        foreach (var v in dianPlayers)
                                        {
                                            if (!iscontinue) break;
                                            var info = UnturnedPlayer.FromCSteamID(v);
                                            try
                                            {
                                                if (info.Player.quests.groupID == null || info.Player.quests.groupID == new CSteamID())
                                                {
                                                    if (!notHaveGroup.Contains(info.CSteamID)) notHaveGroup.Add(info.CSteamID);
                                                    if (notHaveGroup.Count > 1)
                                                    {
                                                        iscontinue = false;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (notHaveGroup.Count != 0 && !notHaveGroup.Contains(info.CSteamID))
                                                    {
                                                        iscontinue = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                if (!notHaveGroup.Contains(info.CSteamID)) notHaveGroup.Add(info.CSteamID);
                                                if (notHaveGroup.Count > 1)
                                                {
                                                    iscontinue = false;
                                                    break;
                                                }
                                            }

                                            foreach (var k in dianPlayers)
                                            {
                                                if (v != k)
                                                {
                                                    var fo = UnturnedPlayer.FromCSteamID(k);
                                                    try
                                                    {
                                                        if (fo.Player.quests.groupID == null || fo.Player.quests.groupID == new CSteamID())
                                                        {
                                                            if (!notHaveGroup.Contains(fo.CSteamID)) notHaveGroup.Add(fo.CSteamID);
                                                            if (notHaveGroup.Count > 1)
                                                            {
                                                                iscontinue = false;
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (notHaveGroup.Count != 0 && !notHaveGroup.Contains(fo.CSteamID))
                                                            {
                                                                iscontinue = false;
                                                                break;
                                                            }
                                                            try
                                                            {
                                                                if (!fo.Player.quests.isMemberOfSameGroupAs(info.Player))
                                                                {
                                                                    iscontinue = false;
                                                                    break;
                                                                }
                                                            }
                                                            catch
                                                            {
                                                                iscontinue = false;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        iscontinue = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion

                                if (!iscontinue)//不同组队的人在点内,则延长时间
                                {
                                    time = time.AddSeconds(config.addCountDownTime);
                                    cState = false;
                                    UnturnedChat.Say($"据点内存在不同组队的玩家！为了决一胜负，已延长 {config.addCountDownTime} 秒", Color.yellow);
                                }
                                else
                                {
                                    if (dianPlayers.Count != 0)
                                    {
                                        lock (dianPlayers)
                                        {
                                            UnturnedChat.Say($"此轮占点结束,正在分发占点奖励！", Color.cyan);
                                            foreach (var v in dianPlayers)
                                            {
                                                rewardPlayer(UnturnedPlayer.FromCSteamID(v));
                                            }
                                            UnturnedPlayer player = UnturnedPlayer.FromCSteamID(dianPlayers[0]);
                                            try
                                            {
                                                GroupInfo info = GroupManager.getGroupInfo(player.Player.quests.groupID);
                                                UnturnedChat.Say($"{player.DisplayName}的团队:{info.name}  [共{dianPlayers.Count}人] 霸占了该点的奖励物资！", Color.cyan);
                                            }
                                            catch
                                            {
                                                UnturnedChat.Say($"{player.DisplayName} 霸占了该点的奖励物资！", Color.cyan);
                                            }
                                        }
                                    }
                                    else UnturnedChat.Say($"由于点内无玩家，此轮占点结束！正在清理！", Color.cyan);
                                    setRandomNextDian();
                                }
                            }
                            else
                            {
                                UnturnedChat.Say($"由于点内无玩家，此轮占点结束！正在清理！", Color.cyan);
                                setRandomNextDian();
                            }
                        }
                    }
                }
            }
            else
            {
                TimeSpan span = DateTime.Now - st;
                if (span.Seconds >= 1)
                {
                    st = DateTime.Now;
                    cState = false;
                    if (playersOnline != new List<CSteamID> { })
                    {
                        if (playersOnline.Count != 0)
                        {
                            foreach (var v in playersOnline)
                            {
                                var info = UnturnedPlayer.FromCSteamID(v);
                                updateUiInfo(info);
                            }
                        }
                    }
                }
            }
        }
        private void setRandomNextDian()
        {
            if (playersOnline.Count >= config.startNeedPlayerNum)
            {
                time = DateTime.Now;
                cState = false;
                if (dianPlayers != null) dianPlayers.Clear();
                int index = UnityEngine.Random.Range(0, config.pointsList.Count);
                if (index == config.pointsList.Count) index = config.pointsList.Count - 1;
                dian = config.pointsList[index];
                UnturnedChat.Say($"新一轮占点活动开始！名称:{dian.name}  奖品种类：{dian.itemsConfig.Count}  抽取颁发次数：{dian.count}", Color.cyan);
            }else
            {
                time = DateTime.Now;
                dianPlayers = null;
                dian = null;
                cState = false;
                foreach (var v in playersOnline)
                {
                    var info = UnturnedPlayer.FromCSteamID(v);
                    updateUiInfo(info);
                }
                UnturnedChat.Say($"由于玩家数量过少，占点活动结束！", Color.red);
            }
        }

        private void rewardPlayer(UnturnedPlayer player)
        {
            string msg = "占点奖励\r\n";
            try
            {
                decimal money = (decimal)UnityEngine.Random.Range(config.min_uconomy, config.max_uconomy);
                fr34kyn01535.Uconomy.Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), money);
                msg += $"{fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName}:{money}\r\n";
                UnturnedChat.Say(player.CSteamID, msg, Color.yellow);
            }
            catch { }
            for (int i = 0; i < dian.count; i++)
            {
                int index = UnityEngine.Random.Range(0, dian.itemsConfig.Count - 1);
                ZhandianItems it = dian.itemsConfig[index];
                player.GiveItem(it.id, it.数量);
                UnturnedChat.Say(player.CSteamID, $"No.{i + 1} ID:{it.id} 数量:{it.数量}", Color.yellow);
            }

        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= 玩家坐标变化;
            U.Events.OnPlayerConnected -= 玩家进服;
            U.Events.OnPlayerDisconnected -= 玩家退服;
            base.Unload();
        }
    }
}
