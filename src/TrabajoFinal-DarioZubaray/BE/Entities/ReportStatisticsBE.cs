namespace BE.Entities
{
    public class ReportStatisticsBE
    {
        #region Propiedades
        public int TotalStudents { get; set; }
        public int StudentsWithGrades { get; set; }
        public decimal? GeneralAverage { get; set; }
        public int Approved { get; set; }
        public int Failed { get; set; }
        public int? AverageAttendance { get; set; }
        #endregion
    }
}
