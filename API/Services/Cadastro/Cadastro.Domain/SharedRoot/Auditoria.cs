namespace Cadastro.Domain.SharedRoot
{
    public class Auditoria : EntityBase
    {
        public string Tabela { get; set; }
        public string IdsAlterados { get; set; }
        public string ValoresAntigos { get; set; }
        public string ValoresNovos { get; set; }
    }
}
