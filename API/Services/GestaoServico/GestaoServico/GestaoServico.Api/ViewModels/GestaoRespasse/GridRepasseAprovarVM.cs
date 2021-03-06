﻿using System;

namespace GestaoServico.Api.ViewModels.GestaoRespasse
{
    public class GridRepasseAprovarVM
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int Destino { get; set; }
        public int Origem { get; set; }
        public string Cliente { get; set; }
        public string Projeto { get; set; }
        public int? Horas { get; set; }
        public decimal? VlUnitario { get; set; }
        public decimal? VlRepasse { get; set; }
        public string Aprovado { get; set; }
        public string Motivo { get; set; }
        public bool Desabilita { get; set; }

        public string Usuario { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}
