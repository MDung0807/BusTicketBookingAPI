﻿namespace BusBookTicket.Core.Models.Entity;

public class Ward : BaseEntity
{
    #region -- Properties --

    public string FullName { get; set; }
    public string FullNameEnglish { get; set; }
    public string CodeName { get; set; }
    public string Name { get; set; }
    public string NameEnglish { get; set; }

    #endregion -- Properties --

    #region -- Relationship --
    public District District { get; set; }
    public AdministrativeUnit AdministrativeUnit { get; set; }
    public HashSet<Customer> Customers { get; set; }
    public HashSet<BusStation> BusStations { get; set; }
    public HashSet<Company> Companies { get; set; }
    #endregion -- Relationship --
}