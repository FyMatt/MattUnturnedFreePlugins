using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace 云背包
{
    public class BackPackConfig : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            address = "127.0.0.1";
            userName= "root";
            userPassword = "password";
            database = "unturned";
            port = "3306";
            groupConfigs.Add(new BackPackGroupConfig { groupName = "default", max_x = 5, max_y = 5 });
            groupConfigs.Add(new BackPackGroupConfig { groupName = "vip", max_x = 10, max_y = 10 });
            groupConfigs.Add(new BackPackGroupConfig { groupName = "svip", max_x = 15, max_y = 15 });
            groupConfigs.Add(new BackPackGroupConfig { groupName = "admin", max_x = 20, max_y = 20 });
        }
        [XmlElement("数据库地址")]
        public string address;
        [XmlElement("数据库用户名")]
        public string userName;
        [XmlElement("数据库密码")]
        public string userPassword;
        [XmlElement("库名")]
        public string database;
        [XmlElement("数据库端口")]
        public string port;
        public string getMySQLConnectionString => $"Data Source={address};Port={port};User ID={userName};Password={userPassword}; Initial Catalog={database};Charset=utf8; SslMode=none;Max pool size=10";
        [XmlArray("云背包设置"), XmlArrayItem("背包")]
        public List<BackPackGroupConfig> groupConfigs = new List<BackPackGroupConfig> { };
    }
    public class BackPackGroupConfig
    {
        [XmlAttribute("权限组名称")]
        public string groupName { get; set; }
        [XmlAttribute("背包长度")]
        public byte max_x { get; set; }
        [XmlAttribute("背包宽度")]
        public byte max_y { get; set; }
    }
}
