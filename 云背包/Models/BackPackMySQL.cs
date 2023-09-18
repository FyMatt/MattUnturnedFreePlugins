using SDG.Unturned;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 云背包.Models
{
    [SugarTable("matt_backpack_table")]
    public class BackPackMySQL
    {
        [SugarColumn(IsNullable = false, Length = 64)]
        public string steamId { get; set; }
        public ushort itemId { get; set; }
        public byte x { get; set; }
        public byte y { get; set; }
        public byte rot { get; set; }
        public byte amount { get; set; }
        public byte quality { get; set; }
        [SugarColumn(ColumnDataType = "blob")]
        public byte[] state { get; set; }

    }
}
