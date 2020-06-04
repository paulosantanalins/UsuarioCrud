using System;
using System.Collections.Generic;

namespace Utils
{
    public class Variables
    {
        public static string EnvironmentName { get; set; }
        public static string DefaultConnection { get; set; }
        public static string Host { get; set; }
        public static int Port { get; set; }
        public static Boolean UseDefaultCredentials { get; set; }
        public static Boolean EnableSsl { get; set; }

        public void SetEnvironmentName(string environmentName)
        {
            EnvironmentName = environmentName;
        }

        public string GetEnvironmentName()
        {
            return EnvironmentName;
        }

        public void SetDefaultConnection(string defaultConnection)
        {
            DefaultConnection = defaultConnection;
        }

        public string GetDefaultConnection()
        {
            return DefaultConnection;
        }
    }
}
