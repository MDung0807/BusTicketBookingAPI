﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookTicket.Models.Entity
{
    public class Review
    {
        public int reviewID { get; set; }
        public string content { get; set; }
        public DateTime dateCreate { get; set; }
        public DateTime dateUpdate { get; set; }
        public string? image { get; set; }
        public int rate { get; set; }

        public int customerID { get; set; }
        public int busID { get; set; }
    }
}
