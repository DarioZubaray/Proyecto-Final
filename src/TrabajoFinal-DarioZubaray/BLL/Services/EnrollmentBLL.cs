using System;
using System.Collections.Generic;

using BE.Entities;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class EnrollmentBLL : IEnrollmentBLL
    {
        #region Propiedades
        private readonly IEnrollmentMPP _enrollmentMPP;
        private readonly ICourseBLL _courseBLL;
        #endregion

        #region Constructor
        public EnrollmentBLL(IEnrollmentMPP enrollmentMPP, ICourseBLL courseBLL)
        {
            _enrollmentMPP = enrollmentMPP;
            _courseBLL = courseBLL;
        }
        #endregion

        #region Métodos
        public bool Enroll(int courseId, int studentId)
        {
            if (_enrollmentMPP.ExistsActiveEnrollment(courseId, studentId))
            {
                throw new InvalidOperationException("Ya está inscripto en este curso.");
            }

            CourseBE newCourse = _courseBLL.FindById(courseId);
            if (newCourse == null)
            {
                throw new InvalidOperationException("El curso seleccionado no existe.");
            }

            if (!newCourse.DayOfWeek.HasValue || !newCourse.StartTime.HasValue || !newCourse.EndTime.HasValue)
            {
                throw new InvalidOperationException("El curso seleccionado no tiene horario definido.");
            }

            List<EnrollmentBE> currentEnrollments = _enrollmentMPP.FindByStudentId(studentId);

            foreach (var enrollment in currentEnrollments)
            {
                if (enrollment.CourseId == courseId)
                {
                    continue;
                }

                if (enrollment.DayOfWeek.HasValue && enrollment.StartTime.HasValue && enrollment.EndTime.HasValue)
                {
                    if (enrollment.DayOfWeek.Value != newCourse.DayOfWeek.Value)
                    {
                        continue;
                    }

                    bool overlap = ExistsScheduleOverlap(
                        studentId,
                        newCourse.DayOfWeek.Value,
                        newCourse.StartTime.Value,
                        newCourse.EndTime.Value,
                        courseId
                    );

                    if (overlap)
                    {
                        throw new InvalidOperationException(
                            $"Tiene traslape de horario con el curso '{enrollment.CourseName}' ({GetDayOfWeekName(enrollment.DayOfWeek.Value)} {enrollment.StartTime.Value:hh\\:mm} - {enrollment.EndTime.Value:hh\\:mm}).");
                    }
                }
            }

            return _enrollmentMPP.Enroll(courseId, studentId);
        }

        public bool Unenroll(int courseId, int studentId)
        {
            return _enrollmentMPP.Unenroll(courseId, studentId);
        }

        public EnrollmentBE FindById(int id)
        {
            return _enrollmentMPP.FindById(id);
        }

        public List<EnrollmentBE> FindByStudentId(int studentId)
        {
            return _enrollmentMPP.FindByStudentId(studentId);
        }

        public List<EnrollmentBE> FindAll()
        {
            return _enrollmentMPP.FindAll();
        }

        public int CountStudentsByCourseId(int courseId)
        {
            return _enrollmentMPP.CountStudentsByCourseId(courseId);
        }

        public bool ExistsActiveEnrollment(int courseId, int studentId)
        {
            return _enrollmentMPP.ExistsActiveEnrollment(courseId, studentId);
        }

        public bool ExistsScheduleOverlap(int studentId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, int courseIdToExclude)
        {
            return _enrollmentMPP.ExistsScheduleOverlap(studentId, dayOfWeek, startTime, endTime, courseIdToExclude);
        }

        private string GetDayOfWeekName(int day)
        {
            switch (day)
            {
                case 1: return "Lunes";
                case 2: return "Martes";
                case 3: return "Miércoles";
                case 4: return "Jueves";
                case 5: return "Viernes";
                case 6: return "Sábado";
                case 7: return "Domingo";
                default: return $"Día {day}";
            }
        }
        #endregion
    }
}
