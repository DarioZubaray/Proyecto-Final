using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

using DAL;
using BE.Entities;
using MPP.Interfaces;

namespace MPP
{
    public class StudentMPP : IStudentMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public StudentMPP() : this(null)
        {
        }

        public StudentMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public List<StudentBE> FindAll()
        {
            string query = @"SELECT id, first_name, last_name, document, email, phone, is_active, created_at, last_update
                            FROM students
                            ORDER BY last_name, first_name";

            return FindMany(query);
        }

        public List<StudentBE> FindAllActive()
        {
            string query = @"SELECT id, first_name, last_name, document, email, phone, is_active, created_at, last_update
                            FROM students
                            WHERE is_active = 1
                            ORDER BY last_name, first_name";

            return FindMany(query);
        }

        public StudentBE FindById(int id)
        {
            string query = @"SELECT id, first_name, last_name, document, email, phone, is_active, created_at, last_update
                            FROM students
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };

            DataTable table = _access.Read(query, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            return MapStudent(table.Rows[0]);
        }

        public List<StudentBE> FindByName(string searchText)
        {
            string query = @"SELECT id, first_name, last_name, document, email, phone, is_active, created_at, last_update
                            FROM students
                            WHERE first_name LIKE @search OR last_name LIKE @search OR document LIKE @search
                            ORDER BY last_name, first_name";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@search", "%" + searchText + "%")
            };

            return FindMany(query, parameters);
        }

        public StudentBE FindByDocument(string document)
        {
            string query = @"SELECT id, first_name, last_name, document, email, phone, is_active, created_at, last_update
                            FROM students
                            WHERE document = @document";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@document", document)
            };

            DataTable table = _access.Read(query, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            return MapStudent(table.Rows[0]);
        }

        public bool Insert(StudentBE student)
        {
            string query = @"INSERT INTO students (first_name, last_name, document, email, phone, is_active, created_at, last_update)
                            VALUES (@firstName, @lastName, @document, @email, @phone, @isActive, @createdAt, @lastUpdate);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = CreateStudentParameters(student);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@createdAt", student.CreatedAt);

            var newId = _access.ReadScalar(query, allParameters);
            return newId > 0;
        }

        public bool Update(StudentBE student)
        {
            string query = @"UPDATE students
                            SET first_name = @firstName,
                                last_name = @lastName,
                                document = @document,
                                email = @email,
                                phone = @phone,
                                is_active = @isActive,
                                last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = CreateStudentParameters(student);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@id", student.Id);

            return _access.Save(query, allParameters);
        }

        public bool Delete(int id)
        {
            string query = @"UPDATE students
                            SET is_active = 0, last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id),
                new SqlParameter("@lastUpdate", DateTime.Now)
            };

            return _access.Save(query, parameters);
        }
        #endregion

        #region Métodos Privados
        private StudentBE MapStudent(DataRow row)
        {
            return new StudentBE
            {
                Id = Convert.ToInt32(row["id"]),
                FirstName = row["first_name"].ToString(),
                LastName = row["last_name"].ToString(),
                Document = row["document"].ToString(),
                Email = row["email"] != DBNull.Value ? row["email"].ToString() : null,
                Phone = row["phone"] != DBNull.Value ? row["phone"].ToString() : null,
                IsActive = Convert.ToBoolean(row["is_active"]),
                CreatedAt = (DateTime)row["created_at"],
                LastUpdate = (DateTime)row["last_update"]
            };
        }

        private SqlParameter[] CreateStudentParameters(StudentBE student)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@firstName", student.FirstName),
                new SqlParameter("@lastName", student.LastName),
                new SqlParameter("@document", student.Document),
                new SqlParameter("@email", (object)student.Email ?? DBNull.Value),
                new SqlParameter("@phone", (object)student.Phone ?? DBNull.Value),
                new SqlParameter("@isActive", student.IsActive),
                new SqlParameter("@lastUpdate", student.LastUpdate)
            };
        }

        private List<StudentBE> FindMany(string query)
        {
            List<StudentBE> students = new List<StudentBE>();
            DataTable table = _access.Read(query);

            foreach (DataRow row in table.Rows)
            {
                students.Add(MapStudent(row));
            }

            return students;
        }

        private List<StudentBE> FindMany(string query, SqlParameter[] parameters)
        {
            List<StudentBE> students = new List<StudentBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                students.Add(MapStudent(row));
            }

            return students;
        }
        #endregion
    }
}
