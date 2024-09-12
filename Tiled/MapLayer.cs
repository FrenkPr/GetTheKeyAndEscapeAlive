using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TopDownGame
{
    class MapLayer
    {
        public string[] IDs { get; }

        public MapLayer(XmlNode layerNode)
        {
            XmlNode dataNode = layerNode.SelectSingleNode("data");
            string csvData = dataNode.InnerText;
            csvData = csvData.Replace("\r\n", "").Replace("\n", "").Replace(" ", "").Replace("\t", "");

            string[] ids = csvData.Split(',');
            IDs = ids;
        }
    }
}
