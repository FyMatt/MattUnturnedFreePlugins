using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matt.Rp插件基础环境.Events
{
    public class MattRPEvents
    {
        public delegate void playerRegistered(UnturnedPlayer player);
        public static playerRegistered onPlayerRegistered; //玩家完成注册事件
        public delegate void playerStartPlaying(UnturnedPlayer player);
        public static playerStartPlaying onPlayerStartPlaying;//玩家开始游戏事件
    }
}
