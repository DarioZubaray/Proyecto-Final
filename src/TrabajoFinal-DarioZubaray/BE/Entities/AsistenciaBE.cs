using System;

namespace BE.Entities
{
    public class AsistenciaBE
    {
        #region Propiedades
        public int Id { get; set; }
        public int CursoId { get; set; }
        public int AlumnoId { get; set; }
        public DateTime Fecha { get; set; }
        public bool Presente { get; set; }
        public DateTime CreatedAt { get; set; }

        public string AlumnoNombre { get; set; }
        public string CursoNombre { get; set; }
        #endregion

        #region Constructor
        public AsistenciaBE()
        {
        }
        #endregion
    }
}
