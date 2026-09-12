using System;
using System.Collections.Generic;

using BE.Entities;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class EnrollmentBLL : IEnrollmentBLL
    {
        #region Propiedades
        private readonly IEnrollmentMPP _inscripcionMPP;
        private readonly ICourseBLL _cursoBLL;
        #endregion

        #region Constructor
        public EnrollmentBLL(IEnrollmentMPP inscripcionMPP, ICourseBLL cursoBLL)
        {
            _inscripcionMPP = inscripcionMPP;
            _cursoBLL = cursoBLL;
        }
        #endregion

        #region Métodos
        public bool Inscribir(int cursoId, int alumnoId)
        {
            if (_inscripcionMPP.ExisteInscripcionActiva(cursoId, alumnoId))
            {
                throw new InvalidOperationException("Ya está inscripto en este curso.");
            }

            CourseBE cursoNuevo = _cursoBLL.FindById(cursoId);
            if (cursoNuevo == null)
            {
                throw new InvalidOperationException("El curso seleccionado no existe.");
            }

            if (!cursoNuevo.DiaSemana.HasValue || !cursoNuevo.HoraInicio.HasValue || !cursoNuevo.HoraFin.HasValue)
            {
                throw new InvalidOperationException("El curso seleccionado no tiene horario definido.");
            }

            List<EnrollmentBE> inscripcionesActuales = _inscripcionMPP.FindByAlumnoId(alumnoId);

            foreach (var inscripcion in inscripcionesActuales)
            {
                if (inscripcion.CursoId == cursoId)
                {
                    continue;
                }

                if (inscripcion.DiaSemana.HasValue && inscripcion.HoraInicio.HasValue && inscripcion.HoraFin.HasValue)
                {
                    if (inscripcion.DiaSemana.Value != cursoNuevo.DiaSemana.Value)
                    {
                        continue;
                    }

                    bool traslape = ExisteTraslapeHorario(
                        alumnoId,
                        cursoNuevo.DiaSemana.Value,
                        cursoNuevo.HoraInicio.Value,
                        cursoNuevo.HoraFin.Value,
                        cursoId
                    );

                    if (traslape)
                    {
                        throw new InvalidOperationException(
                            $"Tiene traslape de horario con el curso '{inscripcion.CursoNombre}' ({GetDiaSemanaNombre(inscripcion.DiaSemana.Value)} {inscripcion.HoraInicio.Value:hh\\:mm} - {inscripcion.HoraFin.Value:hh\\:mm}).");
                    }
                }
            }

            return _inscripcionMPP.Inscribir(cursoId, alumnoId);
        }

        public bool Desinscribir(int cursoId, int alumnoId)
        {
            return _inscripcionMPP.Desinscribir(cursoId, alumnoId);
        }

        public EnrollmentBE FindById(int id)
        {
            return _inscripcionMPP.FindById(id);
        }

        public List<EnrollmentBE> FindByAlumnoId(int alumnoId)
        {
            return _inscripcionMPP.FindByAlumnoId(alumnoId);
        }

        public List<EnrollmentBE> FindAll()
        {
            return _inscripcionMPP.FindAll();
        }

        public int CountAlumnosByCursoId(int cursoId)
        {
            return _inscripcionMPP.CountAlumnosByCursoId(cursoId);
        }

        public bool ExisteInscripcionActiva(int cursoId, int alumnoId)
        {
            return _inscripcionMPP.ExisteInscripcionActiva(cursoId, alumnoId);
        }

        public bool ExisteTraslapeHorario(int alumnoId, int diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int cursoIdExcluir)
        {
            return _inscripcionMPP.ExisteTraslapeHorario(alumnoId, diaSemana, horaInicio, horaFin, cursoIdExcluir);
        }

        private string GetDiaSemanaNombre(int dia)
        {
            switch (dia)
            {
                case 1: return "Lunes";
                case 2: return "Martes";
                case 3: return "Miércoles";
                case 4: return "Jueves";
                case 5: return "Viernes";
                case 6: return "Sábado";
                case 7: return "Domingo";
                default: return $"Día {dia}";
            }
        }
        #endregion
    }
}
