using System.Collections.Generic;

using BE;
using MPP;

namespace BLL
{
    public class UserBLL : IUserBLL
    {
        #region Propiedades
        private readonly IUserMPP _userMPP;
        #endregion

        #region Constructores
        public UserBLL() : this(new MPP.UserMPP())
        {
        }

        public UserBLL(IUserMPP userMPP)
        {
            _userMPP = userMPP;
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
            return _userMPP.ListarObjeto(user);
        }

        public List<UserBE> ListarTodo()
        {
            return _userMPP.ListarTodo();
        }
        #endregion
    }
}
