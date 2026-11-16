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
        private readonly ICourseMPP _courseMPP;
        #endregion

        #region Constructor
        public AttendanceBLL(IAttendanceMPP attendanceMPP, ICourseMPP courseMPP)
        {
            _attendanceMPP = attendanceMPP;
            _courseMPP = courseMPP;
        }
        #endregion

        #region Métodos
        public bool RegisterAttendanceBulk(int courseId, DateTime date, List<AttendanceBE> records)
        {
            CourseBE course = _courseMPP.FindById(courseId);

            if (course == null)
            {
                throw new InvalidOperationException("El curso seleccionado no existe.");
            }

            if (date.Date < course.StartDate.Date || date.Date > course.EndDate.Date)
            {
                throw new InvalidOperationException(
                    "La fecha de la asistencia debe estar dentro del período del curso. (Código: BIZ-017)");
            }

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
