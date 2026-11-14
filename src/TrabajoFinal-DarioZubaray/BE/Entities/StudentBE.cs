using System;

namespace BE.Entities
{
    public class StudentBE
    {
        #region Propiedades
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdate { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        #endregion

        #region Constructor
        public StudentBE() { }

        public StudentBE(int id, string firstName, string lastName, string document, string email, string phone, bool isActive, DateTime createdAt, DateTime lastUpdate)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Document = document;
            Email = email;
            Phone = phone;
            IsActive = isActive;
            CreatedAt = createdAt;
            LastUpdate = lastUpdate;
        }
        #endregion

        #region Métodos
        public override string ToString()
        {
            return FullName;
        }
        #endregion
    }
}
