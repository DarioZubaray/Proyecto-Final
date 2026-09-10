using System;
using System.Collections.Generic;
using BE.Entities;

namespace BLL.Interfaces
{
    public interface IInscripcionBLL
    {
        bool Inscribir(int cursoId, int alumnoId);
        bool Desinscribir(int cursoId, int alumnoId);
        InscripcionBE FindById(int id);
        List<InscripcionBE> FindByAlumnoId(int alumnoId);
        List<InscripcionBE> FindAll();
        int CountAlumnosByCursoId(int cursoId);
        bool ExisteInscripcionActiva(int cursoId, int alumnoId);
        bool ExisteTraslapeHorario(int alumnoId, int diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int cursoIdExcluir);
    }
}
