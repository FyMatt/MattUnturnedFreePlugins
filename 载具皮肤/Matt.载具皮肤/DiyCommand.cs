using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Matt.载具皮肤
{
	// Token: 0x02000003 RID: 3
	public class DiyCommand : IRocketCommand
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000210C File Offset: 0x0000030C
		public List<string> Aliases
		{
			get
			{
				return new List<string>();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002124 File Offset: 0x00000324
		public AllowedCaller AllowedCaller
		{
			get
			{
				return AllowedCaller.Both;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002138 File Offset: 0x00000338
		public string Help
		{
			get
			{
				return "";
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002150 File Offset: 0x00000350
		public string Name
		{
			get
			{
				return "dsk";
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002168 File Offset: 0x00000368
		public List<string> Permissions
		{
			get
			{
				return new List<string>
				{
					"dsk"
				};
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000A RID: 10 RVA: 0x0000218C File Offset: 0x0000038C
		public string Syntax
		{
			get
			{
				return "<dsk>";
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000021A4 File Offset: 0x000003A4
		public void Execute(IRocketPlayer caller, string[] command)
		{
			UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;
			InteractableVehicle currentVehicle = unturnedPlayer.CurrentVehicle;
			bool flag = currentVehicle == null;
            if (flag)
            {
                UnturnedChat.Say(unturnedPlayer, Beta.Instance.Translate("err", new object[0]), Color.red);
            }
            else
            {
                bool flag2 = command.Length == 1;
                if (flag2)
                {
                    ushort num;
                    bool flag3 = ushort.TryParse(command[0], out num);
                    bool flag4 = flag3;
                    if (flag4)
                    {
                        VehicleManager.instance.channel.send("tellVehicleSkin", unturnedPlayer.CSteamID, ESteamPacket.CONNECTED, new object[]
                        {
                            currentVehicle.instanceID,
                            num,
                            0
                        });
                        UnturnedChat.Say(unturnedPlayer, Beta.Instance.Translate("ok", new object[0]), Color.yellow);
                    }
                    else
                    {
                        UnturnedChat.Say(unturnedPlayer, Beta.Instance.Translate("no", new object[0]), Color.red);
                    }
                }
                else
                {
                    bool flag5 = command.Length == 2;
                    if (flag5)
                    {
                        ushort num;
                        bool flag6 = ushort.TryParse(command[0], out num);
                        ushort num2;
                        bool flag7 = ushort.TryParse(command[1], out num2);
                        bool flag8 = flag6 && flag7;
                        if (flag8)
                        {
                            VehicleManager.instance.channel.send("tellVehicleSkin", unturnedPlayer.CSteamID, ESteamPacket.CONNECTED, new object[]
                            {
                                currentVehicle.instanceID,
                                num,
                                num2
                            });
                            UnturnedChat.Say(unturnedPlayer, Beta.Instance.Translate("ok", new object[0]), Color.yellow);
                        }
                        else
                        {
                            UnturnedChat.Say(unturnedPlayer, Beta.Instance.Translate("no", new object[0]), Color.red);
                        }
                    }
                    else
                    {
                        UnturnedChat.Say(unturnedPlayer, Beta.Instance.Translate("no", new object[0]), Color.red);
                    }
                }
                if (command.Length == 0)
                {
                    UnturnedChat.Say(unturnedPlayer, $"/{Name} <皮肤ID>", Color.red);
                }
            }
        }
    }
}
