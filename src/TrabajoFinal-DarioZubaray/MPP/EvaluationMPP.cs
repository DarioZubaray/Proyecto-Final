using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

using DAL;
using BE.Entities;
using MPP.Interfaces;

namespace MPP
{
    public class EvaluationMPP : IEvaluationMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public EvaluationMPP() : this(null)
        {
        }

        public EvaluationMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public int Save(EvaluationBE evaluation)
        {
            if (evaluation.Id > 0)
            {
                string updateQuery = @"UPDATE evaluations
                                        SET course_id = @courseId,
                                            name = @name,
                                            date = @date,
                                            last_update = @lastUpdate
                                        WHERE id = @id";

                SqlParameter[] updateParameters = new SqlParameter[]
                {
                    new SqlParameter("@courseId", evaluation.CourseId),
                    new SqlParameter("@name", evaluation.Name),
                    new SqlParameter("@date", evaluation.Date.Date),
                    new SqlParameter("@lastUpdate", evaluation.LastUpdate),
                    new SqlParameter("@id", evaluation.Id)
                };

                _access.Save(updateQuery, updateParameters);
                return evaluation.Id;
            }

            string insertQuery = @"INSERT INTO evaluations (course_id, name, date, created_by, created_at, last_update)
                                    VALUES (@courseId, @name, @date, @createdBy, @createdAt, @lastUpdate);
                                    SELECT SCOPE_IDENTITY();";

            SqlParameter[] insertParameters = new SqlParameter[]
            {
                new SqlParameter("@courseId", evaluation.CourseId),
                new SqlParameter("@name", evaluation.Name),
                new SqlParameter("@date", evaluation.Date.Date),
                new SqlParameter("@createdBy", evaluation.CreatedBy),
                new SqlParameter("@createdAt", evaluation.CreatedAt),
                new SqlParameter("@lastUpdate", evaluation.LastUpdate)
            };

            return _access.ReadScalar(insertQuery, insertParameters);
        }

        public void SaveGrades(int evaluationId, List<GradeBE> grades)
        {
            if (grades == null)
            {
                return;
            }

            foreach (GradeBE grade in grades)
            {
                string query = @"INSERT INTO grades (evaluation_id, student_id, grade)
                                VALUES (@evaluationId, @studentId, @grade)";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@evaluationId", evaluationId),
                    new SqlParameter("@studentId", grade.StudentId),
                    new SqlParameter("@grade", grade.Grade)
                };

                _access.Save(query, parameters);
            }
        }

        public List<EvaluationBE> FindByCourseId(int courseId)
        {
            string query = @"SELECT id, course_id, name, date, created_by, created_at, last_update, dvh
                            FROM evaluations
                            WHERE course_id = @courseId
                            ORDER BY date DESC, id DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@courseId", courseId)
            };

            List<EvaluationBE> evaluations = new List<EvaluationBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                evaluations.Add(MapEvaluation(row));
            }

            return evaluations;
        }

        public List<GradeBE> FindGradesByEvaluationId(int evaluationId)
        {
            string query = @"SELECT id, evaluation_id, student_id, grade, dvh
                            FROM grades
                            WHERE evaluation_id = @evaluationId
                            ORDER BY id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@evaluationId", evaluationId)
            };

            List<GradeBE> grades = new List<GradeBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                grades.Add(MapGrade(row));
            }

            return grades;
        }

        public List<EvaluationBE> FindAll()
        {
            string query = @"SELECT id, course_id, name, date, created_by, created_at, last_update, dvh
                            FROM evaluations
                            ORDER BY id";

            List<EvaluationBE> evaluations = new List<EvaluationBE>();
            DataTable table = _access.Read(query);

            foreach (DataRow row in table.Rows)
            {
                evaluations.Add(MapEvaluation(row));
            }

            return evaluations;
        }

        public List<GradeBE> FindAllGrades()
        {
            string query = @"SELECT id, evaluation_id, student_id, grade, dvh
                            FROM grades
                            ORDER BY id";

            List<GradeBE> grades = new List<GradeBE>();
            DataTable table = _access.Read(query);

            foreach (DataRow row in table.Rows)
            {
                grades.Add(MapGrade(row));
            }

            return grades;
        }

        public EvaluationBE FindById(int id)
        {
            string query = @"SELECT id, course_id, name, date, created_by, created_at, last_update, dvh
                            FROM evaluations
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

            return MapEvaluation(table.Rows[0]);
        }

        public GradeBE FindGradeById(int id)
        {
            string query = @"SELECT id, evaluation_id, student_id, grade, dvh
                            FROM grades
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

            return MapGrade(table.Rows[0]);
        }

        public bool UpdateDvh(int evaluationId, string dvh)
        {
            string query = @"UPDATE evaluations
                             SET dvh = @dvh
                             WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@dvh", dvh),
                new SqlParameter("@id", evaluationId)
            };

            return _access.Save(query, parameters);
        }

        public bool UpdateGradeDvh(int gradeId, string dvh)
        {
            string query = @"UPDATE grades
                             SET dvh = @dvh
                             WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@dvh", dvh),
                new SqlParameter("@id", gradeId)
            };

            return _access.Save(query, parameters);
        }
        #endregion

        #region Métodos Privados
        private EvaluationBE MapEvaluation(DataRow row)
        {
            return new EvaluationBE
            {
                Id = Convert.ToInt32(row["id"]),
                CourseId = Convert.ToInt32(row["course_id"]),
                Name = row["name"].ToString(),
                Date = (DateTime)row["date"],
                CreatedBy = Convert.ToInt32(row["created_by"]),
                CreatedAt = (DateTime)row["created_at"],
                LastUpdate = (DateTime)row["last_update"],
                Dvh = row["dvh"] != DBNull.Value ? row["dvh"].ToString() : null
            };
        }

        private GradeBE MapGrade(DataRow row)
        {
            return new GradeBE
            {
                Id = Convert.ToInt32(row["id"]),
                EvaluationId = Convert.ToInt32(row["evaluation_id"]),
                StudentId = Convert.ToInt32(row["student_id"]),
                Grade = Convert.ToDecimal(row["grade"]),
                Dvh = row["dvh"] != DBNull.Value ? row["dvh"].ToString() : null
            };
        }
        #endregion
    }
}
