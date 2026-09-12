using System;
using System.Collections.Generic;

using BE.Entities;

namespace BLL.Interfaces
{
    public interface IAttendanceBLL
    {
        bool RegistrarAsistenciaBulk(int cursoId, DateTime fecha, List<AttendanceBE> registros);
        List<AttendanceBE> FindByCursoIdAndFecha(int cursoId, DateTime fecha);
        bool EsDocenteDelCurso(int cursoId, int docenteId);
    }
}
