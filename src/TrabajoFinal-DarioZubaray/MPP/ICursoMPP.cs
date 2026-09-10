using System;
using System.Collections.Generic;
using BE.Entities;

namespace MPP
{
    public interface ICursoMPP
    {
        bool Save(CursoBE curso);
        bool Delete(CursoBE curso);
        CursoBE FindById(int id);
        List<CursoBE> FindAll();
        List<CursoBE> FindByName(string nombre);
        bool ExisteTraslapeAula(int aulaId, DateTime fechaInicio, DateTime fechaFin, int cursoIdExcluir, int? diaSemana, TimeSpan? horaInicio, TimeSpan? horaFin);
        bool SaveDocentes(int cursoId, List<int> docenteIds);
        List<UserBE> GetDocentesByCursoId(int cursoId);
        List<CursoBE> FindByDocenteId(int docenteId);
        List<CursoBE> FindByAlumnoId(int alumnoId);
        bool ExisteTraslapeDocente(int docenteId, int diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int cursoIdExcluir);
    }
}
