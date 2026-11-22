using System.Collections.Generic;

using BE.Entities;

namespace BLL.Interfaces
{
    public interface ITicketBLL
    {
        bool Create(TicketBE ticket);
        List<TicketBE> FindAll();
        List<TicketBE> FindByUser(int userId);
        bool Close(int ticketId, int closedBy);
    }
}
