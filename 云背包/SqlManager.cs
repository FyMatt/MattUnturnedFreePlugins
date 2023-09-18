using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using 云背包.Models;

namespace 云背包
{
    public static class SqlManager
    {
        private static BackPackMain instance => BackPackMain.instance;
        private static SqlSugarClient sql => instance.sql;
        public static void readMySqlBackPackStateToPlayer(string steamId,ref Items items)
        {
            List<BackPackMySQL> result = sql.Queryable<BackPackMySQL>().Where(e => e.steamId == steamId).ToList();
            if (result.Count != 0)
            {
                foreach (var v in result)
                {
                    Item item = new Item(v.itemId, v.amount, v.quality, v.state);
                    items.addItem(v.x, v.y, v.rot, item);
                }
            }
        }
        public static void saveBackPackStateToMySQL()
        {
            if (instance.playerItems.Count != 0)
            {
                foreach (var v in instance.playerItems)
                {
                    saveBackPackStateToMySQL(v.Key.ToString(), v.Value);
                }
            }
        }
        public static void saveBackPackStateToMySQL(string steamId, Items items)
        {
            removeAllBackPackStateOnMySQL(steamId);
            foreach (var v in items.items)
            {
                BackPackMySQL mysql = new BackPackMySQL { steamId = steamId, state = v.item.state, itemId = v.item.id, rot = v.rot, x = v.x, y = v.y, amount = v.item.amount, quality = v.item.quality };
                sql.Insertable<BackPackMySQL>(mysql).ExecuteCommand();
            }
        }
        private static void removeAllBackPackStateOnMySQL(string steamId)
        {
            sql.Deleteable<BackPackMySQL>().Where(e => e.steamId == steamId).ExecuteCommand();
        }
    }
}
