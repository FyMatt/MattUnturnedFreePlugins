
using MySql.Data.MySqlClient;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace 经济商店UI
{
    public class Beta : RocketPlugin<Config>
    {
        public static Beta Instance;
        public MysqlManager manager;
        public Dictionary<CSteamID, bool> uistate;//玩家键盘按键打开UI状态监测
        public Dictionary<CSteamID, int> uipage;//玩家商店ui显示的页数
        public Dictionary<CSteamID, List<shopItemInfo>> shops { get; set; }//获取最新商店信息
        protected override void Load()
        {
            Logger.LogWarning($"欢迎使用 {Assembly.GetName().Name} 版本：{Assembly.GetName().Version}");
            Logger.LogWarning("Matt插件群：656525189");
            

            Instance = this;
            manager = new MysqlManager();
            uistate = new Dictionary<CSteamID, bool> { };
            uipage = new Dictionary<CSteamID, int> { };
            shops = new Dictionary<CSteamID, List<shopItemInfo>> { };
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            PlayerInput.onPluginKeyTick += onPluginKeyTick;
            EffectManager.onEffectButtonClicked += onEffectButtonClicked;
            EffectManager.onEffectTextCommitted += onEffectTextCommitted;
            base.Load();
        }
        private void onEffectTextCommitted(Player player, string buttonName, string text)
        {
            UnturnedPlayer p = UnturnedPlayer.FromPlayer(player);
            if (buttonName == "Player_Search_Text")
            {
                if (text.Length != 0)
                {
                    shops[p.CSteamID] = manager.getshops(text, false);
                    updateShopUI(p, shops[p.CSteamID], 1);
                }
                else
                {
                    shops[p.CSteamID] = manager.getshops("", true);
                    updateShopUI(p, shops[p.CSteamID], 1);
                }
            }
            else if (buttonName == "Un_Item1_ItemNum" || buttonName == "Un_Item2_ItemNum" || buttonName == "Un_Item3_ItemNum" || buttonName == "Un_Item4_ItemNum" || buttonName == "Un_Item5_ItemNum" || buttonName == "Un_Item6_ItemNum" || buttonName == "Un_Item7_ItemNum" || buttonName == "Un_Item8_ItemNum" || buttonName == "Un_Item9_ItemNum" || buttonName == "Un_Item10_ItemNum" || buttonName == "Un_Item11_ItemNum" || buttonName == "Un_Item12_ItemNum" || buttonName == "Un_Item13_ItemNum")
            {
                byte num = 1;
                if (byte.TryParse(text, out num))
                {
                    setshopNum(p, buttonName, num);
                }
                else
                {
                    UnturnedChat.Say(p.CSteamID, "数量为整数！默认数值为1", Color.red);
                }
            }
        }
        private void onEffectButtonClicked(Player player, string buttonName)
        {
            UnturnedPlayer p = UnturnedPlayer.FromPlayer(player);
            if (buttonName == "Player_Button_Right")
            {
                if (uipage[p.CSteamID] != getpagecount(shops[p.CSteamID]))
                {
                    uipage[p.CSteamID] += 1;
                    updateShopUI(p, shops[p.CSteamID], uipage[p.CSteamID]);
                }
            }
            else if (buttonName == "Player_Button_Left")
            {
                if (uipage[p.CSteamID] != 1)
                {
                    uipage[p.CSteamID] -= 1;
                    updateShopUI(p, shops[p.CSteamID], uipage[p.CSteamID]);
                }
            }
            if (buttonName == "Un_Item1_buyButton") 
            {
                if (getshopID(p, "Un_Item1_buyButton") != 0) { buyitem(p, getshopID(p, "Un_Item1_buyButton").ToString(), getshopNum(p, "Un_Item1_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }else if (buttonName == "Un_Item2_buyButton")
            {
                if (getshopID(p, "Un_Item2_buyButton") != 0) {buyitem(p, getshopID(p, "Un_Item2_buyButton").ToString(), getshopNum(p, "Un_Item2_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item3_buyButton")
            {
                if (getshopID(p, "Un_Item3_buyButton") != 0) {buyitem(p, getshopID(p, "Un_Item3_buyButton").ToString(), getshopNum(p, "Un_Item3_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item4_buyButton")
            {
                if (getshopID(p, "Un_Item4_buyButton") != 0) { buyitem(p, getshopID(p, "Un_Item4_buyButton").ToString(), getshopNum(p, "Un_Item4_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item5_buyButton")
            {
                if (getshopID(p, "Un_Item5_buyButton") != 0) { buyitem(p, getshopID(p, "Un_Item5_buyButton").ToString(), getshopNum(p, "Un_Item5_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item6_buyButton")
            {
                if (getshopID(p, "Un_Item6_buyButton") != 0) { buyitem(p, getshopID(p, "Un_Item6_buyButton").ToString(), getshopNum(p, "Un_Item6_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item7_buyButton")
            {
                if (getshopID(p, "Un_Item7_buyButton") != 0) { buyitem(p, getshopID(p, "Un_Item7_buyButton").ToString(), getshopNum(p, "Un_Item7_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item8_buyButton")
            {
                if (getshopID(p, "Un_Item8_buyButton") != 0) { buyitem(p, getshopID(p, "Un_Item8_buyButton").ToString(), getshopNum(p, "Un_Item8_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item9_buyButton")
            {
                if (getshopID(p, "Un_Item9_buyButton") != 0) { buyitem(p, getshopID(p, "Un_Item9_buyButton").ToString(), getshopNum(p, "Un_Item9_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item10_buyButton")
            {
                if (getshopID(p, "Un_Item10_buyButton") != 0) { buyitem(p, getshopID(p, "Un_Item10_buyButton").ToString(), getshopNum(p, "Un_Item10_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item11_buyButton")
            {
                if (getshopID(p, "Un_Item11_buyButton") != 0) { buyitem(p, getshopID(p, "Un_Item11_buyButton").ToString(), getshopNum(p, "Un_Item11_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item12_buyButton")
            {
                if (getshopID(p, "Un_Item12_buyButton") != 0) { buyitem(p, getshopID(p, "Un_Item12_buyButton").ToString(), getshopNum(p, "Un_Item12_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item13_buyButton")
            {
                if (getshopID(p, "Un_Item13_buyButton") != 0) { buyitem(p, getshopID(p, "Un_Item13_buyButton").ToString(), getshopNum(p, "Un_Item13_buyButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }else if (buttonName == "Un_Item1_sellButton")
            {
                if (getshopID(p, "Un_Item1_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item1_sellButton").ToString(), getshopNum(p, "Un_Item1_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item2_sellButton")
            {
                if (getshopID(p, "Un_Item2_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item2_sellButton").ToString(), getshopNum(p, "Un_Item2_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item3_sellButton")
            {
                if (getshopID(p, "Un_Item3_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item3_sellButton").ToString(), getshopNum(p, "Un_Item3_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item4_sellButton")
            {
                if (getshopID(p, "Un_Item4_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item4_sellButton").ToString(), getshopNum(p, "Un_Item4_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item5_sellButton")
            {
                if (getshopID(p, "Un_Item5_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item5_sellButton").ToString(), getshopNum(p, "Un_Item5_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item6_sellButton")
            {
                if (getshopID(p, "Un_Item6_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item6_sellButton").ToString(), getshopNum(p, "Un_Item6_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item7_sellButton")
            {
                if (getshopID(p, "Un_Item7_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item7_sellButton").ToString(), getshopNum(p, "Un_Item7_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item8_sellButton")
            {
                if (getshopID(p, "Un_Item8_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item8_sellButton").ToString(), getshopNum(p, "Un_Item8_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item9_sellButton")
            {
                if (getshopID(p, "Un_Item9_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item9_sellButton").ToString(), getshopNum(p, "Un_Item9_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item10_sellButton")
            {
                if (getshopID(p, "Un_Item10_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item10_sellButton").ToString(), getshopNum(p, "Un_Item10_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item11_sellButton")
            {
                if (getshopID(p, "Un_Item11_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item11_sellButton").ToString(), getshopNum(p, "Un_Item11_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item12_sellButton")
            {
                if (getshopID(p, "Un_Item12_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item12_sellButton").ToString(), getshopNum(p, "Un_Item12_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "Un_Item13_sellButton")
            {
                if (getshopID(p, "Un_Item13_sellButton") != 0) { sellitem(p, getshopID(p, "Un_Item13_sellButton").ToString(), getshopNum(p, "Un_Item13_sellButton")); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
        }
        private void onPluginKeyTick(Player player, uint simulation, byte key, bool state)
        {
            UnturnedPlayer p = UnturnedPlayer.FromPlayer(player);
            if (key == 0 && state == true && uistate[p.CSteamID] == false)
            {
                shops[p.CSteamID] = manager.getshops("",true);
                uistate[p.CSteamID] = true;
                uipage[p.CSteamID] = 1;
                EffectManager.sendUIEffect(Configuration.Instance.effectId, (short)Configuration.Instance.effectId, p.CSteamID, false);
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, p.CSteamID, false, "Player_Count_Text", manager.GetBalance(p.CSteamID.ToString()).ToString() + " " + fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName);
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, p.CSteamID, false, "Player_Name_Text", p.DisplayName);
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, p.CSteamID, false, "Player_Count_exp", p.Experience.ToString());
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, p.CSteamID, false, "Player_Count_em", p.Reputation.ToString());
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, p.CSteamID, false, "Player_Count_Text_name", fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName);
                updateShopUI(p,shops[p.CSteamID],1);
                p.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
            } else if (key == 0 && state == true && uistate[p.CSteamID])
            {
                uistate[p.CSteamID] = false;
                EffectManager.askEffectClearByID(Configuration.Instance.effectId, p.CSteamID);
                p.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            }
        }
        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            shops.Add(player.CSteamID,manager.getshops("",true));
            uipage.Add(player.CSteamID, 0);
            uistate.Add(player.CSteamID, false);
        }
        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            shops.Remove(player.CSteamID);
            uipage.Remove(player.CSteamID);
            uistate.Remove(player.CSteamID);
        }

        #region 方法库
        private ushort getshopID(UnturnedPlayer player, string buyButtonName)
        {
            int index = (uipage[player.CSteamID] - 1) * 13;
            if (buyButtonName == "Un_Item2_buyButton" || buyButtonName == "Un_Item2_sellButton")
            {
                index += 1;
            }
            else if (buyButtonName == "Un_Item3_buyButton" || buyButtonName == "Un_Item3_sellButton")
            {
                index += 2;
            }
            else if (buyButtonName == "Un_Item4_buyButton" || buyButtonName == "Un_Item4_sellButton")
            {
                index += 3;
            }
            else if (buyButtonName == "Un_Item5_buyButton" || buyButtonName == "Un_Item5_sellButton")
            {
                index += 4;
            }
            else if (buyButtonName == "Un_Item6_buyButton" || buyButtonName == "Un_Item6_sellButton")
            {
                index += 5;
            }
            else if (buyButtonName == "Un_Item7_buyButton" || buyButtonName == "Un_Item7_sellButton")
            {
                index += 6;
            }
            else if (buyButtonName == "Un_Item8_buyButton" || buyButtonName == "Un_Item8_sellButton")
            {
                index += 7;
            }
            else if (buyButtonName == "Un_Item9_buyButton" || buyButtonName == "Un_Item9_sellButton")
            {
                index += 8;
            }
            else if (buyButtonName == "Un_Item10_buyButton" || buyButtonName == "Un_Item10_sellButton")
            {
                index += 9;
            }
            else if (buyButtonName == "Un_Item11_buyButton" || buyButtonName == "Un_Item11_sellButton")
            {
                index += 10;
            }
            else if (buyButtonName == "Un_Item12_buyButton" || buyButtonName == "Un_Item12_sellButton")
            {
                index += 11;
            }
            else if (buyButtonName == "Un_Item13_buyButton" || buyButtonName == "Un_Item13_sellButton")
            {
                index += 12;
            }
            ushort id = 0;
            if (shops[player.CSteamID].Count > index)
            {
                id = shops[player.CSteamID][index].id;
            }
            return id;
        }//通过按下的按钮获取该商品ID
        private void setshopNum(UnturnedPlayer player, string buyButtonName,byte num)
        {
            int index = (uipage[player.CSteamID] - 1) * 13;
            if (buyButtonName == "Un_Item2_ItemNum")
            {
                index += 1;
            }
            else if (buyButtonName == "Un_Item3_ItemNum")
            {
                index += 2;
            }
            else if (buyButtonName == "Un_Item4_ItemNum")
            {
                index += 3;
            }
            else if (buyButtonName == "Un_Item5_ItemNum")
            {
                index += 4;
            }
            else if (buyButtonName == "Un_Item6_ItemNum")
            {
                index += 5;
            }
            else if (buyButtonName == "Un_Item7_ItemNum")
            {
                index += 6;
            }
            else if (buyButtonName == "Un_Item8_ItemNum")
            {
                index += 7;
            }
            else if (buyButtonName == "Un_Item9_ItemNum")
            {
                index += 8;
            }
            else if (buyButtonName == "Un_Item10_ItemNum")
            {
                index += 9;
            }
            else if (buyButtonName == "Un_Item11_ItemNum")
            {
                index += 10;
            }
            else if (buyButtonName == "Un_Item12_ItemNum")
            {
                index += 11;
            }
            else if (buyButtonName == "Un_Item13_ItemNum")
            {
                index += 12;
            }
            if (shops[player.CSteamID].Count > index)
            {
                shops[player.CSteamID][index].num = num;
            }
        }
        private byte getshopNum(UnturnedPlayer player, string buyButtonName)
        {
            int index = (uipage[player.CSteamID] - 1) * 13;
            if (buyButtonName == "Un_Item2_buyButton" || buyButtonName == "Un_Item2_sellButton")
            {
                index += 1;
            }
            else if (buyButtonName == "Un_Item3_buyButton" || buyButtonName == "Un_Item3_sellButton")
            {
                index += 2;
            }
            else if (buyButtonName == "Un_Item4_buyButton" || buyButtonName == "Un_Item4_sellButton")
            {
                index += 3;
            }
            else if (buyButtonName == "Un_Item5_buyButton" || buyButtonName == "Un_Item5_sellButton")
            {
                index += 4;
            }
            else if (buyButtonName == "Un_Item6_buyButton" || buyButtonName == "Un_Item6_sellButton")
            {
                index += 5;
            }
            else if (buyButtonName == "Un_Item7_buyButton" || buyButtonName == "Un_Item7_sellButton")
            {
                index += 6;
            }
            else if (buyButtonName == "Un_Item8_buyButton" || buyButtonName == "Un_Item8_sellButton")
            {
                index += 7;
            }
            else if (buyButtonName == "Un_Item9_buyButton" || buyButtonName == "Un_Item9_sellButton")
            {
                index += 8;
            }
            else if (buyButtonName == "Un_Item10_buyButton" || buyButtonName == "Un_Item10_sellButton")
            {
                index += 9;
            }
            else if (buyButtonName == "Un_Item11_buyButton" || buyButtonName == "Un_Item11_sellButton")
            {
                index += 10;
            }
            else if (buyButtonName == "Un_Item12_buyButton" || buyButtonName == "Un_Item12_sellButton")
            {
                index += 11;
            }
            else if (buyButtonName == "Un_Item13_buyButton" || buyButtonName == "Un_Item13_sellButton")
            {
                index += 12;
            }
            byte num = 1;
            if (shops[player.CSteamID].Count > index)
            {
                num = shops[player.CSteamID][index].num;
            }
            return num;
        }//通过按下的按钮获取该商品ID
        private int getpagecount(List<shopItemInfo> infos) 
        {
            int page = infos.Count / 13;
            page += 1;
            return page;
        }//获取匹配的所有商品集合
        private void buyitem(UnturnedPlayer player, string itemid,byte num) 
        {
            decimal allcost = manager.getitemcost(itemid) * num;
            if (manager.GetBalance(player.CSteamID.ToString()) >= allcost)
            {
                if (allcost != 0){
                    manager.IncreaseBalance(player.CSteamID.ToString(), 0 - allcost);
                    player.GiveItem(Convert.ToUInt16(itemid), num);
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Player_Count_Text", manager.GetBalance(player.CSteamID.ToString()).ToString() + " " + fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName);
                    UnturnedChat.Say(player.CSteamID, "成功消费了：" + allcost + "  ----当前剩余：" + manager.GetBalance(player.CSteamID.ToString()).ToString() + " " + fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName, Color.yellow);
                }
                else 
                {
                    UnturnedChat.Say(player.CSteamID, "抱歉,此商品禁止购买！", Color.red);
                }
            }
            else
            {
                UnturnedChat.Say(player.CSteamID, "你的" + fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName + "不足", Color.red);
                UnturnedChat.Say(player.CSteamID, "当前剩余：" + manager.GetBalance(player.CSteamID.ToString()).ToString() + " " + fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName, Color.red);
            }
        }//购买物品
        private void sellitem(UnturnedPlayer player, string itemid, byte num)
        {
            if (player.Inventory.search(Convert.ToUInt16(itemid), false, true).Count >= num)
            {
                decimal allsell = manager.getitemsell(itemid) * num;
                manager.IncreaseBalance(player.CSteamID.ToString(), allsell);
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Player_Count_Text", manager.GetBalance(player.CSteamID.ToString()).ToString() + " " + fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName);
                for (int i = 0; i < num; i++)
                {
                    var v = player.Inventory.search(Convert.ToUInt16(itemid), false, true)[i];
                    player.Inventory.removeItem(v.page, player.Inventory.getIndex(v.page, v.jar.x, v.jar.y));
                }
                UnturnedChat.Say(player.CSteamID, "成功出售：" + num + "个 " + manager.getitemname(itemid) + " .获得 " + allsell + " " + fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName, Color.yellow);
            }
            else 
            {
                UnturnedChat.Say(player.CSteamID, "抱歉！你的库存不足！", Color.red);
            }
        }//出售物品
        private void updateShopUI(UnturnedPlayer player,List<shopItemInfo> infos, int page) 
        {
            if (infos.Count > 0)
            {
                if (page == 1 && page < getpagecount(infos))
                {
                    EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "Player_Button_Left", false);
                    EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "Player_Button_Right", true);
                }
                else if (page == getpagecount(infos) && page > 1)
                {
                    EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "Player_Button_Left", true);
                    EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "Player_Button_Right", false);
                }
                else if (getpagecount(infos) == 1 && page == 1)
                {
                    EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "Player_Button_Left", false);
                    EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "Player_Button_Right", false);
                }
                else
                {
                    EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "Player_Button_Left", true);
                    EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "Player_Button_Right", true);
                }
                string pge = page + "/" + getpagecount(infos);
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Player_ItemPages_Num", pge);
                int index = (page - 1) * 13;
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item1_Count", (index + 1).ToString());
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item1_ItemID", infos[index].id.ToString());
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item1_ItemName", infos[index].name.ToString());
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item1_ItemCost", infos[index].buy.ToString());
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item1_ItemSell", infos[index].sell.ToString());
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item1_ItemNum", infos[index].num.ToString());
                index += 1;
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_Count", (index + 1).ToString());
                if (infos.Count > index)
                {
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemID", infos[index].id.ToString());
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemName", infos[index].name.ToString());
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemCost", infos[index].buy.ToString());
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemSell", infos[index].sell.ToString());
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemNum", infos[index].num.ToString());
                    index += 1;
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_Count", (index + 1).ToString());
                    if (infos.Count > index)
                    {
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemID", infos[index].id.ToString());
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemName", infos[index].name.ToString());
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemCost", infos[index].buy.ToString());
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemSell", infos[index].sell.ToString());
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemNum", infos[index].num.ToString());
                        index += 1;
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_Count", (index + 1).ToString());
                        if (infos.Count > index)
                        {
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemID", infos[index].id.ToString());
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemName", infos[index].name.ToString());
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemCost", infos[index].buy.ToString());
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemSell", infos[index].sell.ToString());
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemNum", infos[index].num.ToString());
                            index += 1;
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_Count", (index + 1).ToString());
                            if (infos.Count > index)
                            {
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemID", infos[index].id.ToString());
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemName", infos[index].name.ToString());
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemCost", infos[index].buy.ToString());
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemSell", infos[index].sell.ToString());
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemNum", infos[index].num.ToString());
                                index += 1;
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_Count", (index + 1).ToString());
                                if (infos.Count > index)
                                {
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemID", infos[index].id.ToString());
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemName", infos[index].name.ToString());
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemCost", infos[index].buy.ToString());
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemSell", infos[index].sell.ToString());
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemNum", infos[index].num.ToString());
                                    index += 1;
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_Count", (index + 1).ToString());
                                    if (infos.Count > index)
                                    {
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemID", infos[index].id.ToString());
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemName", infos[index].name.ToString());
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemCost", infos[index].buy.ToString());
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemSell", infos[index].sell.ToString());
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemNum", infos[index].num.ToString());
                                        index += 1;
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_Count", (index + 1).ToString());
                                        if (infos.Count > index)
                                        {
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemID", infos[index].id.ToString());
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemName", infos[index].name.ToString());
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemCost", infos[index].buy.ToString());
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemSell", infos[index].sell.ToString());
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemNum", infos[index].num.ToString());
                                            index += 1;
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_Count", (index + 1).ToString());
                                            if (infos.Count > index)
                                            {
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemID", infos[index].id.ToString());
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemName", infos[index].name.ToString());
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemCost", infos[index].buy.ToString());
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemSell", infos[index].sell.ToString());
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemNum", infos[index].num.ToString());
                                                index += 1;
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_Count", (index + 1).ToString());
                                                if (infos.Count > index)
                                                {
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemID", infos[index].id.ToString());
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemName", infos[index].name.ToString());
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemCost", infos[index].buy.ToString());
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemSell", infos[index].sell.ToString());
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemNum", infos[index].num.ToString());
                                                    index += 1;
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_Count", (index + 1).ToString());
                                                    if (infos.Count > index)
                                                    {
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemID", infos[index].id.ToString());
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemName", infos[index].name.ToString());
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemCost", infos[index].buy.ToString());
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemSell", infos[index].sell.ToString());
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemNum", infos[index].num.ToString());
                                                        index += 1;
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_Count", (index + 1).ToString());
                                                        if (infos.Count > index)
                                                        {
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", infos[index].id.ToString());
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", infos[index].name.ToString());
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", infos[index].buy.ToString());
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", infos[index].sell.ToString());
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemNum", infos[index].num.ToString());
                                                            index += 1;
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_Count", (index + 1).ToString());
                                                            if (infos.Count > index)
                                                            {
                                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", infos[index].id.ToString());
                                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", infos[index].name.ToString());
                                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", infos[index].buy.ToString());
                                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", infos[index].sell.ToString());
                                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", infos[index].num.ToString());
                                                            }
                                                            else
                                                            {
                                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", "暂无商品");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", "暂无商品");
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", "暂无商品");
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", "暂无商品");
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", "暂无商品");
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemNum", "暂无商品");
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", "暂无商品");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemID", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemName", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemCost", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemSell", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemNum", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemNum", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", "暂无商品");
                                                    }
                                                }
                                                else
                                                {
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemID", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemName", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemCost", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemSell", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemNum", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemID", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemName", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemCost", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemSell", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemNum", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemNum", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", "暂无商品");
                                                }
                                            }
                                            else
                                            {
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemID", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemName", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemCost", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemSell", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemNum", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemID", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemName", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemCost", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemSell", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemNum", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemID", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemName", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemCost", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemSell", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemNum", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemNum", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", "暂无商品");
                                            }
                                        }
                                        else
                                        {
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemID", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemName", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemCost", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemSell", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemNum", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemID", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemName", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemCost", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemSell", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemNum", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemID", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemName", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemCost", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemSell", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemNum", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemID", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemName", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemCost", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemSell", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemNum", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemNum", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", "暂无商品");
                                        }
                                    }
                                    else
                                    {
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemID", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemName", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemCost", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemSell", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemNum", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemID", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemName", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemCost", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemSell", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemNum", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemID", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemName", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemCost", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemSell", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemNum", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemID", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemName", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemCost", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemSell", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemNum", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemID", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemName", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemCost", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemSell", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemNum", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemNum", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", "暂无商品");
                                    }
                                }
                                else
                                {
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemID", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemName", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemCost", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemSell", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemNum", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemID", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemName", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemCost", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemSell", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemNum", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemID", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemName", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemCost", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemSell", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemNum", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemID", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemName", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemCost", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemSell", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemNum", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemID", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemName", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemCost", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemSell", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemNum", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemID", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemName", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemCost", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemSell", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemNum", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemNum", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", "暂无商品");
                                }
                            }
                            else
                            {
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemID", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemName", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemCost", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemSell", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemNum", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemID", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemName", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemCost", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemSell", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemNum", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemID", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemName", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemCost", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemSell", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemNum", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemID", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemName", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemCost", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemSell", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemNum", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemID", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemName", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemCost", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemSell", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemNum", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemID", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemName", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemCost", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemSell", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemNum", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemID", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemName", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemCost", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemSell", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemNum", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemNum", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", "暂无商品");
                            }
                        }
                        else
                        {
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemID", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemName", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemCost", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemSell", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemNum", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemID", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemName", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemCost", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemSell", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemNum", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemID", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemName", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemCost", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemSell", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemNum", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemID", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemName", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemCost", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemSell", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemNum", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemID", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemName", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemCost", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemSell", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemNum", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemID", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemName", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemCost", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemSell", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemNum", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemID", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemName", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemCost", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemSell", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemNum", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemID", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemName", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemCost", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemSell", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemNum", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemNum", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", "暂无商品");
                        }
                    }
                    else
                    {
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemID", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemName", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemCost", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemSell", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemID", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemName", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemCost", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemSell", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemID", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemName", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemCost", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemSell", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemID", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemName", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemCost", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemSell", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemID", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemName", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemCost", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemSell", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemID", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemName", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemCost", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemSell", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemID", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemName", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemCost", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemSell", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemID", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemName", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemCost", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemSell", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemID", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemName", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemCost", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemSell", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                    }
                }
                else
                {
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemID", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemName", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemCost", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemSell", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemNum", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemID", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemName", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemCost", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemSell", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemNum", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemID", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemName", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemCost", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemSell", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemNum", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemID", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemName", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemCost", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemSell", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemNum", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemID", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemName", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemCost", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemSell", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemNum", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemID", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemName", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemCost", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemSell", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemNum", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemID", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemName", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemCost", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemSell", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemNum", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemID", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemName", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemCost", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemSell", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemNum", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemID", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemName", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemCost", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemSell", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemNum", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemID", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemName", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemCost", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemSell", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemNum", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemNum", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", "暂无商品");
                }
            }
            else
            {
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item1_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item1_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item1_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item1_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item1_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item2_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item3_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item4_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item5_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item6_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item7_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item8_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item9_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item10_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item11_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item12_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemID", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemName", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemCost", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemSell", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Un_Item13_ItemNum", "暂无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "Player_ItemPages_Num", "1/1");
            }
        }//更新ui内容
        #endregion
        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            PlayerInput.onPluginKeyTick -= onPluginKeyTick;
            EffectManager.onEffectButtonClicked -= onEffectButtonClicked;
            EffectManager.onEffectTextCommitted -= onEffectTextCommitted;
            base.Unload();
        }
    }

    #region 操作数据库
    public class MysqlManager 
    {
        private MySqlConnection createConnection()
        {
            MySqlConnection result = null;
            try
            {
                string connstr = $"SERVER={fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.DatabaseAddress};DATABASE={fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.DatabaseName};UID={fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.DatabaseUsername};PASSWORD={fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.DatabasePassword};PORT={fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.DatabasePort};Charset=utf8;";
                result = new MySqlConnection(connstr);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }
            return result;
        }
        internal List<shopItemInfo> getshops(string itemname,bool searchAllShopItem)
        {
            List<shopItemInfo> info = new List<shopItemInfo> { };
            try
            {
                MySqlConnection mySqlConnection = this.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                int count = 0;
                mySqlConnection.Open();
                if (searchAllShopItem)
                {
                    count = getcount("",true);
                    mySqlCommand.CommandText = string.Concat(new string[]
                    {
                    "select * from `",
                    ZaupShop.ZaupShop.Instance.Configuration.Instance.ItemShopTableName,
                    "`;"
                    });
                }
                else
                {
                    count = getcount(itemname, false);
                    mySqlCommand.CommandText = string.Concat(new string[]
                    {
                    "select * from `",
                    ZaupShop.ZaupShop.Instance.Configuration.Instance.ItemShopTableName,
                    "` where `itemname` like '%",
                    itemname,
                    "%';"
                    });
                }
                MySqlDataReader obj = mySqlCommand.ExecuteReader();
                if (obj != null)
                {
                    for (int b = 0; b < count; b++)
                    {
                        int i = 0;
                        shopItemInfo item = new shopItemInfo();
                        obj.Read();
                        item.id = Convert.ToUInt16(obj.GetString(i));
                        item.name = obj.GetString(i + 1);
                        item.buy = Convert.ToDecimal(obj.GetString(i + 2));
                        item.sell = Convert.ToDecimal(obj.GetString(i + 3));
                        item.num = 1;
                        info.Add(item);
                    }
                }
                mySqlConnection.Close();
                mySqlConnection.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }
            return info;
        }//商品名称模糊搜索or查询所有
        internal int getcount(string name,bool searchAll)
        {
            MySqlConnection mySqlConnection = createConnection();
            MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
            int num = 0;
            if (searchAll)
            {
                mySqlCommand.CommandText = "SELECT COUNT(*) FROM " + ZaupShop.ZaupShop.Instance.Configuration.Instance.ItemShopTableName + ";";
            }
            else 
            {
                mySqlCommand.CommandText = "SELECT COUNT(*) FROM " + ZaupShop.ZaupShop.Instance.Configuration.Instance.ItemShopTableName + " where `itemname` like '%" + name + "%';";
            }
            mySqlConnection.Open();
            object obj = mySqlCommand.ExecuteScalar();
            mySqlConnection.Close();
            mySqlConnection.Dispose();
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out num);
            }
            if (num == 0)
            {
                return 0;
            }
            else { return num; }
        }//获取数据库中出售商品总数
        internal decimal getitemcost(string itemid)
        {
            decimal num = 0m;
            try
            {
                MySqlConnection mySqlConnection = this.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat(new string[]
                {
                    "select `cost` from `",
                    ZaupShop.ZaupShop.Instance.Configuration.Instance.ItemShopTableName,
                    "` where `id` = '",
                    itemid,
                    "';"
                });
                mySqlConnection.Open();
                object obj = mySqlCommand.ExecuteScalar();
                if (obj != null)
                {
                    decimal.TryParse(obj.ToString(), out num);
                }
                mySqlConnection.Close();
                mySqlConnection.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }
            return num;
        }//查询商品售价
        internal decimal getitemsell(string itemid)
        {
            decimal num = 0m;
            try
            {
                MySqlConnection mySqlConnection = this.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat(new string[]
                {
                    "select `buyback` from `",
                    ZaupShop.ZaupShop.Instance.Configuration.Instance.ItemShopTableName,
                    "` where `id` = '",
                    itemid,
                    "';"
                });
                mySqlConnection.Open();
                object obj = mySqlCommand.ExecuteScalar();
                if (obj != null)
                {
                    decimal.TryParse(obj.ToString(), out num);
                }
                mySqlConnection.Close();
                mySqlConnection.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }
            return num;
        }//查询商品回收价格
        internal string getitemname(string itemid)
        {
            string num = "";
            try
            {
                MySqlConnection mySqlConnection = this.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat(new string[]
                {
                    "select `itemname` from `",
                    ZaupShop.ZaupShop.Instance.Configuration.Instance.ItemShopTableName,
                    "` where `id` = '",
                    itemid,
                    "';"
                });
                mySqlConnection.Open();
                object obj = mySqlCommand.ExecuteScalar();
                num = obj.ToString();
                mySqlConnection.Close();
                mySqlConnection.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }
            return num;
        }//查询商品名称
        internal void IncreaseBalance(string steamId, decimal increaseBy)
        {
            fr34kyn01535.Uconomy.Uconomy.Instance.Database.IncreaseBalance(steamId,increaseBy);
        }//增减玩家氪金货币
        internal decimal GetBalance(string steamId)
        {
           return fr34kyn01535.Uconomy.Uconomy.Instance.Database.GetBalance(steamId);
        }//获取玩家当前氪金货币数量
    }
    #endregion
    public class shopItemInfo 
    {
        public string name;
        public ushort id;
        public decimal buy;
        public decimal sell;
        public byte num;
    }//商品信息
}
