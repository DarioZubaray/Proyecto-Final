using System;

namespace BE
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

        #endregion

        #region Constructores
        public UserBE() { }

        public UserBE(int pId, string pUserName, string pPasswordHash, bool pIsActive, int pRetriesCount, DateTime pLastUpdate, DateTime pCreatedAt)
        {
            this.Id = pId;
            this.UserName = pUserName;
            this.PasswordHash = pPasswordHash;
            this.IsActive = pIsActive;
            this.RetriesCount = pRetriesCount;
            this.LastUpdate = pLastUpdate;
            this.CreatedAt = pCreatedAt;
        }
        #endregion

        public override string ToString()
        {
            return $"{Id}, {UserName}, {IsActive}, {LastUpdate}";
        }
    }
}
