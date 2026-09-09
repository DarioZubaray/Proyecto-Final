using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using DAL;
using BE.Entities;

namespace MPP
{
    public class CursoMPP : ICursoMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public CursoMPP() : this(null)
        {
        }

        public CursoMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public bool Save(CursoBE curso)
        {
            if (curso.Id == 0)
            {
                return Insert(curso);
            }

            return Update(curso);
        }

        public bool Delete(CursoBE curso)
        {
            string query = @"UPDATE Cursos
                            SET is_active = 0, last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", curso.Id),
                new SqlParameter("@lastUpdate", DateTime.Now)
            };

            return _access.Save(query, parameters);
        }

        public CursoBE FindById(int id)
        {
            string query = @"SELECT c.id, c.nombre, c.descripcion, c.fecha_inicio, c.fecha_fin,
                                    c.aula_id, c.is_active, c.created_at, c.last_update,
                                    a.nombre AS aula_nombre
                            FROM Cursos c
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            WHERE c.id = @id AND c.is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };

            DataTable table = _access.Read(query, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            CursoBE curso = MapCurso(table.Rows[0]);
            curso.Docentes = GetDocentesByCursoId(id);
            return curso;
        }

        public List<CursoBE> FindAll()
        {
            string query = @"SELECT c.id, c.nombre, c.descripcion, c.fecha_inicio, c.fecha_fin,
                                    c.aula_id, c.is_active, c.created_at, c.last_update,
                                    a.nombre AS aula_nombre
                            FROM Cursos c
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            WHERE c.is_active = 1
                            ORDER BY c.nombre";

            return FindMany(query);
        }

        public List<CursoBE> FindByName(string nombre)
        {
            string query = @"SELECT c.id, c.nombre, c.descripcion, c.fecha_inicio, c.fecha_fin,
                                    c.aula_id, c.is_active, c.created_at, c.last_update,
                                    a.nombre AS aula_nombre
                            FROM Cursos c
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            WHERE c.is_active = 1 AND c.nombre LIKE @nombre
                            ORDER BY c.nombre";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@nombre", "%" + nombre + "%")
            };

            return FindMany(query, parameters);
        }

        public bool ExisteTraslapeAula(int aulaId, DateTime fechaInicio, DateTime fechaFin, int cursoIdExcluir)
        {
            string query = @"SELECT COUNT(*)
                            FROM Cursos
                            WHERE aula_id = @aulaId
                              AND is_active = 1
                              AND id != @cursoIdExcluir
                              AND fecha_inicio < @fechaFin
                              AND fecha_fin > @fechaInicio";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@aulaId", aulaId),
                new SqlParameter("@fechaInicio", fechaInicio),
                new SqlParameter("@fechaFin", fechaFin),
                new SqlParameter("@cursoIdExcluir", cursoIdExcluir)
            };

            return _access.ReadScalar(query, parameters) > 0;
        }

        public bool SaveDocentes(int cursoId, List<int> docenteIds)
        {
            string deleteQuery = @"UPDATE CursoDocentes
                                  SET is_active = 0
                                  WHERE curso_id = @cursoId AND is_active = 1";

            SqlParameter[] deleteParams = new SqlParameter[]
            {
                new SqlParameter("@cursoId", cursoId)
            };

            _access.Save(deleteQuery, deleteParams);

            foreach (int docenteId in docenteIds)
            {
                string insertQuery = @"INSERT INTO CursoDocentes (curso_id, docente_id, is_active, created_at)
                                      VALUES (@cursoId, @docenteId, 1, @createdAt)";
                SqlParameter[] insertParams = new SqlParameter[]
                {
                    new SqlParameter("@cursoId", cursoId),
                    new SqlParameter("@docenteId", docenteId),
                    new SqlParameter("@createdAt", DateTime.Now)
                };
                _access.Save(insertQuery, insertParams);
            }

            return true;
        }

        public List<UserBE> GetDocentesByCursoId(int cursoId)
        {
            string query = @"SELECT u.id, u.user_name, u.password_hash, u.is_active,
                                    u.retries_count, u.last_update, u.created_at,
                                    u.language, u.theme, u.role_id
                            FROM Users u
                            INNER JOIN CursoDocentes cd ON cd.docente_id = u.id
                            WHERE cd.curso_id = @cursoId AND cd.is_active = 1 AND u.is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@cursoId", cursoId)
            };

            List<UserBE> docentes = new List<UserBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                docentes.Add(MapUser(row));
            }

            return docentes;
        }

        public List<CursoBE> FindByDocenteId(int docenteId)
        {
            string query = @"SELECT c.id, c.nombre, c.descripcion, c.fecha_inicio, c.fecha_fin,
                                    c.aula_id, c.is_active, c.created_at, c.last_update,
                                    a.nombre AS aula_nombre
                            FROM Cursos c
                            INNER JOIN CursoDocentes cd ON cd.curso_id = c.id
                            LEFT JOIN Aulas a ON a.id = c.aula_id
                            WHERE cd.docente_id = @docenteId AND cd.is_active = 1 AND c.is_active = 1
                            ORDER BY c.nombre";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@docenteId", docenteId)
            };

            return FindMany(query, parameters);
        }
        #endregion

        #region Métodos Privados
        private CursoBE MapCurso(DataRow row)
        {
            return new CursoBE
            {
                Id = Convert.ToInt32(row["id"]),
                Nombre = row["nombre"].ToString(),
                Descripcion = row["descripcion"] != DBNull.Value ? row["descripcion"].ToString() : null,
                FechaInicio = (DateTime)row["fecha_inicio"],
                FechaFin = (DateTime)row["fecha_fin"],
                AulaId = row["aula_id"] != DBNull.Value ? Convert.ToInt32(row["aula_id"]) : (int?)null,
                AulaNombre = row["aula_nombre"] != DBNull.Value ? row["aula_nombre"].ToString() : null,
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

        private SqlParameter[] CreateCursoParameters(CursoBE curso)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@nombre", curso.Nombre),
                new SqlParameter("@descripcion", (object)curso.Descripcion ?? DBNull.Value),
                new SqlParameter("@fechaInicio", curso.FechaInicio),
                new SqlParameter("@fechaFin", curso.FechaFin),
                new SqlParameter("@aulaId", (object)curso.AulaId ?? DBNull.Value),
                new SqlParameter("@isActive", curso.IsActive),
                new SqlParameter("@lastUpdate", curso.LastUpdate)
            };
        }

        private bool Insert(CursoBE curso)
        {
            string query = @"INSERT INTO Cursos (nombre, descripcion, fecha_inicio, fecha_fin, aula_id, is_active, created_at, last_update)
                            VALUES (@nombre, @descripcion, @fechaInicio, @fechaFin, @aulaId, @isActive, @createdAt, @lastUpdate);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = CreateCursoParameters(curso);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@createdAt", curso.CreatedAt);

            var newId = _access.ReadScalar(query, allParameters);
            if (newId > 0)
            {
                curso.Id = newId;
                return true;
            }
            return false;
        }

        private bool Update(CursoBE curso)
        {
            string query = @"UPDATE Cursos
                            SET nombre = @nombre,
                                descripcion = @descripcion,
                                fecha_inicio = @fechaInicio,
                                fecha_fin = @fechaFin,
                                aula_id = @aulaId,
                                is_active = @isActive,
                                last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = CreateCursoParameters(curso);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@id", curso.Id);

            return _access.Save(query, allParameters);
        }

        private List<CursoBE> FindMany(string query)
        {
            List<CursoBE> cursos = new List<CursoBE>();
            DataTable table = _access.Read(query);

            foreach (DataRow row in table.Rows)
            {
                cursos.Add(MapCurso(row));
            }

            return cursos;
        }

        private List<CursoBE> FindMany(string query, SqlParameter[] parameters)
        {
            List<CursoBE> cursos = new List<CursoBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                cursos.Add(MapCurso(row));
            }

            return cursos;
        }
        #endregion
    }
}
