using System;

namespace BE.Entities
{
    public class TicketBE
    {
        #region Propiedades
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public bool IsClosed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int? ClosedBy { get; set; }
        public string ClosedByName { get; set; }
        #endregion

        #region Constructor
        public TicketBE() { }

        public TicketBE(int id, int userId, string subject, string description, bool isClosed, DateTime createdAt, DateTime? closedAt, int? closedBy)
        {
            Id = id;
            UserId = userId;
            Subject = subject;
            Description = description;
            IsClosed = isClosed;
            CreatedAt = createdAt;
            ClosedAt = closedAt;
            ClosedBy = closedBy;
        }
        #endregion

        #region Métodos
        public override string ToString()
        {
            return Subject;
        }
        #endregion
    }
}
