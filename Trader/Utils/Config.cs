using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi.V1;

namespace Trader.Utils
{
    public class Config
    {
        private XmlDocument doc;
        private string path;

        public Config(string name)
        {
            path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\config\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += name + ".cfg";
            doc = new XmlDocument();
            if (!File.Exists(path))
            {
                doc.LoadXml("<xml><body></body></xml>");
            }
            else
            {
                doc.Load(path);
            }
        }

        #region string
        public string GetVal(string key, string val = "")
        {
            XmlNode body = doc.DocumentElement.SelectSingleNode("body");
            if(body == null)
            {
                body = doc.CreateElement("body");
                doc.DocumentElement.AppendChild(body);
            }
            if(body.Attributes[key] == null)
            {
                body.Attributes.Append(doc.CreateAttribute(key));
                body.Attributes[key].Value = val;
            }
            return body.Attributes[key].Value;
        }

        public string GetVal(string sec, string key, string val = "")
        {
            XmlNode body = doc.DocumentElement.SelectSingleNode("body");
            XmlNode item = body.SelectSingleNode("item[@Name='" + sec + "']");
            if (item == null)
            {
                item = body.AppendChild(doc.CreateElement("item"));
                item.Attributes.Append(doc.CreateAttribute("Name")).Value = sec;
                item.Attributes.Append(doc.CreateAttribute(key)).Value = val;
                return val;
            }
            else
            {
                if (item.Attributes[key] == null)
                {
                    item.Attributes.Append(doc.CreateAttribute(key)).Value = val;
                    return val;
                }
                else return item.Attributes[key].Value;
            }
        }

        public void SetVal(string key, string val)
        {
            XmlNode body = doc.DocumentElement.SelectSingleNode("body");
            if (body == null)
            {
                body = doc.CreateElement("body");
                doc.DocumentElement.AppendChild(body);
            }
            if (body.Attributes[key] == null)
            {
                body.Attributes.Append(doc.CreateAttribute(key));
            }
            body.Attributes[key].Value = val;
        }

        public void SetVal(string sec, string key, string val)
        {
            XmlNode body = doc.DocumentElement.SelectSingleNode("body");
            if (body == null)
            {
                body = doc.CreateElement("body");
                doc.DocumentElement.AppendChild(body);
                XmlNode item = body.AppendChild(doc.CreateElement("item"));
                item.Attributes.Append(doc.CreateAttribute("Name")).Value = sec;
                item.Attributes.Append(doc.CreateAttribute(key)).Value = val;
                return;
            }
            else
            {
                XmlNode item = body.SelectSingleNode("item[@Name='" + sec +"']");
                if(item == null)
                {
                    item = body.AppendChild(doc.CreateElement("item"));
                    item.Attributes.Append(doc.CreateAttribute("Name")).Value = sec;
                }
                if (item.Attributes[key] == null) item.Attributes.Append(doc.CreateAttribute(key)).Value = val;
                else item.Attributes[key].Value = val;
            }
        }
        #endregion

        #region int
        public int GetVal(string key, int val = 0)
        {
            string outval = GetVal(key, val.ToString());
            return int.Parse(outval);
        }
        public int GetVal(string sec, string key, int val = 0)
        {
            string outval = GetVal(sec, key, val.ToString());
            return int.Parse(outval);
        }
        public void SetVal(string key, int val)
        {
            SetVal(key, val.ToString());
        }
        public void SetVal(string sec, string key, int val)
        {
            SetVal(sec, key, val.ToString());
        }
        #endregion

        #region decimal
        public decimal GetVal(string key, decimal val = 0)
        {
            string outval = GetVal(key, val.ToString());
            return decimal.Parse(outval);
        }
        public decimal GetVal(string sec, string key, decimal val = 0)
        {
            string outval = GetVal(sec, key, val.ToString());
            return decimal.Parse(outval);
        }
        public void SetVal(string key, decimal val)
        {
            SetVal(key, val.ToString());
        }
        public void SetVal(string sec, string key, decimal val)
        {
            SetVal(sec, key, val.ToString());
        }
        #endregion

        #region double
        public double GetVal(string key, double val = 0)
        {
            string outval = GetVal(key, val.ToString());
            return double.Parse(outval);
        }
        public double GetVal(string sec, string key, double val = 0)
        {
            string outval = GetVal(key, val.ToString());
            return double.Parse(outval);
        }
        public void SetVal(string key, double val)
        {
            SetVal(key, val.ToString());
        }
        public void SetVal(string sec, string key, double val)
        {
            SetVal(sec, key, val.ToString());
        }

        #endregion

