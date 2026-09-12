using System;
using System.Collections.Generic;
using BE.Entities;

namespace BLL.Interfaces
{
    public interface IAsistenciaBLL
    {
        bool RegistrarAsistenciaBulk(int cursoId, DateTime fecha, List<AsistenciaBE> registros);
        List<AsistenciaBE> FindByCursoIdAndFecha(int cursoId, DateTime fecha);
        bool EsDocenteDelCurso(int cursoId, int docenteId);
    }
}
