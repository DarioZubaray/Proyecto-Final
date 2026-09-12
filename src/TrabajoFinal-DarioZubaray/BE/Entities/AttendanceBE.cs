using System;

namespace BE.Entities
{
    public class AttendanceBE
    {
        #region Propiedades
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
        public DateTime CreatedAt { get; set; }

        public string StudentName { get; set; }
        public string CourseName { get; set; }
        #endregion

        #region Constructor
        public AttendanceBE()
        {
        }
        #endregion
    }
}
