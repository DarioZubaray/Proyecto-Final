using System;
using System.Collections.Generic;

namespace BE.Entities
{
    public class CourseBE
    {
        #region Propiedades
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClassroomId { get; set; }
        public string ClassroomName { get; set; }
        public int? DayOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<UserBE> Teachers { get; set; }
        #endregion

        #region Constructor
        public CourseBE()
        {
            Teachers = new List<UserBE>();
        }

        public CourseBE(int id, string name, string description, DateTime startDate,
            DateTime endDate, int classroomId, int? dayOfWeek, TimeSpan? startTime, TimeSpan? endTime,
            bool isActive, DateTime createdAt, DateTime lastUpdate)
        {
            Id = id;
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            ClassroomId = classroomId;
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
            IsActive = isActive;
            CreatedAt = createdAt;
            LastUpdate = lastUpdate;
            Teachers = new List<UserBE>();
        }
        #endregion

        #region Métodos
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
