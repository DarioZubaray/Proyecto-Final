namespace BLL
{
    /// <summary>
    /// Componente concreto del patrón Decorator.
    /// Representa una actividad concreta (acceso a un formulario, inicio o
    /// cierre de sesión). Ejecutarla no produce efectos secundarios; la
    /// responsabilidad de registrar queda a cargo de un decorador.
    /// </summary>
    public class BaseActivity : IActivity
    {
        #region Propiedades
        public int UserId { get; private set; }
        public string Action { get; private set; }
        public string FormName { get; private set; }
        public string Description { get; private set; }
        #endregion

        #region Constructor
        public BaseActivity(int userId, string action, string formName = null, string description = null)
        {
            UserId = userId;
            Action = action;
            FormName = formName;
            Description = description;
        }
        #endregion

        #region Métodos
        public bool Execute()
        {
            return true;
        }
        #endregion
    }
}
