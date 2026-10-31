namespace BE.Entities
{
    public class AttendanceReportLineBE
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int TotalPlans { get; set; }
        public int PresentPlans { get; set; }
        public int? AttendancePercent { get; set; }
    }
}
