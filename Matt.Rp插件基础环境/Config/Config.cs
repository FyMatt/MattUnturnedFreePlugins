using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Matt.Rp插件基础环境.LoaderConfig
{
    public class Config : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            MaineffectId = 2000;
            NoticeEffectId = 2001;
            staringGameback = "https://img1.imgtp.com/2023/01/16/eIvLWkxk.jpg";
            registeGameback = "https://img1.imgtp.com/2023/01/16/uqmZqB0q.jpg";

            address = "127.0.0.1";
            userName = "root";
            userPassword = "password";
            database = "unturned";
            port = "3306";

            emailAddress = "邮箱地址";
            emailPassword = "邮箱密钥";
            stmpIp = "smtp.qq.com";
            sendName = "发送邮件的标题";
            sendAddress = "邮箱地址";
        }
        [XmlElement("登录注册UI_ID")]
        public ushort MaineffectId;
        [XmlElement("通知UI_ID")]
        public ushort NoticeEffectId;
        [XmlElement("开始游戏界面背景图URL地址")]
        public string staringGameback;
        [XmlElement("注册界面背景图URL地址")]
        public string registeGameback;
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
        [XmlElement("邮箱用户地址")]
        public string emailAddress;
        [XmlElement("邮箱用户密码")]
        public string emailPassword;
        [XmlElement("邮箱stmpIp")]
        public string stmpIp;
        [XmlElement("显示邮件地址")]
        public string sendAddress;
        [XmlElement("显示邮件发送者名字")]
        public string sendName;
        public string getMySQLConnectionString => $"Data Source={address};Port={port};User ID={userName};Password={userPassword}; Initial Catalog={database};Charset=utf8; SslMode=none;Max pool size=10";
    }
}
