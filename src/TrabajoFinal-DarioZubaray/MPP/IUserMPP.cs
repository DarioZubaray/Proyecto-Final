using System;
using System.Collections.Generic;

using BE;

namespace MPP
{
    public interface IUserMPP
    {
        UserBE ObtenerPorUserName(string userName);
        bool ActualizarLastUpdate(int userId, DateTime lastUpdate);
        bool ActualizarRetries(int userId, int retriesCount);
        bool Desactivar(int userId);
        bool Baja(UserBE user);
        bool Guardar(UserBE user);
        UserBE ListarObjeto(UserBE objeto);
        List<UserBE> ListarTodo();
    }
}
