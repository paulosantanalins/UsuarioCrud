using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Base
{
    public class Credenciais
    {
        public static string Host { get; set; }
        public static int Port { get; set; }
        public static Boolean UseDefaultCredentials { get; set; }
        public static Boolean EnableSsl { get; set; }

        public string GetHost()
        {
            return Host;
        }
        public int GetPort()
        {
            return Port;
        }
        public Boolean GetUseDefaultCredentials()
        {
            return UseDefaultCredentials;
        }
        public Boolean GetEnableSsl()
        {
            return EnableSsl;
        }
    }
}
