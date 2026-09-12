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
        public bool Inscribir(int cursoId, int alumnoId)
        {
            string query = @"INSERT INTO CursoAlumnos (curso_id, alumno_id, is_active, created_at)
                            VALUES (@cursoId, @alumnoId, 1, @createdAt)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", cursoId),
                new SqlParameter("@alumnoId", alumnoId),
                new SqlParameter("@createdAt", DateTime.Now)
            };

            return _access.Save(query, parameters);
        }

        public bool Desinscribir(int cursoId, int alumnoId)
        {
            string query = @"UPDATE CursoAlumnos
                            SET is_active = 0
                            WHERE curso_id = @cursoId AND alumno_id = @alumnoId AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", cursoId),
                new SqlParameter("@alumnoId", alumnoId)
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

            return MapInscripcion(table.Rows[0]);
        }

        public List<EnrollmentBE> FindByAlumnoId(int alumnoId)
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
                new SqlParameter("@alumnoId", alumnoId)
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

        public int CountAlumnosByCursoId(int cursoId)
        {
            string query = @"SELECT COUNT(*)
                            FROM CursoAlumnos
                            WHERE curso_id = @cursoId AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", cursoId)
            };

            return _access.ReadScalar(query, parameters);
        }

        public bool ExisteInscripcionActiva(int cursoId, int alumnoId)
        {
            string query = @"SELECT COUNT(*)
                            FROM CursoAlumnos
                            WHERE curso_id = @cursoId AND alumno_id = @alumnoId AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", cursoId),
                new SqlParameter("@alumnoId", alumnoId)
            };

            return _access.ReadScalar(query, parameters) > 0;
        }

        public bool ExisteTraslapeHorario(int alumnoId, int diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int cursoIdExcluir)
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
                new SqlParameter("@alumnoId", alumnoId),
                new SqlParameter("@diaSemana", diaSemana),
                new SqlParameter("@horaInicio", horaInicio),
                new SqlParameter("@horaFin", horaFin),
                new SqlParameter("@cursoIdExcluir", cursoIdExcluir)
            };

            return _access.ReadScalar(query, parameters) > 0;
        }
        #endregion

        #region Métodos Privados
        private EnrollmentBE MapInscripcion(DataRow row)
        {
            return new EnrollmentBE
            {
                Id = Convert.ToInt32(row["id"]),
                CursoId = Convert.ToInt32(row["curso_id"]),
                AlumnoId = Convert.ToInt32(row["alumno_id"]),
                IsActive = Convert.ToBoolean(row["is_active"]),
                CreatedAt = (DateTime)row["created_at"],
                CursoNombre = row["curso_nombre"].ToString(),
                AlumnoNombre = row["alumno_nombre"].ToString(),
                DiaSemana = row["dia_semana"] != DBNull.Value ? Convert.ToInt32(row["dia_semana"]) : (int?)null,
                HoraInicio = row["hora_inicio"] != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row["hora_inicio"].ToString()) : (TimeSpan?)null,
                HoraFin = row["hora_fin"] != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row["hora_fin"].ToString()) : (TimeSpan?)null,
                AulaNombre = row["aula_nombre"] != DBNull.Value ? row["aula_nombre"].ToString() : null
            };
        }

        private List<EnrollmentBE> FindMany(string query)
        {
            List<EnrollmentBE> inscripciones = new List<EnrollmentBE>();
            DataTable table = _access.Read(query);

            foreach (DataRow row in table.Rows)
            {
                inscripciones.Add(MapInscripcion(row));
            }

            return inscripciones;
        }

        private List<EnrollmentBE> FindMany(string query, SqlParameter[] parameters)
        {
            List<EnrollmentBE> inscripciones = new List<EnrollmentBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                inscripciones.Add(MapInscripcion(row));
            }

            return inscripciones;
        }
        #endregion
    }
}
