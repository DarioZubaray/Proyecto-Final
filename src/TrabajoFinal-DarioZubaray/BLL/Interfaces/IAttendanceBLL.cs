using System;
using System.Collections.Generic;

using BE.Entities;

namespace BLL.Interfaces
{
    public interface IAttendanceBLL
    {
        bool RegisterAttendanceBulk(int courseId, DateTime date, List<AttendanceBE> records);
        List<AttendanceBE> FindByCourseIdAndDate(int courseId, DateTime date);
        bool IsTeacherOfCourse(int courseId, int teacherId);
    }
}
