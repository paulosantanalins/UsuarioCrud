﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Domain
{
    public class EntityBase
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
