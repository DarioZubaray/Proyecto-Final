using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

using DAL;
using BE.Entities;
using MPP.Interfaces;

namespace MPP
{
    public class AttendanceMPP : IAttendanceMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public AttendanceMPP() : this(null)
        {
        }

        public AttendanceMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public bool RegisterAttendanceBulk(int courseId, DateTime date, List<AttendanceBE> records)
        {
            foreach (var record in records)
            {
                string query = @"IF EXISTS (SELECT 1 FROM ClasesAlumnos WHERE curso_id = @cursoId AND alumno_id = @alumnoId AND fecha = @fecha)
                                    UPDATE ClasesAlumnos SET presente = @presente WHERE curso_id = @cursoId AND alumno_id = @alumnoId AND fecha = @fecha
                                ELSE
                                    INSERT INTO ClasesAlumnos (curso_id, alumno_id, fecha, presente, created_at)
                                    VALUES (@cursoId, @alumnoId, @fecha, @presente, @createdAt)";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@cursoId", courseId),
                    new SqlParameter("@alumnoId", record.StudentId),
                    new SqlParameter("@fecha", date.Date),
                    new SqlParameter("@presente", record.IsPresent),
                    new SqlParameter("@createdAt", DateTime.Now)
                };

                _access.Save(query, parameters);
            }

            return true;
        }

        public List<AttendanceBE> FindByCourseIdAndDate(int courseId, DateTime date)
        {
            string query = @"SELECT ca.id, ca.curso_id, ca.alumno_id, ca.fecha, ca.presente, ca.created_at,
                                    u.user_name AS alumno_nombre,
                                    c.nombre AS curso_nombre
                            FROM ClasesAlumnos ca
                            INNER JOIN Users u ON u.id = ca.alumno_id
                            INNER JOIN Cursos c ON c.id = ca.curso_id
                            WHERE ca.curso_id = @cursoId AND ca.fecha = @fecha
                            ORDER BY u.user_name";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", courseId),
                new SqlParameter("@fecha", date.Date)
            };

            return FindMany(query, parameters);
        }

        public bool IsTeacherOfCourse(int courseId, int teacherId)
        {
            string query = @"SELECT COUNT(*)
                            FROM CursoDocentes
                            WHERE curso_id = @cursoId AND docente_id = @docenteId AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", courseId),
                new SqlParameter("@docenteId", teacherId)
            };

            return _access.ReadScalar(query, parameters) > 0;
        }
        #endregion

        #region Métodos Privados
        private AttendanceBE MapAttendance(DataRow row)
        {
            return new AttendanceBE
            {
                Id = Convert.ToInt32(row["id"]),
                CourseId = Convert.ToInt32(row["curso_id"]),
                StudentId = Convert.ToInt32(row["alumno_id"]),
                Date = (DateTime)row["fecha"],
                IsPresent = Convert.ToBoolean(row["presente"]),
                CreatedAt = (DateTime)row["created_at"],
                StudentName = row["alumno_nombre"].ToString(),
                CourseName = row["curso_nombre"].ToString()
            };
        }

        private List<AttendanceBE> FindMany(string query, SqlParameter[] parameters)
        {
            List<AttendanceBE> attendances = new List<AttendanceBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                attendances.Add(MapAttendance(row));
            }

            return attendances;
        }
        #endregion
    }
}
