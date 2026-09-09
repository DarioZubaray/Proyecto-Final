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
        bool ExisteTraslapeAula(int aulaId, DateTime fechaInicio, DateTime fechaFin, int cursoIdExcluir);
        bool SaveDocentes(int cursoId, List<int> docenteIds);
        List<UserBE> GetDocentesByCursoId(int cursoId);
        List<CursoBE> FindByDocenteId(int docenteId);
    }
}
