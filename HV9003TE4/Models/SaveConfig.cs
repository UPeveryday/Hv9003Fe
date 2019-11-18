using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Configuration;

namespace HV9003TE4.Models
{
    public static class XmlConfig
    {
        /// <summary>
        /// 更新ADD节点的值
        /// </summary>
        /// <param name="ConnenctionString"></param>
        /// <param name="strKey"></param>
        public static void UpdataAllAddNodeConfigXml(string ConnenctionString, string strKey)
        {
            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径  
            string strFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            // string  strFileName= AppDomain.CurrentDomain.BaseDirectory + "\\exe.config";  
            doc.Load(strFileName);
            //找出名称为“add”的所有元素  
            XmlNodeList nodes = doc.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性  
                XmlAttribute att = nodes[i].Attributes["key"];
                //根据元素的第一个属性来判断当前的元素是不是目标元素  
                if (att.Value == strKey)
                {
                    //对目标元素中的第二个属性赋值  
                    att = nodes[i].Attributes["value"];
                    att.Value = ConnenctionString;
                    break;
                }
            }
            //保存上面的修改  
            doc.Save(strFileName);
            ConfigurationManager.RefreshSection("appSettings");
        }
        /// <summary>
        /// 读取key的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAddNodeValue(string key)
        {
            return ConfigurationManager.AppSettings.GetValues(key)[0];
        }

        /// <summary>
        /// 获取所有ADD的值
        /// </summary>
        /// <returns></returns>
        public static XmlConfigAdd GetAllXmlItem()
        {
            //判断App.config配置文件中是否有Key(非null) 
            if (ConfigurationManager.AppSettings.HasKeys())
            {
                List<string> theKeys = new List<string>(); //保存Key的集合 
                List<string> theValues = new List<string>(); //保存Value的集合 
                //遍历出所有的Key并添加进theKeys集合 
                foreach (string theKey in ConfigurationManager.AppSettings.Keys)
                {
                    theKeys.Add(theKey);
                }
                //根据Key遍历出所有的Value并添加进theValues集合 
                for (int i = 0; i < theKeys.Count; i++)
                {
                    foreach (string theValue in ConfigurationManager.AppSettings.GetValues(theKeys[i]))
                    {
                        theValues.Add(theValue);
                    }
                }
                return new XmlConfigAdd() { theKeys = theKeys, theValues = theValues };

            }
            return new XmlConfigAdd();
        }

        public struct XmlConfigAdd
        {
            public List<string> theKeys; //保存Key的集合 
            public List<string> theValues;
        }
    }
}
