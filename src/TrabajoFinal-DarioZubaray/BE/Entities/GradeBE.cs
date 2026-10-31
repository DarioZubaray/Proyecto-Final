namespace BE.Entities
{
    public class GradeBE
    {
        #region Propiedades
        public int Id { get; set; }
        public int EvaluationId { get; set; }
        public int StudentId { get; set; }
        public decimal Grade { get; set; }
        #endregion
    }
}
