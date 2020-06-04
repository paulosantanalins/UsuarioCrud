using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.Core.Notifications
{
    public struct Notification
    {
        public string _key;
        public string _value;

        public Notification(string key, string value)
        {
            _key = key;
            _value = value;
        }
    }
}
