using System.Collections.Generic;

using BE;
using MPP;

namespace BLL
{
    public class UserBLL
    {
        #region Propiedades
        private UserMPP mapeador;
        #endregion

        #region Constructores
        public UserBLL()
        {
            mapeador = new UserMPP();
        }
        #endregion

        #region Métodos
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
        #endregion
    }
}
