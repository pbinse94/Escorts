﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.DTO
{
    public class EscortGalleryDto
    {
        public int ID { get; set; }
        public int EscortId { get; set; }
        public string? FileName { get; set; }
        public byte MediaType { get; set; }
    }
}
