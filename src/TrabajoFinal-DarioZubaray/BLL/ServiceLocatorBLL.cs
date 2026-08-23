using MPP;

namespace BLL
{
    public static class ServiceLocatorBLL
    {
        #region Propiedades
        private static IUserMPP _userMPP;
        #endregion

        #region Métodos
        public static IUserMPP GetUserMPP()
        {
            if (_userMPP == null)
            {
                _userMPP = new UserMPP();
            }

            return _userMPP;
        }

        public static IAuthBLL CreateAuthBLL()
        {
            return new AuthBLL(GetUserMPP());
        }

        public static IUserBLL CreateUserBLL()
        {
            return new UserBLL(GetUserMPP());
        }
        #endregion
    }
}
