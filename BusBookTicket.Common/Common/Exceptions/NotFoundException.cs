﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookTicket.Common.Common.Exceptions
{
    public class NotFoundException : ExceptionDetail
    {
        public NotFoundException(string message):base(message) { }
    }
}