using System;
using System.Collections.Generic;

using BE.Entities;

namespace MPP.Interfaces
{
    public interface IEnrollmentMPP
    {
        bool Inscribir(int cursoId, int alumnoId);
        bool Desinscribir(int cursoId, int alumnoId);
        EnrollmentBE FindById(int id);
        List<EnrollmentBE> FindByAlumnoId(int alumnoId);
        List<EnrollmentBE> FindAll();
        int CountAlumnosByCursoId(int cursoId);
        bool ExisteInscripcionActiva(int cursoId, int alumnoId);
        bool ExisteTraslapeHorario(int alumnoId, int diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int cursoIdExcluir);
    }
}
