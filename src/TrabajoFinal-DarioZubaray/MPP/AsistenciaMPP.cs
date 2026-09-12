using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using DAL;
using BE.Entities;

namespace MPP
{
    public class AsistenciaMPP : IAsistenciaMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public AsistenciaMPP() : this(null)
        {
        }

        public AsistenciaMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public bool RegistrarAsistenciaBulk(int cursoId, DateTime fecha, List<AsistenciaBE> registros)
        {
            foreach (var registro in registros)
            {
                string query = @"IF EXISTS (SELECT 1 FROM ClasesAlumnos WHERE curso_id = @cursoId AND alumno_id = @alumnoId AND fecha = @fecha)
                                    UPDATE ClasesAlumnos SET presente = @presente WHERE curso_id = @cursoId AND alumno_id = @alumnoId AND fecha = @fecha
                                ELSE
                                    INSERT INTO ClasesAlumnos (curso_id, alumno_id, fecha, presente, created_at)
                                    VALUES (@cursoId, @alumnoId, @fecha, @presente, @createdAt)";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@cursoId", cursoId),
                    new SqlParameter("@alumnoId", registro.AlumnoId),
                    new SqlParameter("@fecha", fecha.Date),
                    new SqlParameter("@presente", registro.Presente),
                    new SqlParameter("@createdAt", DateTime.Now)
                };

                _access.Save(query, parameters);
            }

            return true;
        }

        public List<AsistenciaBE> FindByCursoIdAndFecha(int cursoId, DateTime fecha)
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
                new SqlParameter("@cursoId", cursoId),
                new SqlParameter("@fecha", fecha.Date)
            };

            return FindMany(query, parameters);
        }

        public bool EsDocenteDelCurso(int cursoId, int docenteId)
        {
            string query = @"SELECT COUNT(*)
                            FROM CursoDocentes
                            WHERE curso_id = @cursoId AND docente_id = @docenteId AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", cursoId),
                new SqlParameter("@docenteId", docenteId)
            };

            return _access.ReadScalar(query, parameters) > 0;
        }
        #endregion

        #region Métodos Privados
        private AsistenciaBE MapAsistencia(DataRow row)
        {
            return new AsistenciaBE
            {
                Id = Convert.ToInt32(row["id"]),
                CursoId = Convert.ToInt32(row["curso_id"]),
                AlumnoId = Convert.ToInt32(row["alumno_id"]),
                Fecha = (DateTime)row["fecha"],
                Presente = Convert.ToBoolean(row["presente"]),
                CreatedAt = (DateTime)row["created_at"],
                AlumnoNombre = row["alumno_nombre"].ToString(),
                CursoNombre = row["curso_nombre"].ToString()
            };
        }

        private List<AsistenciaBE> FindMany(string query, SqlParameter[] parameters)
        {
            List<AsistenciaBE> asistencias = new List<AsistenciaBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                asistencias.Add(MapAsistencia(row));
            }

            return asistencias;
        }
        #endregion
    }
}
