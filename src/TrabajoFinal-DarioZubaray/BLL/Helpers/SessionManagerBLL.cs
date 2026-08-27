using System.Collections.Generic;

using BE;
using MPP;

namespace BLL
{
    public class SessionManagerBLL
    {
        #region Propiedades
        private static Dictionary<int, SessionManagerBLL> _instances
            = new Dictionary<int, SessionManagerBLL>();

        public UserBE User { get; private set; }
        public RoleCompositeBE RoleTree { get; private set; }
        #endregion

        #region Constructor
        private SessionManagerBLL(UserBE user, RoleCompositeBE roleTree)
        {
            User = user;
            RoleTree = roleTree;
        }
        #endregion

        #region Métodos Públicos
        public static SessionManagerBLL GetInstance(int userId)
        {
            if (!_instances.ContainsKey(userId))
            {
                return null;
            }
            return _instances[userId];
        }

        public static SessionManagerBLL CreateSession(UserBE user)
        {
            if (user == null)
            {
                return null;
            }

            var permissionBLL = new PermissionBLL();
            RoleCompositeBE roleTree = permissionBLL.BuildRoleTree(user.RoleId);

            var session = new SessionManagerBLL(user, roleTree);
            _instances[user.Id] = session;

            CultureHelperBLL.SetCulture(user.Language);

            return session;
        }

        public static void RemoveSession(int userId)
        {
            if (_instances.ContainsKey(userId))
            {
                _instances.Remove(userId);
            }
        }

        public void UpdateLanguage(string language)
        {
            if (User != null)
            {
                User.Language = language;
                CultureHelperBLL.SetCulture(language);
            }
        }

        public bool HasPermission(string permissionName)
        {
            if (RoleTree == null)
            {
                return false;
            }
            return RoleTree.HasPermission(permissionName);
        }
        #endregion
    }
}
