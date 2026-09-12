using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

using DAL;
using BE.Entities;
using MPP.Interfaces;

namespace MPP
{
    public class EnrollmentMPP : IEnrollmentMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public EnrollmentMPP() : this(null)
        {
        }

        public EnrollmentMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public bool Enroll(int courseId, int studentId)
        {
            string query = @"INSERT INTO CursoAlumnos (curso_id, alumno_id, is_active, created_at)
                            VALUES (@cursoId, @alumnoId, 1, @createdAt)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", courseId),
                new SqlParameter("@alumnoId", studentId),
                new SqlParameter("@createdAt", DateTime.Now)
            };

            return _access.Save(query, parameters);
        }

        public bool Unenroll(int courseId, int studentId)
        {
            string query = @"UPDATE CursoAlumnos
                            SET is_active = 0
                            WHERE curso_id = @cursoId AND alumno_id = @alumnoId AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", courseId),
                new SqlParameter("@alumnoId", studentId)
            };

            return _access.Save(query, parameters);
        }

        public EnrollmentBE FindById(int id)
        {
            string query = @"SELECT ca.id, ca.curso_id, ca.alumno_id, ca.is_active, ca.created_at,
                                    c.nombre AS curso_nombre, c.dia_semana, c.hora_inicio, c.hora_fin,
                                    a.nombre AS aula_nombre,
                                    u.user_name AS alumno_nombre
                            FROM CursoAlumnos ca
                            INNER JOIN Cursos c ON c.id = ca.curso_id
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            INNER JOIN Users u ON u.id = ca.alumno_id
                            WHERE ca.id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };

            DataTable table = _access.Read(query, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            return MapEnrollment(table.Rows[0]);
        }

        public List<EnrollmentBE> FindByStudentId(int studentId)
        {
            string query = @"SELECT ca.id, ca.curso_id, ca.alumno_id, ca.is_active, ca.created_at,
                                    c.nombre AS curso_nombre, c.dia_semana, c.hora_inicio, c.hora_fin,
                                    a.nombre AS aula_nombre,
                                    u.user_name AS alumno_nombre
                            FROM CursoAlumnos ca
                            INNER JOIN Cursos c ON c.id = ca.curso_id
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            INNER JOIN Users u ON u.id = ca.alumno_id
                            WHERE ca.alumno_id = @alumnoId AND ca.is_active = 1 AND c.is_active = 1
                            ORDER BY c.id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@alumnoId", studentId)
            };

            return FindMany(query, parameters);
        }

        public List<EnrollmentBE> FindAll()
        {
            string query = @"SELECT ca.id, ca.curso_id, ca.alumno_id, ca.is_active, ca.created_at,
                                    c.nombre AS curso_nombre, c.dia_semana, c.hora_inicio, c.hora_fin,
                                    a.nombre AS aula_nombre,
                                    u.user_name AS alumno_nombre
                            FROM CursoAlumnos ca
                            INNER JOIN Cursos c ON c.id = ca.curso_id
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            INNER JOIN Users u ON u.id = ca.alumno_id
                            WHERE ca.is_active = 1 AND c.is_active = 1
                            ORDER BY u.user_name, c.nombre";

            return FindMany(query);
        }

        public int CountStudentsByCourseId(int courseId)
        {
            string query = @"SELECT COUNT(*)
                            FROM CursoAlumnos
                            WHERE curso_id = @cursoId AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", courseId)
            };

            return _access.ReadScalar(query, parameters);
        }

        public bool ExistsActiveEnrollment(int courseId, int studentId)
        {
            string query = @"SELECT COUNT(*)
                            FROM CursoAlumnos
                            WHERE curso_id = @cursoId AND alumno_id = @alumnoId AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", courseId),
                new SqlParameter("@alumnoId", studentId)
            };

            return _access.ReadScalar(query, parameters) > 0;
        }

        public bool ExistsScheduleOverlap(int studentId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, int courseIdToExclude)
        {
            string query = @"SELECT COUNT(*)
                            FROM CursoAlumnos ca
                            INNER JOIN Cursos c ON c.id = ca.curso_id
                            WHERE ca.alumno_id = @alumnoId
                              AND ca.is_active = 1
                              AND c.is_active = 1
                              AND c.dia_semana = @diaSemana
                              AND c.hora_inicio < @horaFin
                              AND c.hora_fin > @horaInicio
                              AND c.id != @cursoIdExcluir";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@alumnoId", studentId),
                new SqlParameter("@diaSemana", dayOfWeek),
                new SqlParameter("@horaInicio", startTime),
                new SqlParameter("@horaFin", endTime),
                new SqlParameter("@cursoIdExcluir", courseIdToExclude)
            };

            return _access.ReadScalar(query, parameters) > 0;
        }
        #endregion

        #region Métodos Privados
        private EnrollmentBE MapEnrollment(DataRow row)
        {
            return new EnrollmentBE
            {
                Id = Convert.ToInt32(row["id"]),
                CourseId = Convert.ToInt32(row["curso_id"]),
                StudentId = Convert.ToInt32(row["alumno_id"]),
                IsActive = Convert.ToBoolean(row["is_active"]),
                CreatedAt = (DateTime)row["created_at"],
                CourseName = row["curso_nombre"].ToString(),
                StudentName = row["alumno_nombre"].ToString(),
                DayOfWeek = row["dia_semana"] != DBNull.Value ? Convert.ToInt32(row["dia_semana"]) : (int?)null,
                StartTime = row["hora_inicio"] != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row["hora_inicio"].ToString()) : (TimeSpan?)null,
                EndTime = row["hora_fin"] != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row["hora_fin"].ToString()) : (TimeSpan?)null,
                ClassroomName = row["aula_nombre"] != DBNull.Value ? row["aula_nombre"].ToString() : null
            };
        }

        private List<EnrollmentBE> FindMany(string query)
        {
            List<EnrollmentBE> enrollments = new List<EnrollmentBE>();
            DataTable table = _access.Read(query);

            foreach (DataRow row in table.Rows)
            {
                enrollments.Add(MapEnrollment(row));
            }

            return enrollments;
        }

        private List<EnrollmentBE> FindMany(string query, SqlParameter[] parameters)
        {
            List<EnrollmentBE> enrollments = new List<EnrollmentBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                enrollments.Add(MapEnrollment(row));
            }

            return enrollments;
        }
        #endregion
    }
}
