using System.Collections.Generic;

using BE.Entities;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class ReportBLL : IReportBLL
    {
        #region Propiedades
        private readonly IReportMPP _reportMPP;
        #endregion

        #region Constructor
        public ReportBLL(IReportMPP reportMPP)
        {
            _reportMPP = reportMPP;
        }
        #endregion

        #region Métodos
        public List<StudentPerformanceBE> GetCoursePerformance(int courseId)
        {
            return _reportMPP.GetCoursePerformance(courseId);
        }

        public List<AttendanceReportLineBE> GetAttendanceReport(int courseId)
        {
            return _reportMPP.GetAttendanceReport(courseId);
        }

        public List<GradeReportLineBE> GetGradesReport(int courseId)
        {
            return _reportMPP.GetGradesReport(courseId);
        }
        #endregion
    }
}
