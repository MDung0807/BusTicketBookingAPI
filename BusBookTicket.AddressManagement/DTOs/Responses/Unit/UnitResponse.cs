﻿using BusBookTicket.AddressManagement.DTOs.Responses.Province;

namespace BusBookTicket.AddressManagement.DTOs.Responses.Unit;

public class UnitResponse
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public List<ProvinceResponse> Provinces { get; set; }
}