using System.Collections.Generic;

using BE.Entities;

namespace BLL.Interfaces
{
    public interface IReportBLL
    {
        List<StudentPerformanceBE> GetCoursePerformance(int courseId);
        List<AttendanceReportLineBE> GetAttendanceReport(int courseId);
        List<GradeReportLineBE> GetGradesReport(int courseId);
    }
}
