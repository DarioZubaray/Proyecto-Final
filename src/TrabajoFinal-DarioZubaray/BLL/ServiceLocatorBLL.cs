using MPP;

namespace BLL
{
    public static class ServiceLocatorBLL
    {
        #region Propiedades
        private static IUserMPP _userMPP;
        private static IRoleMPP _roleMPP;
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

        public static IRoleMPP GetRoleMPP()
        {
            if (_roleMPP == null)
            {
                _roleMPP = new RoleMPP();
            }

            return _roleMPP;
        }

        public static IAuthBLL CreateAuthBLL()
        {
            return new AuthBLL(GetUserMPP());
        }

        public static IUserBLL CreateUserBLL()
        {
            return new UserBLL(GetUserMPP());
        }

        public static PermissionBLL CreatePermissionBLL()
        {
            return new PermissionBLL(GetRoleMPP());
        }

        public static IRoleBLL CreateRoleBLL()
        {
            return new RoleBLL(GetRoleMPP());
        }
        #endregion
    }
}
