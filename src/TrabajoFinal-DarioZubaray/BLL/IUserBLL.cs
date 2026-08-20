using System.Collections.Generic;

using BE;

namespace BLL
{
    public interface IUserBLL
    {
        bool Baja(UserBE user);
        bool Guardar(UserBE user);
        UserBE ListarObjeto(UserBE user);
        List<UserBE> ListarTodo();
    }
}
