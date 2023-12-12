﻿namespace BusBookTicket.BillManage.DTOs.Responses;

public class BillResponse
{
    public string NameCustomer { get; set; }
    
    public DateTime DateDeparture { get; set; }
    
    public DateTime DateCreate { get; set; }
    public long TotalPrice { get; set; }
    public string BusStationStart { get; set; }
    public string BusStationEnd { get; set; }
    public string Discount { get; set; } 
    public List<BillItemResponse> Items { get; set; }
}
