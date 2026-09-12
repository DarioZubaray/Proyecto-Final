using System;

namespace BE.Entities
{
    public class EnrollmentBE
    {
        #region Propiedades
        public int Id { get; set; }
        public int CursoId { get; set; }
        public int AlumnoId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CursoNombre { get; set; }
        public string AlumnoNombre { get; set; }
        public int? DiaSemana { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public string AulaNombre { get; set; }
        #endregion

        #region Constructor
        public EnrollmentBE()
        {
        }
        #endregion
    }
}
