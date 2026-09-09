using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using DAL;
using BE.Entities;

namespace MPP
{
    public class AulaMPP : IAulaMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public AulaMPP() : this(null)
        {
        }

        public AulaMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public bool Save(AulaBE aula)
        {
            if (aula.Id == 0)
            {
                return Insert(aula);
            }

            return Update(aula);
        }

        public bool Delete(AulaBE aula)
        {
            string query = @"UPDATE Aulas
                            SET is_active = 0, last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", aula.Id),
                new SqlParameter("@lastUpdate", DateTime.Now)
            };

            return _access.Save(query, parameters);
        }

        public AulaBE FindById(int id)
        {
            string query = @"SELECT id, nombre, capacidad, is_active, created_at, last_update
                            FROM Aulas
                            WHERE id = @id AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };

            DataTable table = _access.Read(query, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            return MapAula(table.Rows[0]);
        }

        public List<AulaBE> FindAll()
        {
            string query = @"SELECT id, nombre, capacidad, is_active, created_at, last_update
                            FROM Aulas
                            WHERE is_active = 1
                            ORDER BY nombre";

            return FindMany(query);
        }

        public List<AulaBE> FindByName(string nombre)
        {
            string query = @"SELECT id, nombre, capacidad, is_active, created_at, last_update
                            FROM Aulas
                            WHERE is_active = 1 AND nombre LIKE @nombre
                            ORDER BY nombre";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@nombre", "%" + nombre + "%")
            };

            return FindMany(query, parameters);
        }

        public int Count()
        {
            string query = @"SELECT COUNT(*) FROM Aulas WHERE is_active = 1";
            return _access.ReadScalar(query);
        }
        #endregion

        #region Métodos Privados
        private AulaBE MapAula(DataRow row)
        {
            return new AulaBE
            {
                Id = Convert.ToInt32(row["id"]),
                Nombre = row["nombre"].ToString(),
                Capacidad = Convert.ToInt32(row["capacidad"]),
                IsActive = Convert.ToBoolean(row["is_active"]),
                CreatedAt = (DateTime)row["created_at"],
                LastUpdate = (DateTime)row["last_update"]
            };
        }

        private SqlParameter[] CreateAulaParameters(AulaBE aula)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@nombre", aula.Nombre),
                new SqlParameter("@capacidad", aula.Capacidad),
                new SqlParameter("@isActive", aula.IsActive),
                new SqlParameter("@lastUpdate", aula.LastUpdate)
            };
        }

        private bool Insert(AulaBE aula)
        {
            string query = @"INSERT INTO Aulas (nombre, capacidad, is_active, created_at, last_update)
                            VALUES (@nombre, @capacidad, @isActive, @createdAt, @lastUpdate);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = CreateAulaParameters(aula);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@createdAt", aula.CreatedAt);

            var newId = _access.ReadScalar(query, allParameters);
            return newId > 0;
        }

        private bool Update(AulaBE aula)
        {
            string query = @"UPDATE Aulas
                            SET nombre = @nombre,
                                capacidad = @capacidad,
                                is_active = @isActive,
                                last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = CreateAulaParameters(aula);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@id", aula.Id);

            return _access.Save(query, allParameters);
        }

        private List<AulaBE> FindMany(string query)
        {
            List<AulaBE> aulas = new List<AulaBE>();
            DataTable table = _access.Read(query);

            foreach (DataRow row in table.Rows)
            {
                aulas.Add(MapAula(row));
            }

            return aulas;
        }

        private List<AulaBE> FindMany(string query, SqlParameter[] parameters)
        {
            List<AulaBE> aulas = new List<AulaBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                aulas.Add(MapAula(row));
            }

            return aulas;
        }
        #endregion
    }
}
