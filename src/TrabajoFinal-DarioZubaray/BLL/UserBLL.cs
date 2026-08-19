using System.Collections.Generic;

using BE;
using MPP;

namespace BLL
{
    public class UserBLL
    {
        private UserMPP mapeador;

        public UserBLL()
        {
            mapeador = new UserMPP();
        }

        public bool Baja(UserBE user)
        {
            return mapeador.Baja(user);
        }

        public bool Guardar(UserBE user)
        {
            return mapeador.Guardar(user);
        }

        public UserBE ListarObjeto(UserBE user)
        {
            throw new System.NotImplementedException();
        }

        public List<UserBE> ListarTodo()
        {
            return mapeador.ListarTodo();
        }
    }
}
