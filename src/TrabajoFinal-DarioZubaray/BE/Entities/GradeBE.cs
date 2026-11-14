namespace BE.Entities
{
    public class GradeBE
    {
        #region Propiedades
        public int Id { get; set; }
        public int EvaluationId { get; set; }
        public int StudentId { get; set; }
        public decimal Grade { get; set; }
        public string Dvh { get; set; }
        #endregion

        #region Métodos
        public object[] GetDvhValues()
        {
            return new object[]
            {
                Id,             // 0
                EvaluationId,   // 1
                StudentId,      // 2
                Grade           // 3
            };
        }
        #endregion
    }
}
