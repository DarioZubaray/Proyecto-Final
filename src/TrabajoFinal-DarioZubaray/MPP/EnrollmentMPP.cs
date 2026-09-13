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
            string query = @"INSERT INTO course_students (course_id, student_id, is_active, created_at)
                            VALUES (@courseId, @studentId, 1, @createdAt)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@courseId", courseId),
                new SqlParameter("@studentId", studentId),
                new SqlParameter("@createdAt", DateTime.Now)
            };

            return _access.Save(query, parameters);
        }

        public bool Unenroll(int courseId, int studentId)
        {
            string query = @"UPDATE course_students
                            SET is_active = 0
                            WHERE course_id = @courseId AND student_id = @studentId AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@courseId", courseId),
                new SqlParameter("@studentId", studentId)
            };

            return _access.Save(query, parameters);
        }

        public EnrollmentBE FindById(int id)
        {
            string query = @"SELECT cs.id, cs.course_id, cs.student_id, cs.is_active, cs.created_at,
                                    c.name AS course_name, c.day_of_week, c.start_time, c.end_time,
                                    cl.name AS classroom_name,
                                    u.user_name AS student_name
                            FROM course_students cs
                            INNER JOIN courses c ON c.id = cs.course_id
                            LEFT JOIN classrooms cl ON cl.id = c.classroom_id
                            INNER JOIN users u ON u.id = cs.student_id
                            WHERE cs.id = @id";

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
            string query = @"SELECT cs.id, cs.course_id, cs.student_id, cs.is_active, cs.created_at,
                                    c.name AS course_name, c.day_of_week, c.start_time, c.end_time,
                                    cl.name AS classroom_name,
                                    u.user_name AS student_name
                            FROM course_students cs
                            INNER JOIN courses c ON c.id = cs.course_id
                            LEFT JOIN classrooms cl ON cl.id = c.classroom_id
                            INNER JOIN users u ON u.id = cs.student_id
                            WHERE cs.student_id = @studentId AND cs.is_active = 1 AND c.is_active = 1
                            ORDER BY c.id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@studentId", studentId)
            };

            return FindMany(query, parameters);
        }

        public List<EnrollmentBE> FindAll()
        {
            string query = @"SELECT cs.id, cs.course_id, cs.student_id, cs.is_active, cs.created_at,
                                    c.name AS course_name, c.day_of_week, c.start_time, c.end_time,
                                    cl.name AS classroom_name,
                                    u.user_name AS student_name
                            FROM course_students cs
                            INNER JOIN courses c ON c.id = cs.course_id
                            LEFT JOIN classrooms cl ON cl.id = c.classroom_id
                            INNER JOIN users u ON u.id = cs.student_id
                            WHERE cs.is_active = 1 AND c.is_active = 1
                            ORDER BY u.user_name, c.name";

            return FindMany(query);
        }

        public int CountStudentsByCourseId(int courseId)
        {
            string query = @"SELECT COUNT(*)
                            FROM course_students
                            WHERE course_id = @courseId AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@courseId", courseId)
            };

            return _access.ReadScalar(query, parameters);
        }

        public bool ExistsActiveEnrollment(int courseId, int studentId)
        {
            string query = @"SELECT COUNT(*)
                            FROM course_students
                            WHERE course_id = @courseId AND student_id = @studentId AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@courseId", courseId),
                new SqlParameter("@studentId", studentId)
            };

            return _access.ReadScalar(query, parameters) > 0;
        }

        public bool ExistsScheduleOverlap(int studentId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, int courseIdToExclude)
        {
            string query = @"SELECT COUNT(*)
                            FROM course_students cs
                            INNER JOIN courses c ON c.id = cs.course_id
                            WHERE cs.student_id = @studentId
                              AND cs.is_active = 1
                              AND c.is_active = 1
                              AND c.day_of_week = @dayOfWeek
                              AND c.start_time < @endTime
                              AND c.end_time > @startTime
                              AND c.id != @courseIdToExclude";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@studentId", studentId),
                new SqlParameter("@dayOfWeek", dayOfWeek),
                new SqlParameter("@startTime", startTime),
                new SqlParameter("@endTime", endTime),
                new SqlParameter("@courseIdToExclude", courseIdToExclude)
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
                CourseId = Convert.ToInt32(row["course_id"]),
                StudentId = Convert.ToInt32(row["student_id"]),
                IsActive = Convert.ToBoolean(row["is_active"]),
                CreatedAt = (DateTime)row["created_at"],
                CourseName = row["course_name"].ToString(),
                StudentName = row["student_name"].ToString(),
                DayOfWeek = row["day_of_week"] != DBNull.Value ? Convert.ToInt32(row["day_of_week"]) : (int?)null,
                StartTime = row["start_time"] != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row["start_time"].ToString()) : (TimeSpan?)null,
                EndTime = row["end_time"] != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row["end_time"].ToString()) : (TimeSpan?)null,
                ClassroomName = row["classroom_name"] != DBNull.Value ? row["classroom_name"].ToString() : null
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
