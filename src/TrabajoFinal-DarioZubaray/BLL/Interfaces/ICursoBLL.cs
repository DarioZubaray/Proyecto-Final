using System;
using System.Collections.Generic;
using BE.Entities;

namespace BLL.Interfaces
{
    public interface ICursoBLL
    {
        bool Save(CursoBE curso);
        bool Delete(CursoBE curso);
        CursoBE FindById(int id);
        List<CursoBE> FindAll();
        List<CursoBE> FindByName(string nombre);
        bool ExisteTraslapeAula(int aulaId, DateTime fechaInicio, DateTime fechaFin, int cursoIdExcluir, int? diaSemana, TimeSpan? horaInicio, TimeSpan? horaFin);
        bool SaveDocentes(int cursoId, List<int> docenteIds);
        bool ValidarTraslapeDocentes(int cursoId, int? diaSemana, TimeSpan? horaInicio, TimeSpan? horaFin, List<int> docenteIds);
        List<UserBE> GetDocentesByCursoId(int cursoId);
        List<CursoBE> FindByDocenteId(int docenteId);
        List<CursoBE> FindByAlumnoId(int alumnoId);
    }
}
