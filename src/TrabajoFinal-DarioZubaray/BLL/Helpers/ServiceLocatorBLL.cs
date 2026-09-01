using BLL.Interfaces;
using BLL.Services;
using MPP;

namespace BLL.Helpers
{
    public static class ServiceLocatorBLL
    {
        #region Propiedades
        private static IUserMPP _userMPP;
        private static IRoleMPP _roleMPP;
        private static IActivityMPP _activityMPP;
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

        public static IActivityMPP GetActivityMPP()
        {
            if (_activityMPP == null)
            {
                _activityMPP = new ActivityMPP();
            }

            return _activityMPP;
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

        public static IActivityBLL CreateActivityBLL()
        {
            return new ActivityBLL(GetActivityMPP());
        }
        #endregion
    }
}
