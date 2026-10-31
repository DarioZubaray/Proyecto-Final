using System;

namespace BE.Entities
{
    public class EvaluationBE
    {
        #region Propiedades
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdate { get; set; }
        #endregion
    }
}
