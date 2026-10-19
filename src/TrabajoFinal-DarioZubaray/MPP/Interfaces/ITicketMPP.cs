using System.Collections.Generic;

using BE.Entities;

namespace MPP.Interfaces
{
    public interface ITicketMPP
    {
        bool Insert(TicketBE ticket);
        List<TicketBE> FindAll();
        List<TicketBE> FindByUser(int userId);
        bool Close(int ticketId, int closedBy, System.DateTime closedAt);
    }
}
