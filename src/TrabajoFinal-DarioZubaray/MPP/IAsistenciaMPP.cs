using System;
using System.Collections.Generic;
using BE.Entities;

namespace MPP
{
    public interface IAsistenciaMPP
    {
        bool RegistrarAsistenciaBulk(int cursoId, DateTime fecha, List<AsistenciaBE> registros);
        List<AsistenciaBE> FindByCursoIdAndFecha(int cursoId, DateTime fecha);
        bool EsDocenteDelCurso(int cursoId, int docenteId);
    }
}
