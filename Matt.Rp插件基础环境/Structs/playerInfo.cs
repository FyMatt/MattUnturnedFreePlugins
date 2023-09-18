using FreeSql.DataAnnotations;
using Matt.Rp插件基础环境.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matt.Rp插件基础环境.Structs
{
    public class NsRpplayerInfo
    {
        [Column(IsPrimary = true)]
        public string steamId { get; set; } = string.Empty;//玩家id
        public string displayName { get; set; } = string.Empty;//玩家名称
        public ECareer? career { get; set; } = null;//玩家职业
        public string qq { get; set; } = string.Empty;//QQ
        public string emailAddress { get; set; } = string.Empty;//邮箱地址
        public DateTime? createTime { get; set; } = null;//注册的时间
    }
}
