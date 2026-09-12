using System;
using System.Collections.Generic;

using BE.Entities;

namespace MPP.Interfaces
{
    public interface IAttendanceMPP
    {
        bool RegistrarAsistenciaBulk(int cursoId, DateTime fecha, List<AttendanceBE> registros);
        List<AttendanceBE> FindByCursoIdAndFecha(int cursoId, DateTime fecha);
        bool EsDocenteDelCurso(int cursoId, int docenteId);
    }
}
