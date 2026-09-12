using System;
using System.Collections.Generic;

using BE.Entities;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class CourseBLL : ICourseBLL
    {
        #region Propiedades
        private readonly ICourseMPP _courseMPP;
        #endregion

        #region Constructor
        public CourseBLL(ICourseMPP courseMPP)
        {
            _courseMPP = courseMPP;
        }
        #endregion

        #region Métodos
        public bool Save(CourseBE course)
        {
            bool hasOverlap = _courseMPP.ExistsClassroomOverlap(
                course.ClassroomId,
                course.StartDate,
                course.EndDate,
                course.Id,
                course.DayOfWeek,
                course.StartTime,
                course.EndTime
            );

            if (hasOverlap)
            {
                throw new InvalidOperationException("El aula ya está ocupada en ese horario.");
            }

            return _courseMPP.Save(course);
        }

        public bool SaveTeachers(int courseId, List<int> teacherIds)
        {
            return _courseMPP.SaveTeachers(courseId, teacherIds);
        }

        public bool ValidateTeacherOverlap(int courseId, int? dayOfWeek, TimeSpan? startTime, TimeSpan? endTime, List<int> teacherIds)
        {
            if (!dayOfWeek.HasValue || !startTime.HasValue || !endTime.HasValue)
            {
                return false;
            }

            foreach (int teacherId in teacherIds)
            {
                if (_courseMPP.ExistsTeacherOverlap(teacherId, dayOfWeek.Value, startTime.Value, endTime.Value, courseId))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Delete(CourseBE course)
        {
            return _courseMPP.Delete(course);
        }

        public CourseBE FindById(int id)
        {
            return _courseMPP.FindById(id);
        }

        public List<CourseBE> FindAll()
        {
            return _courseMPP.FindAll();
        }

        public List<CourseBE> FindAllIncludingInactive()
        {
            return _courseMPP.FindAllIncludingInactive();
        }

        public List<CourseBE> FindByName(string name)
        {
            return _courseMPP.FindByName(name);
        }

        public bool ExistsClassroomOverlap(int classroomId, DateTime startDate, DateTime endDate, int courseIdToExclude, int? dayOfWeek, TimeSpan? startTime, TimeSpan? endTime)
        {
            return _courseMPP.ExistsClassroomOverlap(classroomId, startDate, endDate, courseIdToExclude, dayOfWeek, startTime, endTime);
        }

        public List<UserBE> GetTeachersByCourseId(int courseId)
        {
            return _courseMPP.GetTeachersByCourseId(courseId);
        }

        public List<CourseBE> FindByTeacherId(int teacherId)
        {
            return _courseMPP.FindByTeacherId(teacherId);
        }

        public List<CourseBE> FindByStudentId(int studentId)
        {
            return _courseMPP.FindByStudentId(studentId);
        }
        #endregion
    }
}
