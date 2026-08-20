using System.Collections.Generic;

using BE;
using MPP;

namespace BLL
{
    public class UserBLL
    {
        #region Propiedades
        private UserMPP _userMPP;
        #endregion

        #region Constructores
        public UserBLL()
        {
            _userMPP = new UserMPP();
        }
        #endregion

        #region Métodos
        public bool Baja(UserBE user)
        {
            return _userMPP.Baja(user);
        }

        public bool Guardar(UserBE user)
        {
            return _userMPP.Guardar(user);
        }

        public UserBE ListarObjeto(UserBE user)
        {
            throw new System.NotImplementedException();
        }

        public List<UserBE> ListarTodo()
        {
            return _userMPP.ListarTodo();
        }
        #endregion
    }
}
