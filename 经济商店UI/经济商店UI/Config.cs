using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace 经济商店UI
{
    public class Config : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            effectId = 2012;
        }
        [XmlElement("特效ID")]
        public ushort effectId;
    }
}
