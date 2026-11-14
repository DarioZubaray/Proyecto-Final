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
        public string Dvh { get; set; }
        #endregion

        #region Métodos
        public object[] GetDvhValues()
        {
            return new object[]
            {
                Id,          // 0
                CourseId,    // 1
                Name,        // 2
                Date,        // 3
                CreatedBy,   // 4
                CreatedAt,   // 5
                LastUpdate   // 6
            };
        }
        #endregion
    }
}
