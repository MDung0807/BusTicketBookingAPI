﻿using BusBookTicket.Buses.DTOs.Requests;
using BusBookTicket.Buses.DTOs.Responses;
using BusBookTicket.Core.Common;

namespace BusBookTicket.Buses.Services.BusTypeServices;

public interface IBusTypeService : IService<BusTypeForm, BusTypeFormUpdate, int, BusTypeResponse>
{
    
}