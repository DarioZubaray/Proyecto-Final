using System;
using System.Collections.Generic;

using BE.Entities;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class AttendanceBLL : IAttendanceBLL
    {
        #region Propiedades
        private readonly IAttendanceMPP _asistenciaMPP;
        #endregion

        #region Constructor
        public AttendanceBLL(IAttendanceMPP asistenciaMPP)
        {
            _asistenciaMPP = asistenciaMPP;
        }
        #endregion

        #region Métodos
        public bool RegistrarAsistenciaBulk(int cursoId, DateTime fecha, List<AttendanceBE> registros)
        {
            return _asistenciaMPP.RegistrarAsistenciaBulk(cursoId, fecha, registros);
        }

        public List<AttendanceBE> FindByCursoIdAndFecha(int cursoId, DateTime fecha)
        {
            return _asistenciaMPP.FindByCursoIdAndFecha(cursoId, fecha);
        }

        public bool EsDocenteDelCurso(int cursoId, int docenteId)
        {
            return _asistenciaMPP.EsDocenteDelCurso(cursoId, docenteId);
        }
        #endregion
    }
}
