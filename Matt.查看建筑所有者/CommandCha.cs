using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Matt.查看建筑所有者
{
    public class CommandCha : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "cha";

        public string Help => "cha";

        public string Syntax => "cha";

        public List<string> Aliases => new List<string> { "ca" };

        public List<string> Permissions => new List<string> { };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = caller as UnturnedPlayer;
            if (Main.instance.effectList.Contains(player.CSteamID))
            {
                Main.instance.effectList.Remove(player.CSteamID);
                UnturnedChat.Say(player, "成功关闭查询建筑所有者模式！", Color.red);
            }
            else
            {
                Main.instance.effectList.Add(player.CSteamID);
                UnturnedChat.Say(player, "成功开启查询建筑所有者模式！", Color.yellow);
            }
        }
    }
}
