using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 云背包.Commands
{
    public class CommandYBB : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "ybb";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { "backpack"};

        public List<string> Permissions => new List<string> { };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = caller as UnturnedPlayer;
            player.Player.inventory.updateItems(PlayerInventory.STORAGE, BackPackMain.instance.playerItems[player.CSteamID]);
            player.Player.inventory.sendStorage();
        }
    }
}
