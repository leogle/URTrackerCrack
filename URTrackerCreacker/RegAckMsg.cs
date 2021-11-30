using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace URTrackerCrack
{
    public class RegAckMsg
    {
        private string checkMessage = string.Empty;
        private string customerName = string.Empty;
        private int allowUserCount = 5;
        private DateTime expireDate = DateTime.MinValue;
        private DateTime nextCheckTime = DateTime.MinValue;
        private int checkResult;
        private int functionLevel;

        public int CheckResult
        {
            get
            {
                return this.checkResult;
            }
            set
            {
                this.checkResult = value;
            }
        }

        public string CheckMessage
        {
            get
            {
                return this.checkMessage;
            }
            set
            {
                this.checkMessage = value;
            }
        }

        public string CustomerName
        {
            get
            {
                return this.customerName;
            }
            set
            {
                this.customerName = value;
            }
        }

        public int AllowedUserCount
        {
            get
            {
                return this.allowUserCount;
            }
            set
            {
                this.allowUserCount = value;
            }
        }

        public int FunctionLevel
        {
            get
            {
                return this.functionLevel;
            }
            set
            {
                this.functionLevel = value;
            }
        }

        public DateTime ExpireDate
        {
            get
            {
                return this.expireDate;
            }
            set
            {
                this.expireDate = value;
            }
        }

        public DateTime NextCheckTime
        {
            get
            {
                return this.nextCheckTime;
            }
            set
            {
                this.nextCheckTime = value;
            }
        }

        public string toXml()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RegAckMsg));
            StringBuilder sb = new StringBuilder();
            StringWriter stringWriter = new StringWriter(sb);
            xmlSerializer.Serialize((TextWriter)stringWriter, (object)this);
            return sb.ToString();
        }

        public string toBase64()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(this.toXml()));
        }

        public static RegAckMsg ToObject(string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RegAckMsg));
            StringReader stringReader = new StringReader(xmlString);
            object obj = xmlSerializer.Deserialize((TextReader)stringReader);
            stringReader.Close();
            return (RegAckMsg)obj;
        }

        public static RegAckMsg ToResgAckMsg(string base64Code)
        {
            return RegAckMsg.ToObject(Encoding.UTF8.GetString(Convert.FromBase64String(base64Code)));
        }
    }
}
