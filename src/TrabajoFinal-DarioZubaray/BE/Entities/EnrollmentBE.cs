using System;

namespace BE.Entities
{
    public class EnrollmentBE
    {
        #region Propiedades
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CourseName { get; set; }
        public string StudentName { get; set; }
        public int? DayOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string ClassroomName { get; set; }
        #endregion

        #region Constructor
        public EnrollmentBE()
        {
        }
        #endregion
    }
}
