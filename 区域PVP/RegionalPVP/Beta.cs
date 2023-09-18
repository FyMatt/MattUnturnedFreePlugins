
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
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
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RegionalPVP
{
    public class Beta : RocketPlugin<ConfigRegion>
    {
        public static Beta instance;
        public List<Vector3> addopint = new List<Vector3> { };
        public List<Vector3> delopint = new List<Vector3> { };
        public Dictionary<CSteamID, bool> playerdic;
        public Dictionary<CSteamID, string> ppoint;
        public Dictionary<CSteamID, bool> pispvpmode;
        public Dictionary<CSteamID, string> playername;
        public Vector3 point1 { get; set; }
        public Vector3 point2 { get; set; }
        public Vector3 yuanxinpoint { get; set; }
        public double range { get; set; }
        public bool servermode;
        public List<EDeathCause> causes = new List<EDeathCause> {EDeathCause.SPLASH,EDeathCause.CHARGE,EDeathCause.GUN, EDeathCause.VEHICLE, EDeathCause.MELEE, EDeathCause.GRENADE, EDeathCause.LANDMINE, EDeathCause.MISSILE, EDeathCause.SENTRY };
        protected override void Load()
        {
            Logger.LogWarning($"欢迎使用 {Assembly.GetName().Name} 版本：{Assembly.GetName().Version}");
            Logger.LogWarning("Matt插件群：656525189");

            instance = this;
            servermode = Provider.isPvP;
            playerdic = new Dictionary<CSteamID, bool> { };
            ppoint = new Dictionary<CSteamID, string> { };
            pispvpmode = new Dictionary<CSteamID, bool> { };
            playername = new Dictionary<CSteamID, string> { };
            UnturnedPlayerEvents.OnPlayerUpdatePosition += UnturnedPlayerEvents_OnPlayerUpdatePosition;
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            DamageTool.onPlayerAllowedToDamagePlayer += DamageTool_onPlayerAllowedToDamagePlayer;
            DamageTool.playerDamaged += playerDamaged;
            for (int i = 0; i < Configuration.Instance.PVPRegion.Count; i++)
            {
                addopint.Add(new Vector3(Configuration.Instance.PVPRegion[i].x + Configuration.Instance.PVPRegion[i].range, 0, Configuration.Instance.PVPRegion[i].z + Configuration.Instance.PVPRegion[i].range));
                delopint.Add(new Vector3(Configuration.Instance.PVPRegion[i].x - Configuration.Instance.PVPRegion[i].range, 0, Configuration.Instance.PVPRegion[i].z - Configuration.Instance.PVPRegion[i].range));
            }

        }


        private void playerDamaged(Player player, ref EDeathCause cause, ref ELimb limb, ref CSteamID killer, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage)
        {
            if (causes.Contains(cause))
            {
                UnturnedPlayer damager = UnturnedPlayer.FromPlayer(player);
                if (pispvpmode[damager.CSteamID] == false)
                {
                    canDamage = false;
                }
            }
        }

        private void DamageTool_onPlayerAllowedToDamagePlayer(Player instigator, Player victim, ref bool isAllowed)
        {
            UnturnedPlayer killer = UnturnedPlayer.FromPlayer(instigator);
            UnturnedPlayer damager = UnturnedPlayer.FromPlayer(victim);
            if (pispvpmode[killer.CSteamID] == true && pispvpmode[damager.CSteamID] == true)
            {
                isAllowed = true;
            }
            else
            {
                isAllowed = false;
            }
        }

        private void Events_OnPlayerConnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            playerdic.Add(player.CSteamID, true);
            ppoint.Add(player.CSteamID, "null");
            pispvpmode.Add(player.CSteamID, Provider.isPvP);
            playername.Add(player.CSteamID, "null");
        }

        private void Events_OnPlayerDisconnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            playerdic.Remove(player.CSteamID);
            ppoint.Remove(player.CSteamID);
            pispvpmode.Remove(player.CSteamID);
            playername.Remove(player.CSteamID);
        }

        private void UnturnedPlayerEvents_OnPlayerUpdatePosition(Rocket.Unturned.Player.UnturnedPlayer player, Vector3 position)
        {
            for (int i = 0; i < Configuration.Instance.PVPRegion.Count; i++)
            {
                if (delopint[i].x <= player.Position.x && player.Position.x <= addopint[i].x && delopint[i].z <= player.Position.z && player.Position.z <= addopint[i].z)
                {
                    if (playername[player.CSteamID] != Configuration.Instance.PVPRegion[i].name && playername[player.CSteamID] != "null")
                    {
                        playerdic[player.CSteamID] = true;
                        UnturnedChat.Say(player, "正在更新所在区域...", Color.yellow);
                    }
                    if (Configuration.Instance.PVPRegion[i].isPVP == true)
                    {
                        pispvpmode[player.CSteamID] = true;
                        Provider.isPvP = true;
                    }
                    else
                    {
                        pispvpmode[player.CSteamID] = false;
                        Provider.isPvP = false;
                    }
                    if (playerdic[player.CSteamID] == true)
                    {
                        playerdic[player.CSteamID] = false;
                        playername[player.CSteamID] = Configuration.Instance.PVPRegion[i].name;
                        string pvp = "PVP";
                        if (Configuration.Instance.PVPRegion[i].isPVP == false) { pvp = "PVE"; }
                        ppoint[player.CSteamID] = Configuration.Instance.PVPRegion[i].name + "】[" + pvp;
                        if (pvp == "PVP")
                        {
                            UnturnedChat.Say(player, "当前已进入:【" + ppoint[player.CSteamID] + "]区域！请拿好你的武器,随时准备战斗！", Color.yellow);
                        }
                        else
                        {
                            UnturnedChat.Say(player, "当前已进入:【" + ppoint[player.CSteamID] + "]区域！", Color.yellow);
                        }
                        return;
                    }
                }
                else
                {
                    if (Playerisonpoint(player) == false)
                    {
                        if (playerdic[player.CSteamID] == false)
                        {
                            playername[player.CSteamID] = "null";
                            playerdic[player.CSteamID] = true;
                            pispvpmode[player.CSteamID] = servermode;
                            Provider.isPvP = servermode;
                            UnturnedChat.Say(player, "当前已离开:【" + ppoint[player.CSteamID] + "]区域！", Color.yellow);
                            return;
                        }
                    }
                }
            }
        }

        public bool Playerisonpoint(UnturnedPlayer player)
        {
            bool on = false;
            for (int i = 0; i < Configuration.Instance.PVPRegion.Count; i++)
            {
                if (delopint[i].x <= player.Position.x && player.Position.x <= addopint[i].x && delopint[i].z <= player.Position.z && player.Position.z <= addopint[i].z)
                {
                    on = true;
                }
            }
            return on;
        }

        protected override void Unload()
        {
            DamageTool.onPlayerAllowedToDamagePlayer -= DamageTool_onPlayerAllowedToDamagePlayer;
            DamageTool.playerDamaged -= playerDamaged;
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= UnturnedPlayerEvents_OnPlayerUpdatePosition;
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
        }
    }
    public class CommandZB : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "point";

        public string Help => "通过此指令获得当前坐标";

        public string Syntax => "/point";

        public List<string> Aliases => new List<string> { };

        public List<string> Permissions => new List<string> { };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer p = (UnturnedPlayer)caller;
            UnturnedChat.Say(p, "当前坐标：" + p.Position);
        }
    }

    public class CommandSetPoint : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "setP";

        public string Help => "指令格式：/setP <1或者2> <x> <z>     设置第一个坐标的x z值或第二个坐标的x z值 然后再使用/getR 自动计算半径并返回数据";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { };

        public List<string> Permissions => new List<string> { };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length == 3)
            {
                if (command[0] == "1" || command[0] == "2")
                {
                    float x;
                    float.TryParse(command[1], out x);
                    float z;
                    float.TryParse(command[2], out z);
                    if (command[0] == "1")
                    {
                        Beta.instance.point1 = new Vector3(x, 0, z);
                        UnturnedChat.Say(player.CSteamID, "成功对坐标1赋值:" + Beta.instance.point1 + ",两个坐标都赋值后请输入/getR获取半径值", Color.yellow);
                    }
                    if (command[0] == "2")
                    {
                        Beta.instance.point2 = new Vector3(x, 0, z);
                        UnturnedChat.Say(player.CSteamID, "成功对坐标2赋值:" + Beta.instance.point2 + ",两个坐标都赋值后请输入/getR获取半径值", Color.yellow);
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
    }

    public class CommandGetRange : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "getR";

        public string Help => "指令格式：/getR <圆心为坐标1还是坐标2。1的话就填1>  获取半径后输入指令/setRM查看使用方法后再填写参数进行设置插件配置文件";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { };

        public List<string> Permissions => new List<string> { };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (Beta.instance.point1 != new Vector3() && Beta.instance.point2 != new Vector3())
            {
                if (command.Length == 1)
                {
                    if (command[0] == "1") { Beta.instance.yuanxinpoint = Beta.instance.point1; }
                    if (command[0] == "2") { Beta.instance.yuanxinpoint = Beta.instance.point2; }
                    UnturnedChat.Say(player.CSteamID, "成功设置圆心坐标:" + Beta.instance.yuanxinpoint, Color.yellow);
                    double range;
                    range = getRange(Beta.instance.point1, Beta.instance.point2);
                    Beta.instance.range = range;
                    UnturnedChat.Say(player.CSteamID, "半径为：" + range, Color.yellow);
                    UnturnedChat.Say(player.CSteamID, "成功取得半径，请输入/setRM  进行下一步操作!", Color.yellow);
                }
                else
                {
                    UnturnedChat.Say(player.CSteamID, Help, Color.red);
                }

            }
            else
            {
                if (Beta.instance.point1 == new Vector3())
                {
                    UnturnedChat.Say(player.CSteamID, "坐标1为空，请使用/setP 1 <x> <z> 来设置坐标1", Color.red);
                }
                if (Beta.instance.point2 == new Vector3())
                {
                    UnturnedChat.Say(player.CSteamID, "坐标2为空，请使用/setP 2 <x> <z> 来设置坐标2", Color.red);
                }
            }

        }


        public double getRange(Vector3 point1, Vector3 point2)
        {
            double range = default(double);
            range=Math.Sqrt((point1.x - point2.x) * (point1.x - point2.x) + (point1.z - point2.z) * (point1.z - point2.z));
            return range;
        }
    }
    public class CommandSetRegionalMode : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "setRM";

        public string Help => "/setRM <区域名称> <pvp或者pve(参数都是小写)>";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { };

        public List<string> Permissions => new List<string> {  };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (Beta.instance.yuanxinpoint != null && Beta.instance.range != 0)
            {
                if (command.Length == 2)
                {
                    if (command[1] == "pvp" || command[1] == "pve")
                    {
                        string name = command[0];
                        string mode = command[1];
                        bool conti = true;
                        bool mo = false;
                        if (mode == "pvp") { mo = true; }
                        foreach (var item in Beta.instance.Configuration.Instance.PVPRegion)
                        {
                            if (name == item.name)
                            {
                                conti = false;
                            }
                        }
                        if (conti)
                        {
                            Beta.instance.Configuration.Instance.PVPRegion.Add(new Region { name = name, range = (float)Beta.instance.range, isPVP = mo, x = Beta.instance.yuanxinpoint.x, z = Beta.instance.yuanxinpoint.z });
                            Beta.instance.Configuration.Save();
                            UnturnedChat.Say(player.CSteamID, "新的区域产生！信息如下：", Color.yellow);
                            UnturnedChat.Say(player.CSteamID, "区域名称：" + name + "--半径：" + Beta.instance.range + "--模式:" + mode + "--圆心坐标:" + Beta.instance.yuanxinpoint, Color.yellow);
                            UnturnedChat.Say(player.CSteamID, "已修改配置文件并且卸载插件，请重启服务器！");
                            Beta.instance.point1 = new Vector3();
                            Beta.instance.point2 = new Vector3();
                            Beta.instance.yuanxinpoint = new Vector3();
                            Beta.instance.range = 0;
                        }
                        else
                        {
                            UnturnedChat.Say(player.CSteamID, "此区域名称已存在!", Color.red);
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
            else
            {
                UnturnedChat.Say(player.CSteamID, "圆心为空或半径为0，请输入/getR  进行设置", Color.red);
            }
        }
    }

    public class ConfigRegion : IRocketPluginConfiguration,IDefaultable
    {
        public void LoadDefaults()
        {
            PVPRegion.Add(new Region { name = "飞机场", range = 100, isPVP = true, x = 747, z = 647 });
        }
        public List<Region> PVPRegion = new List<Region> { };
    }
    public class Region
    {
        public string name;
        public float range;
        public bool isPVP;
        public float x;
        public float z;
    }
}
