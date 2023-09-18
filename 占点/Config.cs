using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using static SDG.Unturned.WeatherAsset;

namespace 占点
{
    public class Config : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            effectId = 2011;
            startNeedPlayerNum = 1;
            addCountDownTime = 30;
            countDownTime = 60;
            min_uconomy = 500.0f;
            max_uconomy = 1000.0f;
            pointsList.Add(new dian { name = "飞机场", range = 5, count = 1, position = new point { x = 747, y = 46, z = 647 }, itemsConfig = new List<ZhandianItems> { new ZhandianItems { id = 363, 数量 = 1 }, new ZhandianItems { id = 17, 数量 = 2 } } });
            pointsList.Add(new dian { name = "监狱岛", range = 5, count = 1, position = new point { x = -246, y = 40, z = 29 }, itemsConfig = new List<ZhandianItems> { new ZhandianItems { id = 122, 数量 = 1 }, new ZhandianItems { id = 125, 数量 = 2 } } });
        }
        [XmlElement("特效ID")]
        public ushort effectId;
        /// <summary>
        /// 开始占点活动人数
        /// </summary>
        [XmlElement("开始占点活动人数")]
        public int startNeedPlayerNum;
        /// <summary>
        /// 占点所需时间_秒
        /// </summary>
        [XmlElement("占点延续时长_秒")]
        public int addCountDownTime;
        /// <summary>
        /// 占点倒计时_秒
        /// </summary>
        [XmlElement("占点倒计时_秒")]
        public int countDownTime;
        [XmlElement("经济奖励最小值")]
        public float min_uconomy;
        [XmlElement("经济奖励最大值")]
        public float max_uconomy;
        /// <summary>
        /// 据点列表
        /// </summary>
        [XmlArray("据点列表"),XmlArrayItem("据点信息")]
        public List<dian> pointsList = new List<dian> { };
    }
    public class dian
    {
        [XmlAttribute("据点名称")]
        public string name;

        [XmlAttribute("据点范围")]
        public float range;

        [XmlAttribute("抽取奖励个数")]
        public int count;

        [XmlElement("坐标")]
        public point position;

        [XmlArrayItem("奖励设置"),XmlArray("奖励列表")]
        public List<ZhandianItems> itemsConfig;
    }
    public class ZhandianItems
    {
        [XmlAttribute("物品ID")]
        public ushort id;

        [XmlAttribute("物品数量")]
        public byte 数量;
    }
    public class point
    {
        [XmlAttribute("x")]
        public float x;
        [XmlAttribute("y")]
        public float y;
        [XmlAttribute("z")]
        public float z;
    }
}
