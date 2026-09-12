using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

using DAL;
using BE.Entities;
using MPP.Interfaces;

namespace MPP
{
    public class CourseMPP : ICourseMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public CourseMPP() : this(null)
        {
        }

        public CourseMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public bool Save(CourseBE course)
        {
            if (course.Id == 0)
            {
                return Insert(course);
            }

            return Update(course);
        }

        public bool Delete(CourseBE course)
        {
            string query = @"UPDATE Cursos
                            SET is_active = 0, last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", course.Id),
                new SqlParameter("@lastUpdate", DateTime.Now)
            };

            return _access.Save(query, parameters);
        }

        public CourseBE FindById(int id)
        {
            string query = @"SELECT c.id, c.nombre, c.descripcion, c.fecha_inicio, c.fecha_fin,
                                    c.aula_id, c.dia_semana, c.hora_inicio, c.hora_fin,
                                    c.is_active, c.created_at, c.last_update,
                                    a.nombre AS aula_nombre
                            FROM Cursos c
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            WHERE c.id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };

            DataTable table = _access.Read(query, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            CourseBE course = MapCourse(table.Rows[0]);
            course.Teachers = GetTeachersByCourseId(id);
            return course;
        }

        public List<CourseBE> FindAll()
        {
            string query = @"SELECT c.id, c.nombre, c.descripcion, c.fecha_inicio, c.fecha_fin,
                                    c.aula_id, c.dia_semana, c.hora_inicio, c.hora_fin,
                                    c.is_active, c.created_at, c.last_update,
                                    a.nombre AS aula_nombre
                            FROM Cursos c
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            WHERE c.is_active = 1
                            ORDER BY c.id";

            var courses = FindMany(query);
            foreach (var course in courses)
            {
                course.Teachers = GetTeachersByCourseId(course.Id);
            }
            return courses;
        }

        public List<CourseBE> FindAllIncludingInactive()
        {
            string query = @"SELECT c.id, c.nombre, c.descripcion, c.fecha_inicio, c.fecha_fin,
                                    c.aula_id, c.dia_semana, c.hora_inicio, c.hora_fin,
                                    c.is_active, c.created_at, c.last_update,
                                    a.nombre AS aula_nombre
                            FROM Cursos c
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            WHERE c.fecha_fin >= CAST(GETDATE() AS DATE)
                            ORDER BY c.is_active DESC, c.id";

            var courses = FindMany(query);
            foreach (var course in courses)
            {
                course.Teachers = GetTeachersByCourseId(course.Id);
            }
            return courses;
        }

        public List<CourseBE> FindByName(string name)
        {
            string query = @"SELECT c.id, c.nombre, c.descripcion, c.fecha_inicio, c.fecha_fin,
                                    c.aula_id, c.dia_semana, c.hora_inicio, c.hora_fin,
                                    c.is_active, c.created_at, c.last_update,
                                    a.nombre AS aula_nombre
                            FROM Cursos c
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            WHERE c.nombre LIKE @nombre
                            ORDER BY c.is_active DESC, c.id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@nombre", "%" + name + "%")
            };

            var courses = FindMany(query, parameters);
            foreach (var course in courses)
            {
                course.Teachers = GetTeachersByCourseId(course.Id);
            }
            return courses;
        }

        public bool ExistsClassroomOverlap(int classroomId, DateTime startDate, DateTime endDate, int courseIdToExclude, int? dayOfWeek, TimeSpan? startTime, TimeSpan? endTime)
        {
            string query = @"SELECT COUNT(*)
                            FROM Cursos
                            WHERE aula_id = @aulaId
                              AND is_active = 1
                              AND id != @cursoIdExcluir
                              AND fecha_inicio < @fechaFin
                              AND fecha_fin > @fechaInicio
                              AND (
                                  -- Ambos tienen dia definido y coinciden
                                  (dia_semana IS NOT NULL AND @diaSemana IS NOT NULL AND dia_semana = @diaSemana)
                                  -- O ambos no tienen dia definido
                                  OR (dia_semana IS NULL AND @diaSemana IS NULL)
                              )
                              AND (
                                  -- Ambos tienen horario definido y se trasapan
                                  (hora_inicio IS NOT NULL AND hora_fin IS NOT NULL AND @horaInicio IS NOT NULL AND @horaFin IS NOT NULL
                                   AND hora_inicio < @horaFin AND hora_fin > @horaInicio)
                                  -- O ambos no tienen horario definido
                                  OR (hora_inicio IS NULL AND hora_fin IS NULL AND @horaInicio IS NULL AND @horaFin IS NULL)
                              )";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@aulaId", classroomId),
                new SqlParameter("@fechaInicio", startDate),
                new SqlParameter("@fechaFin", endDate),
                new SqlParameter("@cursoIdExcluir", courseIdToExclude),
                new SqlParameter("@diaSemana", (object)dayOfWeek ?? DBNull.Value),
                new SqlParameter("@horaInicio", startTime.HasValue ? (object)startTime.Value : DBNull.Value),
                new SqlParameter("@horaFin", endTime.HasValue ? (object)endTime.Value : DBNull.Value)
            };

            return _access.ReadScalar(query, parameters) > 0;
        }

        public bool SaveTeachers(int courseId, List<int> teacherIds)
        {
            string deleteQuery = @"UPDATE CursoDocentes
                                  SET is_active = 0
                                  WHERE curso_id = @cursoId AND is_active = 1";

            SqlParameter[] deleteParams = new SqlParameter[]
            {
                new SqlParameter("@cursoId", courseId)
            };

            _access.Save(deleteQuery, deleteParams);

            foreach (int teacherId in teacherIds)
            {
                string insertQuery = @"INSERT INTO CursoDocentes (curso_id, docente_id, is_active, created_at)
                                      VALUES (@cursoId, @docenteId, 1, @createdAt)";
                SqlParameter[] insertParams = new SqlParameter[]
                {
                    new SqlParameter("@cursoId", courseId),
                    new SqlParameter("@docenteId", teacherId),
                    new SqlParameter("@createdAt", DateTime.Now)
                };
                _access.Save(insertQuery, insertParams);
            }

            return true;
        }

        public List<UserBE> GetTeachersByCourseId(int courseId)
        {
            string query = @"SELECT u.id, u.user_name, u.password_hash, u.is_active,
                                    u.retries_count, u.last_update, u.created_at,
                                    u.language, u.theme, u.role_id
                            FROM Users u
                            INNER JOIN CursoDocentes cd ON cd.docente_id = u.id
                            WHERE cd.curso_id = @cursoId AND cd.is_active = 1 AND u.is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", courseId)
            };

            List<UserBE> teachers = new List<UserBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                teachers.Add(MapUser(row));
            }

            return teachers;
        }

        public List<CourseBE> FindByTeacherId(int teacherId)
        {
            string query = @"SELECT c.id, c.nombre, c.descripcion, c.fecha_inicio, c.fecha_fin,
                                    c.aula_id, c.dia_semana, c.hora_inicio, c.hora_fin,
                                    c.is_active, c.created_at, c.last_update,
                                    a.nombre AS aula_nombre
                            FROM Cursos c
                            INNER JOIN CursoDocentes cd ON cd.curso_id = c.id
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            WHERE cd.docente_id = @docenteId AND cd.is_active = 1 AND c.is_active = 1
                            ORDER BY c.id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@docenteId", teacherId)
            };

            return FindMany(query, parameters);
        }

        public List<CourseBE> FindByStudentId(int studentId)
        {
            string query = @"SELECT c.id, c.nombre, c.descripcion, c.fecha_inicio, c.fecha_fin,
                                    c.aula_id, c.dia_semana, c.hora_inicio, c.hora_fin,
                                    c.is_active, c.created_at, c.last_update,
                                    a.nombre AS aula_nombre
                            FROM Cursos c
                            INNER JOIN CursoAlumnos ca ON ca.curso_id = c.id
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            WHERE ca.alumno_id = @alumnoId AND ca.is_active = 1 AND c.is_active = 1
                            ORDER BY c.id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@alumnoId", studentId)
            };

            return FindMany(query, parameters);
        }

        public bool ExistsTeacherOverlap(int teacherId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, int courseIdToExclude)
        {
            string query = @"SELECT COUNT(*)
                            FROM CursoDocentes cd
                            INNER JOIN Cursos c ON c.id = cd.curso_id
                            WHERE cd.docente_id = @docenteId
                            AND cd.is_active = 1
                            AND c.is_active = 1
                            AND c.dia_semana = @diaSemana
                            AND c.hora_inicio < @horaFin
                            AND c.hora_fin > @horaInicio
                            AND c.id <> @cursoIdExcluir";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@docenteId", teacherId),
                new SqlParameter("@diaSemana", dayOfWeek),
                new SqlParameter("@horaInicio", startTime),
                new SqlParameter("@horaFin", endTime),
                new SqlParameter("@cursoIdExcluir", courseIdToExclude)
            };

            DataTable table = _access.Read(query, parameters);
            return Convert.ToInt32(table.Rows[0][0]) > 0;
        }
        #endregion

        #region Métodos Privados
        private CourseBE MapCourse(DataRow row)
        {
            return new CourseBE
            {
                Id = Convert.ToInt32(row["id"]),
                Name = row["nombre"].ToString(),
                Description = row["descripcion"] != DBNull.Value ? row["descripcion"].ToString() : null,
                StartDate = (DateTime)row["fecha_inicio"],
                EndDate = (DateTime)row["fecha_fin"],
                ClassroomId = row["aula_id"] != DBNull.Value ? Convert.ToInt32(row["aula_id"]) : 0,
                ClassroomName = row["aula_nombre"] != DBNull.Value ? row["aula_nombre"].ToString() : null,
                DayOfWeek = row["dia_semana"] != DBNull.Value ? Convert.ToInt32(row["dia_semana"]) : (int?)null,
                StartTime = row["hora_inicio"] != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row["hora_inicio"].ToString()) : (TimeSpan?)null,
                EndTime = row["hora_fin"] != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row["hora_fin"].ToString()) : (TimeSpan?)null,
                IsActive = Convert.ToBoolean(row["is_active"]),
                CreatedAt = (DateTime)row["created_at"],
                LastUpdate = (DateTime)row["last_update"]
            };
        }

        private UserBE MapUser(DataRow row)
        {
            return new UserBE
            {
                Id = Convert.ToInt32(row["id"]),
                UserName = row["user_name"].ToString(),
                PasswordHash = row["password_hash"].ToString(),
                IsActive = Convert.ToBoolean(row["is_active"]),
                RetriesCount = Convert.ToInt32(row["retries_count"]),
                LastUpdate = (DateTime)row["last_update"],
                CreatedAt = (DateTime)row["created_at"],
                Language = row["language"].ToString(),
                Theme = row["theme"] != DBNull.Value ? row["theme"].ToString() : "System",
                RoleId = row["role_id"] != DBNull.Value ? Convert.ToInt32(row["role_id"]) : 0
            };
        }

        private SqlParameter[] CreateCourseParameters(CourseBE course)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@nombre", course.Name),
                new SqlParameter("@descripcion", (object)course.Description ?? DBNull.Value),
                new SqlParameter("@fechaInicio", course.StartDate),
                new SqlParameter("@fechaFin", course.EndDate),
                new SqlParameter("@aulaId", course.ClassroomId),
                new SqlParameter("@diaSemana", (object)course.DayOfWeek ?? DBNull.Value),
                new SqlParameter("@horaInicio", course.StartTime.HasValue ? (object)course.StartTime.Value : DBNull.Value),
                new SqlParameter("@horaFin", course.EndTime.HasValue ? (object)course.EndTime.Value : DBNull.Value),
                new SqlParameter("@isActive", course.IsActive),
                new SqlParameter("@lastUpdate", course.LastUpdate)
            };
        }

        private bool Insert(CourseBE course)
        {
            string query = @"INSERT INTO Cursos (nombre, descripcion, fecha_inicio, fecha_fin, aula_id,
                                    dia_semana, hora_inicio, hora_fin, is_active, created_at, last_update)
                            VALUES (@nombre, @descripcion, @fechaInicio, @fechaFin, @aulaId,
                                    @diaSemana, @horaInicio, @horaFin, @isActive, @createdAt, @lastUpdate);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = CreateCourseParameters(course);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@createdAt", course.CreatedAt);

            var newId = _access.ReadScalar(query, allParameters);
            if (newId > 0)
            {
                course.Id = newId;
                return true;
            }
            return false;
        }

        private bool Update(CourseBE course)
        {
            string query = @"UPDATE Cursos
                            SET nombre = @nombre,
                                descripcion = @descripcion,
                                fecha_inicio = @fechaInicio,
                                fecha_fin = @fechaFin,
                                aula_id = @aulaId,
                                dia_semana = @diaSemana,
                                hora_inicio = @horaInicio,
                                hora_fin = @horaFin,
                                is_active = @isActive,
                                last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = CreateCourseParameters(course);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@id", course.Id);

            return _access.Save(query, allParameters);
        }

        private List<CourseBE> FindMany(string query)
        {
            List<CourseBE> courses = new List<CourseBE>();
            DataTable table = _access.Read(query);

            foreach (DataRow row in table.Rows)
            {
                courses.Add(MapCourse(row));
            }

            return courses;
        }

        private List<CourseBE> FindMany(string query, SqlParameter[] parameters)
        {
            List<CourseBE> courses = new List<CourseBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                courses.Add(MapCourse(row));
            }

            return courses;
        }
        #endregion
    }
}
