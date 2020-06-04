using System.Collections.Generic;

namespace Cadastro.Domain.SharedRoot
{
    public class LdapSeguranca
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string Search_Base { get; set; }
        public string Login { get; set; }
        public string LoginAD { get; set; }
        public string Password { get; set; }
        public List<string> SearchEmails { get; set; }
    }
}
