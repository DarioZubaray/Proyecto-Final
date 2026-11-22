using System;
using System.Collections.Generic;

using BE.Entities;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class TicketBLL : ITicketBLL
    {
        #region Propiedades
        private readonly ITicketMPP _ticketMPP;
        #endregion

        #region Constructor
        public TicketBLL(ITicketMPP ticketMPP)
        {
            _ticketMPP = ticketMPP;
        }
        #endregion

        #region Métodos
        public bool Create(TicketBE ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            if (string.IsNullOrWhiteSpace(ticket.Subject))
            {
                throw new InvalidOperationException("El asunto del ticket es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(ticket.Description))
            {
                throw new InvalidOperationException("La descripción del ticket es obligatoria.");
            }

            ticket.Subject = ticket.Subject.Trim();
            ticket.Description = ticket.Description.Trim();
            ticket.IsClosed = false;
            ticket.CreatedAt = DateTime.Now;
            ticket.ClosedAt = null;
            ticket.ClosedBy = null;

            return _ticketMPP.Insert(ticket);
        }

        public List<TicketBE> FindAll()
        {
            return _ticketMPP.FindAll();
        }

        public List<TicketBE> FindByUser(int userId)
        {
            return _ticketMPP.FindByUser(userId);
        }

        public bool Close(int ticketId, int closedBy)
        {
            if (ticketId <= 0 || closedBy <= 0)
            {
                return false;
            }

            return _ticketMPP.Close(ticketId, closedBy, DateTime.Now);
        }
        #endregion
    }
}
