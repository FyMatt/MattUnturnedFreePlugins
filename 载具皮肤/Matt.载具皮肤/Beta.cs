using System;
using System.Collections.Generic;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Steamworks;

namespace Matt.载具皮肤
{
	// Token: 0x02000002 RID: 2
	public class Beta : RocketPlugin
	{
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        protected override void Load()
        {
            Logger.LogWarning($"欢迎使用 {Assembly.GetName().Name} 版本：{Assembly.GetName().Version}");
            Logger.LogWarning("Matt插件群：656525189");
            Beta.Instance = this;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002085 File Offset: 0x00000285
		protected override void Unload()
		{

		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x000020B4 File Offset: 0x000002B4
		public override TranslationList DefaultTranslations
		{
			get
			{
				TranslationList translationList = new TranslationList();
				translationList.Add("ok", "载具皮肤已经修改成功!");
				translationList.Add("no", "无效的参数!");
				translationList.Add("err", "请在车内使用该指令!");
				return translationList;
			}
		}

		// Token: 0x04000001 RID: 1
		public static Beta Instance;
	}
}
