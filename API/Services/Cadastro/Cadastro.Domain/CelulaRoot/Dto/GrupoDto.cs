﻿using System;

namespace Cadastro.Domain.CelulaRoot.Entity
{
    public class GrupoDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }

        public string Descricao { get; set; }
        
        public bool Ativo { get; set; }
    }
}
