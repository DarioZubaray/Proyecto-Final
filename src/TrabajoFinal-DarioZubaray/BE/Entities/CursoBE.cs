using System;
using System.Collections.Generic;

namespace BE.Entities
{
    public class CursoBE
    {
        #region Propiedades
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int AulaId { get; set; }
        public string AulaNombre { get; set; }
        public int? DiaSemana { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<UserBE> Docentes { get; set; }
        #endregion

        #region Constructor
        public CursoBE()
        {
            Docentes = new List<UserBE>();
        }

        public CursoBE(int id, string nombre, string descripcion, DateTime fechaInicio,
            DateTime fechaFin, int aulaId, int? diaSemana, TimeSpan? horaInicio, TimeSpan? horaFin,
            bool isActive, DateTime createdAt, DateTime lastUpdate)
        {
            Id = id;
            Nombre = nombre;
            Descripcion = descripcion;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            AulaId = aulaId;
            DiaSemana = diaSemana;
            HoraInicio = horaInicio;
            HoraFin = horaFin;
            IsActive = isActive;
            CreatedAt = createdAt;
            LastUpdate = lastUpdate;
            Docentes = new List<UserBE>();
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
