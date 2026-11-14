using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

using DAL;
using BE.Entities;
using MPP.Interfaces;

namespace MPP
{
    public class ReportMPP : IReportMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public ReportMPP() : this(null)
        {
        }

        public ReportMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public List<StudentPerformanceBE> GetCoursePerformance(int courseId)
        {
            string query = @"SELECT s.id AS student_id,
                                    (s.first_name + ' ' + s.last_name) AS student_name,
                                    att.planillas,
                                    att.presentes,
                                    gr.grades_count,
                                    gr.grade_avg
                            FROM course_students cs
                            INNER JOIN students s ON s.id = cs.student_id
                            LEFT JOIN (
                                SELECT course_id, student_id,
                                       COUNT(*) AS planillas,
                                       SUM(CASE WHEN is_present = 1 THEN 1 ELSE 0 END) AS presentes
                                FROM attendance
                                WHERE course_id = @courseId
                                GROUP BY course_id, student_id
                            ) att ON att.course_id = cs.course_id AND att.student_id = cs.student_id
                            LEFT JOIN (
                                SELECT e.course_id, g.student_id,
                                       COUNT(*) AS grades_count,
                                       AVG(g.grade) AS grade_avg
                                FROM grades g
                                INNER JOIN evaluations e ON e.id = g.evaluation_id
                                WHERE e.course_id = @courseId
                                GROUP BY e.course_id, g.student_id
                            ) gr ON gr.course_id = cs.course_id AND gr.student_id = cs.student_id
                            WHERE cs.course_id = @courseId
                              AND cs.is_active = 1
                            ORDER BY s.last_name, s.first_name";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@courseId", courseId)
            };

            List<StudentPerformanceBE> lines = new List<StudentPerformanceBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                lines.Add(MapStudentPerformance(row));
            }

            return lines;
        }

        public List<AttendanceReportLineBE> GetAttendanceReport(int courseId)
        {
            string query = @"SELECT s.id AS student_id,
                                    (s.first_name + ' ' + s.last_name) AS student_name,
                                    COUNT(a.id) AS planillas,
                                    SUM(CASE WHEN a.is_present = 1 THEN 1 ELSE 0 END) AS presentes
                            FROM course_students cs
                            INNER JOIN students s ON s.id = cs.student_id
                            LEFT JOIN attendance a ON a.course_id = cs.course_id AND a.student_id = cs.student_id
                            WHERE cs.course_id = @courseId
                              AND cs.is_active = 1
                            GROUP BY s.id, s.first_name, s.last_name
                            ORDER BY s.last_name, s.first_name";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@courseId", courseId)
            };

            List<AttendanceReportLineBE> lines = new List<AttendanceReportLineBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                lines.Add(MapAttendanceReportLine(row));
            }

            return lines;
        }

        public List<GradeReportLineBE> GetGradesReport(int courseId)
        {
            string query = @"SELECT s.id AS student_id,
                                    (s.first_name + ' ' + s.last_name) AS student_name,
                                    ISNULL(ev.evaluations_count, 0) AS evaluations_count,
                                    ISNULL(gr.grades_count, 0) AS grades_count,
                                    gr.grade_avg
                            FROM course_students cs
                            INNER JOIN students s ON s.id = cs.student_id
                            LEFT JOIN (
                                SELECT course_id, COUNT(*) AS evaluations_count
                                FROM evaluations
                                WHERE course_id = @courseId
                                GROUP BY course_id
                            ) ev ON ev.course_id = cs.course_id
                            LEFT JOIN (
                                SELECT e.course_id, g.student_id,
                                       COUNT(*) AS grades_count,
                                       AVG(g.grade) AS grade_avg
                                FROM grades g
                                INNER JOIN evaluations e ON e.id = g.evaluation_id
                                WHERE e.course_id = @courseId
                                GROUP BY e.course_id, g.student_id
                            ) gr ON gr.course_id = cs.course_id AND gr.student_id = cs.student_id
                            WHERE cs.course_id = @courseId
                              AND cs.is_active = 1
                            ORDER BY s.last_name, s.first_name";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@courseId", courseId)
            };

            List<GradeReportLineBE> lines = new List<GradeReportLineBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                lines.Add(MapGradeReportLine(row));
            }

            return lines;
        }
        #endregion

        #region Métodos Privados
        private StudentPerformanceBE MapStudentPerformance(DataRow row)
        {
            return new StudentPerformanceBE
            {
                StudentId = Convert.ToInt32(row["student_id"]),
                StudentName = row["student_name"].ToString(),
                AttendancePercent = row["planillas"] == DBNull.Value
                    ? (int?)null
                    : (int)Math.Round(Convert.ToDecimal(row["presentes"]) * 100m / Convert.ToDecimal(row["planillas"])),
                GradesCount = row["grades_count"] == DBNull.Value ? 0 : Convert.ToInt32(row["grades_count"]),
                Average = row["grade_avg"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["grade_avg"])
            };
        }

        private AttendanceReportLineBE MapAttendanceReportLine(DataRow row)
        {
            int totalPlans = Convert.ToInt32(row["planillas"]);
            int presentPlans = row["presentes"] == DBNull.Value ? 0 : Convert.ToInt32(row["presentes"]);

            return new AttendanceReportLineBE
            {
                StudentId = Convert.ToInt32(row["student_id"]),
                StudentName = row["student_name"].ToString(),
                TotalPlans = totalPlans,
                PresentPlans = presentPlans,
                AttendancePercent = totalPlans == 0
                    ? (int?)null
                    : (int)Math.Round(Convert.ToDecimal(presentPlans) * 100m / totalPlans)
            };
        }

        private GradeReportLineBE MapGradeReportLine(DataRow row)
        {
            return new GradeReportLineBE
            {
                StudentId = Convert.ToInt32(row["student_id"]),
                StudentName = row["student_name"].ToString(),
                EvaluationsCount = Convert.ToInt32(row["evaluations_count"]),
                GradesCount = Convert.ToInt32(row["grades_count"]),
                Average = row["grade_avg"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["grade_avg"])
            };
        }
        #endregion
    }
}
