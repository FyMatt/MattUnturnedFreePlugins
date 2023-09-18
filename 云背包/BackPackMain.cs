
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
using SqlSugar;
using Rocket.Core;
using Rocket.API.Serialisation;
using 云背包.Models;

namespace 云背包
{
    public class BackPackMain : RocketPlugin<BackPackConfig>
    {
        public static BackPackMain instance { get; private set; }
        public SqlSugarClient sql { get; private set; }
        public Dictionary<CSteamID, Items> playerItems { get; private set; }
      
        //定义一个常量来存储插件群号
        private const string PluginGroup = "656525189";

        //定义一个加载方法
        protected override void Load()
        {
            //使用$符号来格式化字符串
            Logger.LogWarning($"欢迎使用 {Assembly.GetName().Name} 版本：{Assembly.GetName().Version}");
            //使用常量来输出插件群号
            Logger.LogWarning($"Matt插件群：{PluginGroup}");

            instance = this;
            playerItems = new Dictionary<CSteamID, Items> { };
            sql = new SqlSugarClient(new ConnectionConfig()
            {
                //可以在连接字符串中设置连接池pooling=true;表示开启连接池
                //eg:min pool size=2;max poll size=4;表示最小连接池为2，最大连接池是4；默认是100
                ConnectionString = Configuration.Instance.getMySQLConnectionString,
                DbType = SqlSugar.DbType.MySql,//我这里使用的是Mysql数据库
                IsAutoCloseConnection = true,//自动关闭连接
                InitKeyType = InitKeyType.Attribute
            });
            sql.CodeFirst.InitTables<BackPackMySQL>();

            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            Provider.onServerShutdown += onServerShutdown;
        }

        private void onServerShutdown()
        {
            SqlManager.saveBackPackStateToMySQL();
        }

        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            SqlManager.saveBackPackStateToMySQL(player.CSteamID.ToString(), playerItems[player.CSteamID]);
            playerItems.Remove(player.CSteamID);
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            if (!playerItems.ContainsKey(player.CSteamID))
            {
                Items items = new Items(PlayerInventory.STORAGE);
                BackPackGroupConfig config = getMaxBackPackGroupConfig(player);
                items.resize(config.max_x, config.max_y);
                SqlManager.readMySqlBackPackStateToPlayer(player.CSteamID.ToString(), ref items);
                playerItems.Add(player.CSteamID, items);
            }
        }
        private BackPackGroupConfig getMaxBackPackGroupConfig(UnturnedPlayer player)
        {
            BackPackGroupConfig config = Configuration.Instance.groupConfigs.FirstOrDefault(e => e.groupName == "default");
            try
            {
                foreach (var v in Configuration.Instance.groupConfigs)
                {
                    RocketPermissionsGroup group = R.Permissions.GetGroup(v.groupName);
                    if (group != null || player.IsAdmin)
                    {
                        if (group != null && group.Members.Count != 0 && group.Members.Contains(player.CSteamID.ToString()))
                        {
                            int max = v.max_x * v.max_y;
                            if (config == null || config.max_x * config.max_y < max)
                            {
                                config = v;
                            }
                        }
                        else if (player.IsAdmin)
                        {
                            int max = v.max_x * v.max_y;
                            if (config == null || config.max_x * config.max_y < max)
                            {
                                config = v;
                            }
                        }
                    }
                }
            }
            catch (Exception ex){ Logger.LogException(ex); }
            return config;
        }
        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            Provider.onServerShutdown -= onServerShutdown;
            base.Unload();
        }
    }
}
