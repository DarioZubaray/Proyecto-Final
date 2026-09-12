using System;
using System.Collections.Generic;
using BE.Entities;
using BLL.Interfaces;
using MPP;

namespace BLL.Services
{
    public class AsistenciaBLL : IAsistenciaBLL
    {
        #region Propiedades
        private readonly IAsistenciaMPP _asistenciaMPP;
        #endregion

        #region Constructor
        public AsistenciaBLL(IAsistenciaMPP asistenciaMPP)
        {
            _asistenciaMPP = asistenciaMPP;
        }
        #endregion

        #region Métodos
        public bool RegistrarAsistenciaBulk(int cursoId, DateTime fecha, List<AsistenciaBE> registros)
        {
            return _asistenciaMPP.RegistrarAsistenciaBulk(cursoId, fecha, registros);
        }

        public List<AsistenciaBE> FindByCursoIdAndFecha(int cursoId, DateTime fecha)
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
