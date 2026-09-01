using System;

namespace BE.Entities
{
    public class ActivityLogBE
    {
        #region Propiedades
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public string FormName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        #endregion

        #region Constructores
        public ActivityLogBE() { }

        public ActivityLogBE(int userId, string action, string formName, string description)
        {
            UserId = userId;
            Action = action;
            FormName = formName;
            Description = description;
            CreatedAt = DateTime.Now;
        }
        #endregion

        #region Métodos
        public override string ToString()
        {
            return $"{CreatedAt}, {Action}, {FormName}";
        }
        #endregion
    }
}
