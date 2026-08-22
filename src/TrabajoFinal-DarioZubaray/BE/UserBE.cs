using System;

namespace BE
{
    public class UserBE
    {
        #region Properties
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public int RetriesCount { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RoleId { get; set; }
        public RoleBE Role { get; set; }
        public string Language { get; set; }
        #endregion

        #region Constructors
        public UserBE() { }

        public UserBE(
            int id, string userName, string passwordHash,
            bool isActive, int retriesCount, DateTime lastUpdate,
            DateTime createdAt, int roleId, string language)
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
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{Id}, {UserName}, {IsActive}, {LastUpdate}";
        }
        #endregion
    }
}
