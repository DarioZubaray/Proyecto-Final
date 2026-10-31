namespace BE.Entities
{
    public class GradeReportLineBE
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int EvaluationsCount { get; set; }
        public int GradesCount { get; set; }
        public decimal? Average { get; set; }
    }
}
