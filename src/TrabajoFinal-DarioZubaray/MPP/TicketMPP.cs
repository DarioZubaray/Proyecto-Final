using System;
using System.Collections.Generic;
using System.Data;

using BE.Entities;
using DAL;
using MPP.Interfaces;
using Microsoft.Data.SqlClient;

namespace MPP
{
    public class TicketMPP : ITicketMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public TicketMPP() : this(null) { }

        public TicketMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos
        public bool Insert(TicketBE ticket)
        {
            if (ticket == null)
            {
                return false;
            }

            string query = @"INSERT INTO tickets (user_id, subject, description, is_closed, created_at)
                            VALUES (@userId, @subject, @description, 0, @createdAt);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters =
            {
                new SqlParameter("@userId", ticket.UserId),
                new SqlParameter("@subject", ticket.Subject ?? (object)DBNull.Value),
                new SqlParameter("@description", ticket.Description ?? (object)DBNull.Value),
                new SqlParameter("@createdAt", ticket.CreatedAt)
            };

            var newId = _access.ReadScalar(query, parameters);

            if (newId > 0)
            {
                ticket.Id = newId;
                return true;
            }

            return false;
        }

        public List<TicketBE> FindAll()
        {
            string query = @"SELECT t.id, t.user_id, u.user_name, t.subject, t.description,
                                    t.is_closed, t.created_at, t.closed_at, t.closed_by,
                                    cu.user_name AS closed_by_name
                            FROM tickets t
                            INNER JOIN users u ON u.id = t.user_id
                            LEFT JOIN users cu ON cu.id = t.closed_by
                            ORDER BY t.created_at DESC";

            return MapTickets(_access.Read(query));
        }

        public List<TicketBE> FindByUser(int userId)
        {
            string query = @"SELECT t.id, t.user_id, u.user_name, t.subject, t.description,
                                    t.is_closed, t.created_at, t.closed_at, t.closed_by,
                                    cu.user_name AS closed_by_name
                            FROM tickets t
                            INNER JOIN users u ON u.id = t.user_id
                            LEFT JOIN users cu ON cu.id = t.closed_by
                            WHERE t.user_id = @userId
                            ORDER BY t.created_at DESC";

            SqlParameter[] parameters = { new SqlParameter("@userId", userId) };

            return MapTickets(_access.Read(query, parameters));
        }

        public bool Close(int ticketId, int closedBy, DateTime closedAt)
        {
            string query = @"UPDATE tickets
                            SET is_closed = 1, closed_at = @closedAt, closed_by = @closedBy
                            WHERE id = @id AND is_closed = 0";

            SqlParameter[] parameters =
            {
                new SqlParameter("@id", ticketId),
                new SqlParameter("@closedBy", closedBy),
                new SqlParameter("@closedAt", closedAt)
            };

            return _access.Save(query, parameters);
        }
        #endregion

        #region Métodos Privados
        private List<TicketBE> MapTickets(DataTable table)
        {
            var tickets = new List<TicketBE>();

            if (table == null)
            {
                return tickets;
            }

            foreach (DataRow row in table.Rows)
            {
                tickets.Add(MapTicket(row));
            }

            return tickets;
        }

        private TicketBE MapTicket(DataRow row)
        {
            return new TicketBE
            {
                Id = Convert.ToInt32(row["id"]),
                UserId = Convert.ToInt32(row["user_id"]),
                UserName = row["user_name"].ToString(),
                Subject = row["subject"].ToString(),
                Description = row["description"].ToString(),
                IsClosed = Convert.ToBoolean(row["is_closed"]),
                CreatedAt = (DateTime)row["created_at"],
                ClosedAt = row["closed_at"] == DBNull.Value ? (DateTime?)null : (DateTime)row["closed_at"],
                ClosedBy = row["closed_by"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["closed_by"]),
                ClosedByName = row["closed_by_name"] == DBNull.Value ? null : row["closed_by_name"].ToString()
            };
        }
        #endregion
    }
}
