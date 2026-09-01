using System;

namespace BE.Entities
{
    public class UserBE
    {
        #region Propiedades
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public int RetriesCount { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RoleId { get; set; }
        public string Language { get; set; }
        public string Theme { get; set; }
        #endregion

        #region Constructor
        public UserBE() { }

        public UserBE(
            int id, string userName, string passwordHash,
            bool isActive, int retriesCount, DateTime lastUpdate,
            DateTime createdAt, int roleId, string language)
            : this(id, userName, passwordHash, isActive, retriesCount,
                   lastUpdate, createdAt, roleId, language, null)
        {
        }

        public UserBE(
            int id, string userName, string passwordHash,
            bool isActive, int retriesCount, DateTime lastUpdate,
            DateTime createdAt, int roleId, string language, string theme)
        {
            Id = id;
            UserName = userName;
            PasswordHash = passwordHash;
            IsActive = isActive;
            RetriesCount = retriesCount;
            LastUpdate = lastUpdate;
            CreatedAt = createdAt;
            RoleId = roleId;
            Language = language;
            Theme = theme;
        }
        #endregion

        #region Métodos
        public override string ToString()
        {
            return $"{Id}, {UserName}, {IsActive}, {LastUpdate}";
        }
        #endregion
    }
}
