using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.LinkRoot.Entity
{
    public class Link : EntityBase
    {
        public string Nome { get; set; }
        public string Url { get; set; }
    }
}
