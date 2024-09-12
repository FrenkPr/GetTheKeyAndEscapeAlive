using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TopDownGame
{
    static class XmlUtilities
    {
        public static string GetStringAttribute(XmlNode node, string attrName)
        {
            string value = "";
            XmlNode attrNode;

            if (node != null)
            {
                attrNode = node.Attributes.GetNamedItem(attrName);

                if (attrNode != null)
                {
                    value = attrNode.Value;
                }
            }

            return value;
        }

        public static int GetIntAttribute(XmlNode node, string attrName)
        {
            try
            {
                return int.Parse(GetStringAttribute(node, attrName));
            }

            catch
            {
                return 0;
            }
        }

        public static float GetFloatAttribute(XmlNode node, string attrName)
        {
            try
            {
                return float.Parse(GetStringAttribute(node, attrName));
            }

            catch
            {
                return 0;
            }
        }

        public static bool GetBoolAttribute(XmlNode node, string attrName)
        {
            try
            {
                return bool.Parse(GetStringAttribute(node, attrName));
            }

            catch
            {
                return false;
            }
        }
    }
}
