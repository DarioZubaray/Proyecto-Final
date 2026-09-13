using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

using DAL;
using BE.Entities;
using MPP.Interfaces;

namespace MPP
{
    public class AttendanceMPP : IAttendanceMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public AttendanceMPP() : this(null)
        {
        }

        public AttendanceMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public bool RegisterAttendanceBulk(int courseId, DateTime date, List<AttendanceBE> records)
        {
            foreach (var record in records)
            {
                string query = @"IF EXISTS (SELECT 1 FROM attendance WHERE course_id = @courseId AND student_id = @studentId AND date = @date)
                                    UPDATE attendance SET is_present = @isPresent WHERE course_id = @courseId AND student_id = @studentId AND date = @date
                                ELSE
                                    INSERT INTO attendance (course_id, student_id, date, is_present, created_at)
                                    VALUES (@courseId, @studentId, @date, @isPresent, @createdAt)";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@courseId", courseId),
                    new SqlParameter("@studentId", record.StudentId),
                    new SqlParameter("@date", date.Date),
                    new SqlParameter("@isPresent", record.IsPresent),
                    new SqlParameter("@createdAt", DateTime.Now)
                };

                _access.Save(query, parameters);
            }

            return true;
        }

        public List<AttendanceBE> FindByCourseIdAndDate(int courseId, DateTime date)
        {
            string query = @"SELECT a.id, a.course_id, a.student_id, a.date, a.is_present, a.created_at,
                                    u.user_name AS student_name,
                                    c.name AS course_name
                            FROM attendance a
                            INNER JOIN users u ON u.id = a.student_id
                            INNER JOIN courses c ON c.id = a.course_id
                            WHERE a.course_id = @courseId AND a.date = @date
                            ORDER BY u.user_name";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@courseId", courseId),
                new SqlParameter("@date", date.Date)
            };

            return FindMany(query, parameters);
        }

        public bool IsTeacherOfCourse(int courseId, int teacherId)
        {
            string query = @"SELECT COUNT(*)
                            FROM course_teachers
                            WHERE course_id = @courseId AND teacher_id = @teacherId AND is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@courseId", courseId),
                new SqlParameter("@teacherId", teacherId)
            };

            return _access.ReadScalar(query, parameters) > 0;
        }
        #endregion

        #region Métodos Privados
        private AttendanceBE MapAttendance(DataRow row)
        {
            return new AttendanceBE
            {
                Id = Convert.ToInt32(row["id"]),
                CourseId = Convert.ToInt32(row["course_id"]),
                StudentId = Convert.ToInt32(row["student_id"]),
                Date = (DateTime)row["date"],
                IsPresent = Convert.ToBoolean(row["is_present"]),
                CreatedAt = (DateTime)row["created_at"],
                StudentName = row["student_name"].ToString(),
                CourseName = row["course_name"].ToString()
            };
        }

        private List<AttendanceBE> FindMany(string query, SqlParameter[] parameters)
        {
            List<AttendanceBE> attendances = new List<AttendanceBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                attendances.Add(MapAttendance(row));
            }

            return attendances;
        }
        #endregion
    }
}
