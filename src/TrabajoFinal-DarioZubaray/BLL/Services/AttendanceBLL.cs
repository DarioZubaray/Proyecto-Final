using System;
using System.Collections.Generic;

using BE.Entities;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class AttendanceBLL : IAttendanceBLL
    {
        #region Propiedades
        private readonly IAttendanceMPP _attendanceMPP;
        #endregion

        #region Constructor
        public AttendanceBLL(IAttendanceMPP attendanceMPP)
        {
            _attendanceMPP = attendanceMPP;
        }
        #endregion

        #region Métodos
        public bool RegisterAttendanceBulk(int courseId, DateTime date, List<AttendanceBE> records)
        {
            return _attendanceMPP.RegisterAttendanceBulk(courseId, date, records);
        }

        public List<AttendanceBE> FindByCourseIdAndDate(int courseId, DateTime date)
        {
            return _attendanceMPP.FindByCourseIdAndDate(courseId, date);
        }

        public bool IsTeacherOfCourse(int courseId, int teacherId)
        {
            return _attendanceMPP.IsTeacherOfCourse(courseId, teacherId);
        }
        #endregion
    }
}
