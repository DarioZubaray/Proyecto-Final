using System;
using System.Collections.Generic;

using BE.Entities;

namespace MPP.Interfaces
{
    public interface IEnrollmentMPP
    {
        bool Enroll(int courseId, int studentId);
        bool Unenroll(int courseId, int studentId);
        EnrollmentBE FindById(int id);
        List<EnrollmentBE> FindByStudentId(int studentId);
        List<EnrollmentBE> FindAll();
        int CountStudentsByCourseId(int courseId);
        bool ExistsActiveEnrollment(int courseId, int studentId);
        bool ExistsScheduleOverlap(int studentId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, int courseIdToExclude);
    }
}
