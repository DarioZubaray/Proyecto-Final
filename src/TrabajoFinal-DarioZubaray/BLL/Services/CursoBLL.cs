using System;
using System.Collections.Generic;
using BE.Entities;
using BLL.Interfaces;
using MPP;

namespace BLL.Services
{
    public class CursoBLL : ICursoBLL
    {
        #region Propiedades
        private readonly ICursoMPP _cursoMPP;
        #endregion

        #region Constructor
        public CursoBLL(ICursoMPP cursoMPP)
        {
            _cursoMPP = cursoMPP;
        }
        #endregion

        #region Métodos
        public bool Save(CursoBE curso)
        {
            bool hayTraslape = _cursoMPP.ExisteTraslapeAula(
                curso.AulaId,
                curso.FechaInicio,
                curso.FechaFin,
                curso.Id,
                curso.DiaSemana,
                curso.HoraInicio,
                curso.HoraFin
            );

            if (hayTraslape)
            {
                throw new InvalidOperationException("El aula ya está ocupada en ese horario.");
            }

            return _cursoMPP.Save(curso);
        }

        public bool SaveDocentes(int cursoId, List<int> docenteIds)
        {
            return _cursoMPP.SaveDocentes(cursoId, docenteIds);
        }

        public bool ValidarTraslapeDocentes(int cursoId, int? diaSemana, TimeSpan? horaInicio, TimeSpan? horaFin, List<int> docenteIds)
        {
            if (!diaSemana.HasValue || !horaInicio.HasValue || !horaFin.HasValue)
            {
                return false;
            }

            foreach (int docenteId in docenteIds)
            {
                if (_cursoMPP.ExisteTraslapeDocente(docenteId, diaSemana.Value, horaInicio.Value, horaFin.Value, cursoId))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Delete(CursoBE curso)
        {
            return _cursoMPP.Delete(curso);
        }

        public CursoBE FindById(int id)
        {
            return _cursoMPP.FindById(id);
        }

        public List<CursoBE> FindAll()
        {
            return _cursoMPP.FindAll();
        }

        public List<CursoBE> FindByName(string nombre)
        {
            return _cursoMPP.FindByName(nombre);
        }

        public bool ExisteTraslapeAula(int aulaId, DateTime fechaInicio, DateTime fechaFin, int cursoIdExcluir, int? diaSemana, TimeSpan? horaInicio, TimeSpan? horaFin)
        {
            return _cursoMPP.ExisteTraslapeAula(aulaId, fechaInicio, fechaFin, cursoIdExcluir, diaSemana, horaInicio, horaFin);
        }

        public List<UserBE> GetDocentesByCursoId(int cursoId)
        {
            return _cursoMPP.GetDocentesByCursoId(cursoId);
        }

        public List<CursoBE> FindByDocenteId(int docenteId)
        {
            return _cursoMPP.FindByDocenteId(docenteId);
        }

        public List<CursoBE> FindByAlumnoId(int alumnoId)
        {
            return _cursoMPP.FindByAlumnoId(alumnoId);
        }
        #endregion
    }
}
