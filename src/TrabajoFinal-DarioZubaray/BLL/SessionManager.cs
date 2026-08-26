using System.Collections.Generic;

using BE;
using MPP;

namespace BLL
{
    public class SessionManager
    {
        #region Propiedades
        private static Dictionary<int, SessionManager> _instances
            = new Dictionary<int, SessionManager>();

        public UserBE User { get; private set; }
        public RoleComposite RoleTree { get; private set; }
        #endregion

        #region Constructor
        private SessionManager(UserBE user, RoleComposite roleTree)
        {
            User = user;
            RoleTree = roleTree;
        }
        #endregion

        #region Métodos Públicos
        public static SessionManager GetInstance(int userId)
        {
            if (!_instances.ContainsKey(userId))
            {
                return null;
            }
            return _instances[userId];
        }

        public static SessionManager CreateSession(UserBE user)
        {
            if (user == null)
            {
                return null;
            }

            var permissionBLL = new PermissionBLL();
            RoleComposite roleTree = permissionBLL.BuildRoleTree(user.RoleId);

            var session = new SessionManager(user, roleTree);
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
