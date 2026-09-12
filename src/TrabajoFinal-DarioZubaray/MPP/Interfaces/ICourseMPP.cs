using System;
using System.Collections.Generic;

using BE.Entities;

namespace MPP.Interfaces
{
    public interface ICourseMPP
    {
        bool Save(CourseBE course);
        bool Delete(CourseBE course);
        CourseBE FindById(int id);
        List<CourseBE> FindAll();
        List<CourseBE> FindAllIncludingInactive();
        List<CourseBE> FindByName(string name);
        bool ExistsClassroomOverlap(int classroomId, DateTime startDate, DateTime endDate, int courseIdToExclude, int? dayOfWeek, TimeSpan? startTime, TimeSpan? endTime);
        bool SaveTeachers(int courseId, List<int> teacherIds);
        List<UserBE> GetTeachersByCourseId(int courseId);
        List<CourseBE> FindByTeacherId(int teacherId);
        List<CourseBE> FindByStudentId(int studentId);
        bool ExistsTeacherOverlap(int teacherId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, int courseIdToExclude);
    }
}
