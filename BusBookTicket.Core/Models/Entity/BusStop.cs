﻿namespace BusBookTicket.Core.Models.Entity
{
    public class BusStop : BaseEntity
    {
        #region -- Relation ship

        public BusStation? BusStation { get; set; }
        public Bus? Bus { get; set; }
        public HashSet<Ticket_BusStop> TicketBusStops { get; set; }

        #endregion
    }
}
