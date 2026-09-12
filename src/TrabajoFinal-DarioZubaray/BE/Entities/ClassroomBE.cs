using System;

namespace BE.Entities
{
    public class ClassroomBE
    {
        #region Propiedades
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Capacidad { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdate { get; set; }
        #endregion

        #region Constructor
        public ClassroomBE() { }

        public ClassroomBE(int id, string nombre, int capacidad, bool isActive, DateTime createdAt, DateTime lastUpdate)
        {
            Id = id;
            Nombre = nombre;
            Capacidad = capacidad;
            IsActive = isActive;
            CreatedAt = createdAt;
            LastUpdate = lastUpdate;
        }
        #endregion

        #region Métodos
        public override string ToString()
        {
            return Nombre;
        }
        #endregion
    }
}