        #region float
        public float GetVal(string key, float val = 0.0f)
        {
            string outval = GetVal(key, val.ToString());
            return float.Parse(outval);
        }
        public float GetVal(string sec, string key, float val = 0.0f)
        {
            string outval = GetVal(sec, key, val.ToString());
            return float.Parse(outval);
        }
        public void SetVal(string key, float val)
        {
            SetVal(key, val.ToString());
        }
        public void SetVal(string sec, string key, float val)
        {
            SetVal(sec, key, val.ToString());
        }
        #endregion

        #region long
        public long GetVal(string key, long val = 0)
        {
            string outval = GetVal(key, val.ToString());
            return long.Parse(outval);
        }
        public long GetVal(string sec, string key, long val = 0)
        {
            string outval = GetVal(sec, key, val.ToString());
            return long.Parse(outval);
        }
        public void SetVal(string key, long val)
        {
            SetVal(key, val.ToString());
        }
        public void SetVal(string sec, string key, long val)
        {
            SetVal(sec, key, val.ToString());
        }
        #endregion

        #region bool
        public bool GetVal(string key, bool val = false)
        {
            string outval = GetVal(key, "false");
            return bool.Parse(outval);
        }
        public bool GetVal(string sec, string key, bool val = false)
        {
            string outval = GetVal(sec, key, "false");
            return bool.Parse(outval);
        }
        public void SetVal(string key, bool val)
        {
            SetVal(key, val.ToString());
        }
        public void SetVal(string sec, string key, bool val)
        {
            SetVal(sec, key, val.ToString());
        }
        #endregion

        #region CandleInterval
        public CandleInterval GetVal(string key, CandleInterval val = CandleInterval.Unspecified)
        {
            string outval = GetVal(key, val.ToString());
            switch (outval)
            {
                case "_1Min":
                    return CandleInterval._1Min;
                case "_5Min":
                    return CandleInterval._5Min;
                case "_15Min":
                    return CandleInterval._15Min;
                case "Hour":
                    return CandleInterval.Hour;
                case "Day":
                    return CandleInterval.Day;
            }
            return CandleInterval.Unspecified;
        }
        public CandleInterval GetVal(string sec, string key, CandleInterval val = CandleInterval.Unspecified)
        {
            string outval = GetVal(sec, key, val.ToString());
            switch (outval)
            {
                case "_1Min":
                    return CandleInterval._1Min;
                case "_5Min":
                    return CandleInterval._5Min;
                case "_15Min":
                    return CandleInterval._15Min;
                case "Hour":
                    return CandleInterval.Hour;
                case "Day":
                    return CandleInterval.Day;
            }
            return CandleInterval.Unspecified;
        }
        public void SetVal(string key, CandleInterval val)
        {
            SetVal(key, val.ToString());
        }
        public void SetVal(string sec, string key, CandleInterval val)
        {
            SetVal(sec, key, val.ToString());
        }
        #endregion

        #region TimeSpan
        public TimeSpan GetVal(string key, TimeSpan val)
        {
            string outval = GetVal(key, val.ToString());
            return TimeSpan.Parse(outval);
        }
        public TimeSpan GetVal(string sec, string key, TimeSpan val)
        {
            string outval = GetVal(sec, key, val.ToString());
            return TimeSpan.Parse(outval);
        }
        public void SetVal(string key, TimeSpan val)
        {
            SetVal(key, val.ToString());
        }
        public void SetVal(string sec, string key, TimeSpan val)
        {
            SetVal(sec, key, val.ToString());
        }
        #endregion

        #region ServerType
        public Network.ServerMode GetVal(string key, Network.ServerMode val)
        {
            string outval = GetVal(key, val.ToString());
            switch (outval)
            {
                case "FullAccess":
                    return Network.ServerMode.FullAccess;
                case "ReadOnly":
                    return Network.ServerMode.ReadOnly;
                default:
                    return Network.ServerMode.Test;
            }
        }
        public Network.ServerMode GetVal(string sec, string key, Network.ServerMode val)
        {
            string outval = GetVal(sec, key, val.ToString());
            switch (outval)
            {
                case "FullAccess":
                    return Network.ServerMode.FullAccess;
                case "ReadOnly":
                    return Network.ServerMode.ReadOnly;
                default:
                    return Network.ServerMode.Test;
            }
        }
        public void SetVal(string key, Network.ServerMode val)
        {
            SetVal(key, val.ToString());
        }
        public void SetVal(string sec, string key, Network.ServerMode val)
        {
            SetVal(sec, key, val.ToString());
        }
        #endregion

        public void Save()
        {
            doc.Save(path);
        }
    }
}
