using System;
using System.Collections.Generic;

using BE.Entities;

namespace BLL.Interfaces
{
    public interface ICourseBLL
    {
        bool Save(CourseBE curso);
        bool Delete(CourseBE curso);
        CourseBE FindById(int id);
        List<CourseBE> FindAll();
        List<CourseBE> FindAllIncludingInactive();
        List<CourseBE> FindByName(string nombre);
        bool ExisteTraslapeAula(int aulaId, DateTime fechaInicio, DateTime fechaFin, int cursoIdExcluir, int? diaSemana, TimeSpan? horaInicio, TimeSpan? horaFin);
        bool SaveDocentes(int cursoId, List<int> docenteIds);
        bool ValidarTraslapeDocentes(int cursoId, int? diaSemana, TimeSpan? horaInicio, TimeSpan? horaFin, List<int> docenteIds);
        List<UserBE> GetDocentesByCursoId(int cursoId);
        List<CourseBE> FindByDocenteId(int docenteId);
        List<CourseBE> FindByAlumnoId(int alumnoId);
    }
}
