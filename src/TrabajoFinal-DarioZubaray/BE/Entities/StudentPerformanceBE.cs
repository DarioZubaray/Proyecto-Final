namespace BE.Entities
{
    public class StudentPerformanceBE
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int? AttendancePercent { get; set; }
        public int GradesCount { get; set; }
        public decimal? Average { get; set; }
    }
}
