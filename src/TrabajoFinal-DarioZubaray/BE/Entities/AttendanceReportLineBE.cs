namespace BE.Entities
{
    public class AttendanceReportLineBE
    {
        #region Propiedades
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int TotalPlans { get; set; }
        public int PresentPlans { get; set; }
        public int? AttendancePercent { get; set; }
        #endregion
    }
}
