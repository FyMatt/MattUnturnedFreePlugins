
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using static SDG.Unturned.WeatherAsset;
using Logger = Rocket.Core.Logging.Logger;

namespace 氪金货币
{
    public class Beta : RocketPlugin<Config>
    {
        public static Beta Instance;
        public MysqlManager manager;
        public Dictionary<CSteamID, bool> uistate;
        public Dictionary<CSteamID, int> uipage;//玩家商店ui显示的页数
        public Dictionary<CSteamID, List<shopItemInfo>> shops { get; set; }

        public delegate void onPlayer_KjMoneyUpdate(UnturnedPlayer player, decimal nowMoney);
        public onPlayer_KjMoneyUpdate onPlayer_kjMoneyUpdate;

        public delegate void OnPlayerSellItem(UnturnedPlayer player, itemInfo item);
        public event OnPlayerSellItem onPlayerSellItem;
        public OnPlayerSellItem PlayerSellItem;

        protected override void Load()
        {
            Instance = this;
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
            if (buttonName == "player_search_input")
            {
                if (text.Length!=0)
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
        }
        private void onEffectButtonClicked(Player player, string buttonName)
        {
            UnturnedPlayer p = UnturnedPlayer.FromPlayer(player);
            if (buttonName == "rightbutton")
            {
                if (uipage[p.CSteamID] != getpagecount(shops[p.CSteamID]))
                {
                    uipage[p.CSteamID] += 1;
                    updateShopUI(p, shops[p.CSteamID], uipage[p.CSteamID]);
                }
            }
            else if (buttonName == "leftbutton")
            {
                if (uipage[p.CSteamID] != 1)
                {
                    uipage[p.CSteamID] -= 1;
                    updateShopUI(p, shops[p.CSteamID], uipage[p.CSteamID]);
                }
            }
            if (buttonName == "item_1_buy_button") 
            {
                if (getshopID(p, "item_1_buy_button") != 0) { buyitem(p, getshopID(p, "item_1_buy_button").ToString(), 1); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }else if (buttonName == "item_2_buy_button")
            {
                if (getshopID(p, "item_2_buy_button") != 0) {buyitem(p, getshopID(p, "item_2_buy_button").ToString(), 1); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "item_3_buy_button")
            {
                if (getshopID(p, "item_3_buy_button") != 0) {buyitem(p, getshopID(p, "item_3_buy_button").ToString(), 1); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
            else if (buttonName == "item_4_buy_button")
            {
                if (getshopID(p, "item_4_buy_button") != 0) {buyitem(p, getshopID(p, "item_4_buy_button").ToString(), 1); } else { UnturnedChat.Say(p.CSteamID, "商品不存在！", Color.red); }
            }
        }
        private void onPluginKeyTick(Player player, uint simulation, byte key, bool state)
        {
            UnturnedPlayer p = UnturnedPlayer.FromPlayer(player);
            if (key == 1 && state == true && uistate[p.CSteamID] == false)
            {
                shops[p.CSteamID] = manager.getshops("",true);
                uistate[p.CSteamID] = true;
                uipage[p.CSteamID] = 1;
                EffectManager.sendUIEffect(Configuration.Instance.effectId, (short)Configuration.Instance.effectId, p.CSteamID, false);
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, p.CSteamID, false, "player_money", manager.GetBalance(p.CSteamID.ToString()).ToString() + " " + Configuration.Instance.货币名称);
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, p.CSteamID, false, "shoptitle", Configuration.Instance.ui_顶部文本);
                updateShopUI(p,shops[p.CSteamID],1);
                p.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
            } else if (key == 1 && state == true && uistate[p.CSteamID])
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
            manager.CheckSetupAccount(player);
        }
        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            shops.Remove(player.CSteamID);
            uipage.Remove(player.CSteamID);
            uistate.Remove(player.CSteamID);
        }

        #region 方法库
        private ushort getshopID(UnturnedPlayer player,string buyButtonName) 
        {
            int index = (uipage[player.CSteamID] - 1) * 4;
            if (buyButtonName == "item_2_buy_button")
            {
                index += 1;
            }
            else if (buyButtonName == "item_3_buy_button")
            {
                index += 2;
            }
            else if (buyButtonName == "item_4_buy_button")
            {
                index += 3;
            }
            ushort id = 0;
            if (shops[player.CSteamID].Count > index) 
            {
                id = shops[player.CSteamID][index].id;
            }
            return id;
        }//通过按下的按钮获取该商品ID
        private int getpagecount(List<shopItemInfo> infos) 
        {
            int page = infos.Count / 4;
            if ((infos.Count % 4) != 0)
            {
                page += 1;
            }
            return page;
        }//获取匹配的所有商品集合
        private void buyitem(UnturnedPlayer player, string itemid,byte num) 
        {
            decimal allcost = manager.getitemcost(itemid) * num;
            if (allcost != 0)
            {
                if (manager.GetBalance(player.CSteamID.ToString()) >= allcost)
                {
                    manager.IncreaseBalance(player.CSteamID.ToString(), 0 - allcost);
                    player.GiveItem(Convert.ToUInt16(itemid), num);
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "player_money", manager.GetBalance(player.CSteamID.ToString()).ToString() + " " + Configuration.Instance.货币名称);
                    UnturnedChat.Say(player.CSteamID, "成功消费了：" + allcost + Configuration.Instance.货币名称 + "  ----当前剩余：" + manager.GetBalance(player.CSteamID.ToString()).ToString() + Configuration.Instance.货币符号, Color.yellow);
                }
                else
                {
                    UnturnedChat.Say(player.CSteamID, "你的 " + Configuration.Instance.货币名称 +  " 不足", Color.red);
                    UnturnedChat.Say(player.CSteamID, "当前剩余：" + manager.GetBalance(player.CSteamID.ToString()).ToString() + Configuration.Instance.货币符号, Color.red);
                }
            }
            else
            {
                UnturnedChat.Say(player, "抱歉，此商品禁止购买！", Color.red);
            }
        }//购买物品
        private void updateShopUI(UnturnedPlayer player,List<shopItemInfo> infos, int page) 
        {
            if (page == 1 && page < getpagecount(infos))
            {
                EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "leftbutton", false);
                EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "rightbutton", true);
            }
            else if (page == getpagecount(infos) && page > 1)
            {
                EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "leftbutton", true);
                EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "rightbutton", false);
            }else if (getpagecount(infos) == 1 && page==1)
            {
                EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "leftbutton", false);
                EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "rightbutton", false);
            }
            else 
            {
                EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "leftbutton", true);
                EffectManager.sendUIEffectVisibility((short)Configuration.Instance.effectId, player.CSteamID, false, "rightbutton", true);
            }
            string pge = page + "/" + getpagecount(infos);
            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "shoppage", pge);
            int index = (page - 1) * 4;
            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_1_buy", infos[index].buy.ToString() + " " + Configuration.Instance.货币符号);
            EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_1_name", infos[index].name.ToString());
            index += 1;
            if (infos.Count > index)
            {
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_2_buy", infos[index].buy.ToString() + " " + Configuration.Instance.货币符号);
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_2_name", infos[index].name.ToString());
                index += 1;
                if (infos.Count > index)
                {
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_3_buy", infos[index].buy.ToString() + " " + Configuration.Instance.货币符号);
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_3_name", infos[index].name.ToString());
                    index += 1;
                    if (infos.Count > index)
                    {
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_4_buy", infos[index].buy.ToString() + " " + Configuration.Instance.货币符号);
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_4_name", infos[index].name.ToString());
                    }
                    else 
                    {
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_4_buy", "无商品");
                        EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_4_name", "无商品");
                    }
                }
                else 
                {
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_3_buy", "无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_3_name", "无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_4_buy", "无商品");
                    EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_4_name", "无商品");
                }
            }
            else 
            {
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_2_buy", "无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_2_name", "无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_3_buy", "无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_3_name", "无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_4_buy", "无商品");
                EffectManager.sendUIEffectText((short)Configuration.Instance.effectId, player.CSteamID, false, "item_4_name", "无商品");
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
        internal MysqlManager()
        {
            this.CheckSchema();
        }
        private MySqlConnection createConnection()
        {
            MySqlConnection result = null;
            try
            {
                string connstr = $"SERVER={Beta.Instance.Configuration.Instance.MySQL地址};DATABASE={Beta.Instance.Configuration.Instance.MySQL库名};UID={Beta.Instance.Configuration.Instance.MySQL用户名};PASSWORD={Beta.Instance.Configuration.Instance.MySQL密码};PORT={Beta.Instance.Configuration.Instance.MySQL端口};Charset=utf8;";
                result = new MySqlConnection(connstr);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }
            return result;
        }
        internal void CheckSchema()
        {
            try
            {
                MySqlConnection mySqlConnection = this.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlConnection.Open();
                mySqlCommand.CommandText = "show tables like '" + Beta.Instance.Configuration.Instance.数据库氪金表表名 + "'";
                if (mySqlCommand.ExecuteScalar() == null)
                {
                    mySqlCommand.CommandText = "CREATE TABLE `" + Beta.Instance.Configuration.Instance.数据库氪金表表名 + "` (`steamId` varchar(32) NOT NULL,`PlayerName` varchar(255) NOT NULL,`Balance` decimal(15,2) NOT NULL DEFAULT '00.00',`lastUpdated` timestamp NOT NULL DEFAULT NOW() ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`steamId`)) ";
                    mySqlCommand.ExecuteNonQuery();
                }
                mySqlCommand.CommandText = "show tables like '" + Beta.Instance.Configuration.Instance.氪金商品表表名 + "'";
                if (mySqlCommand.ExecuteScalar() == null)
                {
                    mySqlCommand.CommandText = "CREATE TABLE `" + Beta.Instance.Configuration.Instance.氪金商品表表名 + "` (`id` varchar(32) NOT NULL,`name` varchar(255) NOT NULL,`buy` decimal(15,2) NOT NULL DEFAULT '30.00',PRIMARY KEY (`id`)) ";
                    mySqlCommand.ExecuteNonQuery();
                }
                mySqlConnection.Close();
                mySqlConnection.Dispose();
                checkStruct();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }
        }

        private void checkStruct()
        {
            MySqlConnection mySqlConnection = this.createConnection();
            MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
            mySqlConnection.Open();
            mySqlCommand.CommandText = "desc `" + Beta.Instance.Configuration.Instance.氪金商品表表名 + "` `sell`";
            if (mySqlCommand.ExecuteScalar() == null)
            {
                mySqlCommand.CommandText = "ALTER TABLE `" + Beta.Instance.Configuration.Instance.氪金商品表表名 + "` ADD `sell` decimal(15,2) NOT NULL DEFAULT '00.00' ";
                mySqlCommand.ExecuteNonQuery();
            }
            mySqlConnection.Close();
            mySqlConnection.Dispose();
        }

        internal void CheckSetupAccount(UnturnedPlayer player)
        {
            try
            {
                MySqlConnection mySqlConnection = this.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                int num = 0;
                mySqlCommand.CommandText = string.Concat(new object[]
                {
            "SELECT EXISTS(SELECT 1 FROM `",
            Beta.Instance.Configuration.Instance.数据库氪金表表名,
            "` WHERE `steamId` ='",
            player.CSteamID.ToString(),
            "' LIMIT 1);"
                });
                mySqlConnection.Open();
                object obj = mySqlCommand.ExecuteScalar();
                if (obj != null)
                {
                    int.TryParse(obj.ToString(), out num);
                }
                if (num == 0)
                {
                    mySqlCommand.CommandText = string.Concat(new object[]
                    {
                        "insert ignore into `",
                        Beta.Instance.Configuration.Instance.数据库氪金表表名,
                        "` (Balance,steamId,PlayerName,lastUpdated) values('",
                        Beta.Instance.Configuration.Instance.默认氪金货币数量,
                        "','",
                        player.CSteamID.ToString(),
                        "','",
                        player.CharacterName.ToString(),
                        "',now())"
                    });
                    mySqlCommand.ExecuteNonQuery();
                    mySqlConnection.Close();
                    mySqlConnection.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }
        }//建立新的玩家档案
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
                    Beta.Instance.Configuration.Instance.氪金商品表表名,
                    "`;"
                    });
                }
                else
                {
                    count = getcount(itemname, false);
                    mySqlCommand.CommandText = string.Concat(new string[]
                    {
                    "select * from `",
                    Beta.Instance.Configuration.Instance.氪金商品表表名,
                    "` where `name` like '%",
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
                mySqlCommand.CommandText = "SELECT COUNT(*) FROM " + Beta.Instance.Configuration.Instance.氪金商品表表名 + ";";
            }
            else 
            {
                mySqlCommand.CommandText = "SELECT COUNT(*) FROM " + Beta.Instance.Configuration.Instance.氪金商品表表名 + " where `name` like '%" + name + "%';";
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
                    "select `buy` from `",
                    Beta.Instance.Configuration.Instance.氪金商品表表名,
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
        }//查询商品价格
        internal decimal getitemsell(string itemid)
        {
            decimal num = 0m;
            try
            {
                MySqlConnection mySqlConnection = this.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat(new string[]
                {
                    "select `sell` from `",
                    Beta.Instance.Configuration.Instance.氪金商品表表名,
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
                    "select `name` from `",
                    Beta.Instance.Configuration.Instance.氪金商品表表名,
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
        internal bool deleteitemshop(string itemid) 
        {
            bool succes = false;
            MySqlConnection mySqlConnection = this.createConnection();
            MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
            int num = 0;
            mySqlCommand.CommandText = string.Concat(new object[]
            {
            "SELECT EXISTS(SELECT 1 FROM `",
            Beta.Instance.Configuration.Instance.氪金商品表表名,
            "` WHERE `id` ='",
            itemid,
            "' LIMIT 1);"
            });
            mySqlConnection.Open();
            object obj = mySqlCommand.ExecuteScalar();
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out num);
            }
            if (num != 0)
            {
                succes = true;
                mySqlCommand.CommandText = string.Concat(new string[]
                 {
                    "delete from `",
                    Beta.Instance.Configuration.Instance.氪金商品表表名,
                    "` where `id` = '",
                    itemid,
                    "';"
                 });
                mySqlCommand.ExecuteNonQuery();
            }
            mySqlConnection.Close();
            mySqlConnection.Dispose();
            return succes;
        }//删除商品信息
        internal bool additemshop(string itemid, string itemname, decimal buy)
        {
            bool succes = false;
            MySqlConnection mySqlConnection = this.createConnection();
            MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
            int num = 0;
            mySqlCommand.CommandText = string.Concat(new object[]
            {
            "SELECT EXISTS(SELECT 1 FROM `",
            Beta.Instance.Configuration.Instance.氪金商品表表名,
            "` WHERE `id` ='",
            itemid,
            "' LIMIT 1);"
            });
            mySqlConnection.Open();
            object obj = mySqlCommand.ExecuteScalar();
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out num);
            }
            if (num == 0)
            {
                succes = true;
                mySqlCommand.CommandText = string.Concat(new object[]
                    {
                        "insert ignore into `",
                        Beta.Instance.Configuration.Instance.氪金商品表表名,
                        "` (id,name,buy) values('",
                        itemid,
                        "','",
                        itemname,
                        "','",
                        buy,
                        ",')"
                    });
                mySqlCommand.ExecuteNonQuery();
            }
            mySqlConnection.Close();
            mySqlConnection.Dispose();
            return succes;
        }//添加商品信息
        internal bool updateitemshop(string itemid, decimal buy) 
        {
            bool succes = false;
            MySqlConnection mySqlConnection = this.createConnection();
            MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
            int num = 0;
            mySqlCommand.CommandText = string.Concat(new object[]
            {
            "SELECT EXISTS(SELECT 1 FROM `",
            Beta.Instance.Configuration.Instance.氪金商品表表名,
            "` WHERE `id` ='",
            itemid,
            "' LIMIT 1);"
            });
            mySqlConnection.Open();
            object obj = mySqlCommand.ExecuteScalar();
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out num);
            }
            if (num != 0)
            {
                succes = true;
                mySqlCommand.CommandText = string.Concat(new object[]
                {
                    "update `",
                    Beta.Instance.Configuration.Instance.氪金商品表表名,
                    "` set `buy` = ('",
                    buy,
                    "') where `id` = '",
                    itemid,
                    "'"
                });
                mySqlCommand.ExecuteNonQuery();
            }
            mySqlConnection.Close();
            mySqlConnection.Dispose();
            return succes;
        } //更新商品价格
        internal decimal IncreaseBalance(string steamId, decimal increaseBy)
        {
            decimal result = 0m;
            try
            {
                MySqlConnection mySqlConnection = this.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat(new object[]
                {
                    "update `",
                    Beta.Instance.Configuration.Instance.数据库氪金表表名,
                    "` set `Balance` = Balance + (",
                    increaseBy,
                    ") where `steamId` = '",
                    steamId.ToString(),
                    "'; select `Balance` from `",
                    Beta.Instance.Configuration.Instance.数据库氪金表表名,
                    "` where `steamId` = '",
                    steamId.ToString(),
                    "'"
                });
                mySqlConnection.Open();
                object obj = mySqlCommand.ExecuteScalar();
                if (obj != null)
                {
                    decimal.TryParse(obj.ToString(), out result);
                }
                mySqlConnection.Close();
                mySqlConnection.Dispose();
                if (Beta.Instance.onPlayer_kjMoneyUpdate != null)
                {
                    Beta.Instance.onPlayer_kjMoneyUpdate.Invoke(UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(steamId))), result);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }
            return result;
        }//增减玩家氪金货币
        internal decimal GetBalance(string steamId)
        {
            decimal num = 0m;
            try
            {
                MySqlConnection mySqlConnection = this.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat(new string[]
                {
                    "select `Balance` from `",
                    Beta.Instance.Configuration.Instance.数据库氪金表表名,
                    "` where `steamId` = '",
                    steamId.ToString(),
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
        }//获取玩家当前氪金货币数量
    }
    #endregion
    public class shopItemInfo 
    {
        public string name;
        public ushort id;
        public decimal buy;
    }//商品信息结构类
    public class Config : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            effectId = 2014;
            MySQL地址 = "127.0.0.1";
            MySQL用户名 = "root";
            MySQL密码 = "password";
            MySQL端口 = 3306;
            MySQL库名 = "unturned";
            ui_顶部文本 = "货币商店";
            数据库氪金表表名 = "kejin_player";
            氪金商品表表名 = "kejin_itemshop";
            货币名称 = "美刀";
            货币符号 = "$";
            默认氪金货币数量 = 0;
        }
        [XmlElement("特效ID")]
        public ushort effectId;
        public string MySQL地址;
        public string MySQL用户名;
        public string MySQL密码;
        public int MySQL端口;
        public string MySQL库名;
        public string ui_顶部文本;
        public string 数据库氪金表表名;
        public string 氪金商品表表名;
        public string 货币名称;
        public string 货币符号;
        public decimal 默认氪金货币数量;
    }
    public class kjsellcommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "kjsell";

        public string Help => "出售物品/kjsell <ID> <数量不填默认为1>";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { };

        public List<string> Permissions => new List<string> { "kjsell" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length == 1 || command.Length == 2)
            {
                try
                {
                    ushort id = Convert.ToUInt16(command[0]);
                    byte num = 1;
                    if (command.Length == 2) num = Convert.ToByte(command[1]);
                    decimal cost = num * Beta.Instance.manager.getitemsell(id.ToString());
                    if (cost > 0)
                    {
                        ItemAsset item = Assets.find(EAssetType.ITEM, id) as ItemAsset;
                        if (item != null)
                        {
                            if (player.Inventory.search(id, false, true).Count >= num)
                            {
                                for (int i = 0; i < num; i++)
                                {
                                    var v = player.Inventory.search(id, false, true)[0];
                                    itemInfo itemInfo = new itemInfo { page = v.page, index = player.Inventory.getIndex(v.page, v.jar.x, v.jar.y) };
                                    if (Beta.Instance.PlayerSellItem != null)
                                    {
                                        Beta.Instance.PlayerSellItem.Invoke(player, itemInfo);
                                    }
                                    player.Inventory.removeItem(v.page, player.Inventory.getIndex(v.page, v.jar.x, v.jar.y));
                                }
                                Beta.Instance.manager.IncreaseBalance(player.CSteamID.ToString(), cost);
                                UnturnedChat.Say(player.CSteamID, "成功出售" + num + "个 " + item.name + "  已获得：" + cost, Color.yellow);
                            }
                            else
                            {
                                UnturnedChat.Say(player.CSteamID, "你的库存不足！", Color.red);
                            }
                        }
                        else
                        {
                            UnturnedChat.Say(player.CSteamID, "该物品不存在！", Color.red);
                        }
                    }
                    else
                    {
                        UnturnedChat.Say(player.CSteamID, "该物品禁止出售！", Color.red);
                    }
                }
                catch 
                {
                    UnturnedChat.Say(player.CSteamID, "该物品不存在！", Color.red);
                }
            }
            else 
            {
                UnturnedChat.Say(player.CSteamID, Help, Color.red);
            }
        }
    }

    public class kjshopcommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "kjshop";

        public string Help => "";

        public string Syntax => "";

        public List<string> help => new List<string> { "添加商品/kjshop add <物品ID> <物品名称> <售价>", "修改商品/kjshop chng <物品ID> <售价>", "删除商品/kjshop rem <物品ID>" };

        public List<string> Aliases => new List<string> { };

        public List<string> Permissions => new List<string> { "kjshop"};

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length != 0)
            {
                if (command[0] == "add" && command.Length == 4)
                {
                    string id = command[1];
                    string name = command[2];
                    decimal buy = Convert.ToDecimal(command[3]);
                    if (buy > 0)
                    {
                        if (Beta.Instance.manager.additemshop(id, name, buy))
                        {
                            UnturnedChat.Say(player.CSteamID, "成功上架:" + name + " 单个售价：" + buy, Color.yellow);
                        }
                    }
                    else { UnturnedChat.Say(player, "金额不能少于0", Color.red); }
                }
                else if (command[0] == "chng" && command.Length == 3)
                {
                    string id = command[1];
                    decimal buy = Convert.ToDecimal(command[2]);
                    if (buy > 0)
                    {
                        if (Beta.Instance.manager.updateitemshop(id, buy)) { UnturnedChat.Say(player.CSteamID, "成功修改:" + id + " 单个售价：" + buy, Color.yellow); }
                    }
                    else { UnturnedChat.Say(player, "金额不能少于0", Color.red); }
                }
                else if (command[0] == "rem" && command.Length == 2)
                {
                    string id = command[1];
                    if (Beta.Instance.manager.deleteitemshop(id)) { UnturnedChat.Say(player.CSteamID, "成功下架:" + id, Color.yellow); }
                }
                else
                {
                    foreach (var v in help)
                    {
                        UnturnedChat.Say(player.CSteamID, v, Color.red);
                    }
                }
            }
            else 
            {
                foreach (var v in help)
                {
                    UnturnedChat.Say(player.CSteamID, v, Color.red);
                }
            }
        }
    }//指令上架物品
    public class kjapay : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "kjapay";

        public string Help => "/kjapay <玩家名称或玩家steamID> <数量>";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { };

        public List<string> Permissions => new List<string> { "kjapay"};

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length != 0)
            {
                if (command.Length == 2)
                {
                    UnturnedPlayer p = UnturnedPlayer.FromName(command[0]);
                    if (p == null) p = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(command[0])));
                    decimal num = Convert.ToDecimal(command[1]);
                    if (num > 0)
                    {
                        Beta.Instance.manager.IncreaseBalance(p.CSteamID.ToString(), num);
                        UnturnedChat.Say(player.CSteamID, "成功为玩家：" + p.CharacterName + "   充值了：" + num, Color.yellow);
                        UnturnedChat.Say(p.CSteamID, "成功充值：" + num + "  操作员：" + player.CharacterName, Color.yellow);
                    }
                    else
                    {
                        UnturnedChat.Say(player, "金额不能少于0", Color.red);
                    }
                }
                else
                {
                    UnturnedChat.Say(player.CSteamID, Help, Color.red);
                }
            }
            else
            {
                UnturnedChat.Say(player.CSteamID, Help, Color.red);
            }
        }
    }//服主为玩家充钱
    public class kjpay : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "kjpay";

        public string Help => "/kjpay <玩家名称或玩家steamID> <数量>";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { };

        public List<string> Permissions => new List<string> { "kjpay" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length != 0)
            {
                if (command.Length == 2)
                {
                    UnturnedPlayer p = UnturnedPlayer.FromName(command[0]);
                    if (p == null) p = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(command[0])));
                    decimal num = Convert.ToDecimal(command[1]);
                    if (num > 0)
                    {
                        if (Beta.Instance.manager.GetBalance(player.CSteamID.ToString()) >= num)
                        {
                            if (p.CSteamID != player.CSteamID)
                            {
                                Beta.Instance.manager.IncreaseBalance(player.CSteamID.ToString(), 0 - num);
                                Beta.Instance.manager.IncreaseBalance(p.CSteamID.ToString(), num);
                                UnturnedChat.Say(player.CSteamID, "成功向玩家：" + p.CharacterName + "  转账：" + num, Color.yellow);
                                UnturnedChat.Say(p.CSteamID, "收到来自：" + player.CharacterName + "的转账,此次金额：" + num, Color.yellow);
                            }
                            else { UnturnedChat.Say(player.CSteamID, "不能想自己转账！", Color.red); }
                        }
                        else { UnturnedChat.Say(player.CSteamID, "想屁吃捏，你的金额不足！", Color.red); }
                    }
                    else { UnturnedChat.Say(player, "金额不能少于0", Color.red); }
                }
                else
                {
                    UnturnedChat.Say(player.CSteamID, Help, Color.red);
                }
            }
            else
            {
                UnturnedChat.Say(player.CSteamID, Help, Color.red);
            }
        }
    }//玩家向玩家转账
    public class kjje : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "kjje";

        public string Help => "/kjje";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { };

        public List<string> Permissions => new List<string> { "kjpay" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            UnturnedChat.Say(player.CSteamID,"当前金额："+ Beta.Instance.manager.GetBalance(player.CSteamID.ToString()), Color.green);
        }
    }//查询玩家当前氪金金额
    public class itemInfo
    {
        public byte page;
        public byte index;
    }
}
