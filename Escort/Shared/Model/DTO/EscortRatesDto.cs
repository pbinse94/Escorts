﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.DTO
{
    public class EscortRatesDto
    {
        public int Id { get; set; }
        public int EscortId { get; set; }
        public int? Duration { get; set; }
        public decimal? InCallRate { get; set; }
        public decimal? OutCallRate { get; set; }
    }
}
