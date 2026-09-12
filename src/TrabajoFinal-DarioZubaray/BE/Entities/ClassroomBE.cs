using System;

namespace BE.Entities
{
    public class ClassroomBE
    {
        #region Propiedades
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdate { get; set; }
        #endregion

        #region Constructor
        public ClassroomBE() { }

        public ClassroomBE(int id, string name, int capacity, bool isActive, DateTime createdAt, DateTime lastUpdate)
        {
            Id = id;
            Name = name;
            Capacity = capacity;
            IsActive = isActive;
            CreatedAt = createdAt;
            LastUpdate = lastUpdate;
        }
        #endregion

        #region Métodos
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
