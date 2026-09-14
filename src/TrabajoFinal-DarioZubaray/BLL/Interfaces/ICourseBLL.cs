using System;
using System.Collections.Generic;

using BE.Entities;

namespace BLL.Interfaces
{
    public interface ICourseBLL
    {
        bool Save(CourseBE course);
        bool Delete(CourseBE course);
        CourseBE FindById(int id);
        List<CourseBE> FindAll();
        List<CourseBE> FindAllIncludingInactive();
        List<CourseBE> FindByName(string name);
        bool ExistsClassroomOverlap(int classroomId, DateTime startDate, DateTime endDate, int courseIdToExclude, int? dayOfWeek, TimeSpan? startTime, TimeSpan? endTime);
        bool SaveTeachers(int courseId, List<int> teacherIds);
        bool ValidateTeacherOverlap(int courseId, int? dayOfWeek, TimeSpan? startTime, TimeSpan? endTime, List<int> teacherIds);
        List<UserBE> GetTeachersByCourseId(int courseId);
        List<CourseBE> FindByTeacherId(int teacherId);
        List<CourseBE> FindByStudentId(int studentId);
    }
}
