namespace BLL
{
    /// <summary>
    /// Interfaz componente del patrón Decorator.
    /// Representa una "actividad" que puede ejecutarse y sobre la cual
    /// se pueden agregar responsabilidades (por ejemplo, registrar el
    /// historial de actividad al finalizar).
    /// </summary>
    public interface IActivity
    {
        int UserId { get; }
        string Action { get; }
        string FormName { get; }
        string Description { get; }
        bool Execute();
    }
}
