using System;

namespace GestaoServico.Api.ViewModels
{
    public abstract class ControleEdicaoVM
    {
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
