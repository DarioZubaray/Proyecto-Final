using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

using DAL;
using BE.Entities;
using MPP.Interfaces;

namespace MPP
{
    public class ClassroomMPP : IClassroomMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public ClassroomMPP() : this(null)
        {
        }

        public ClassroomMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public bool Save(ClassroomBE classroom)
        {
            if (classroom.Id == 0)
            {
                return Insert(classroom);
            }

            return Update(classroom);
        }

        public bool Delete(ClassroomBE classroom)
        {
            string query = @"UPDATE Aulas
                            SET is_active = 0, last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", classroom.Id),
                new SqlParameter("@lastUpdate", DateTime.Now)
            };

            return _access.Save(query, parameters);
        }

        public ClassroomBE FindById(int id)
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

            return MapClassroom(table.Rows[0]);
        }

        public List<ClassroomBE> FindAll()
        {
            string query = @"SELECT id, nombre, capacidad, is_active, created_at, last_update
                            FROM Aulas
                            WHERE is_active = 1
                            ORDER BY nombre";

            return FindMany(query);
        }

        public List<ClassroomBE> FindByName(string name)
        {
            string query = @"SELECT id, nombre, capacidad, is_active, created_at, last_update
                            FROM Aulas
                            WHERE is_active = 1 AND nombre LIKE @nombre
                            ORDER BY nombre";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@nombre", "%" + name + "%")
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
        private ClassroomBE MapClassroom(DataRow row)
        {
            return new ClassroomBE
            {
                Id = Convert.ToInt32(row["id"]),
                Name = row["nombre"].ToString(),
                Capacity = Convert.ToInt32(row["capacidad"]),
                IsActive = Convert.ToBoolean(row["is_active"]),
                CreatedAt = (DateTime)row["created_at"],
                LastUpdate = (DateTime)row["last_update"]
            };
        }

        private SqlParameter[] CreateClassroomParameters(ClassroomBE classroom)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@nombre", classroom.Name),
                new SqlParameter("@capacidad", classroom.Capacity),
                new SqlParameter("@isActive", classroom.IsActive),
                new SqlParameter("@lastUpdate", classroom.LastUpdate)
            };
        }

        private bool Insert(ClassroomBE classroom)
        {
            string query = @"INSERT INTO Aulas (nombre, capacidad, is_active, created_at, last_update)
                            VALUES (@nombre, @capacidad, @isActive, @createdAt, @lastUpdate);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = CreateClassroomParameters(classroom);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@createdAt", classroom.CreatedAt);

            var newId = _access.ReadScalar(query, allParameters);
            return newId > 0;
        }

        private bool Update(ClassroomBE classroom)
        {
            string query = @"UPDATE Aulas
                            SET nombre = @nombre,
                                capacidad = @capacidad,
                                is_active = @isActive,
                                last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = CreateClassroomParameters(classroom);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@id", classroom.Id);

            return _access.Save(query, allParameters);
        }

        private List<ClassroomBE> FindMany(string query)
        {
            List<ClassroomBE> classrooms = new List<ClassroomBE>();
            DataTable table = _access.Read(query);

            foreach (DataRow row in table.Rows)
            {
                classrooms.Add(MapClassroom(row));
            }

            return classrooms;
        }

        private List<ClassroomBE> FindMany(string query, SqlParameter[] parameters)
        {
            List<ClassroomBE> classrooms = new List<ClassroomBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                classrooms.Add(MapClassroom(row));
            }

            return classrooms;
        }
        #endregion
    }
}
