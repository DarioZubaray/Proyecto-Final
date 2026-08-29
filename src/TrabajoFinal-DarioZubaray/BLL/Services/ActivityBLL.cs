using System.Collections.Generic;

using BE;
using MPP;

namespace BLL
{
    public class ActivityBLL : IActivityBLL
    {
        #region Propiedades
        private readonly IActivityMPP _activityMPP;
        #endregion

        #region Constructores
        public ActivityBLL() : this(new MPP.ActivityMPP())
        {
        }

        public ActivityBLL(IActivityMPP activityMPP)
        {
            _activityMPP = activityMPP;
        }
        #endregion

        #region Registro de actividades (Decorator)
        public void LogFormAccess(int userId, string formName, string description = null)
        {
            IActivity activity = new BaseActivity(userId, ActivityActions.FormAccess, formName, description);
            new ActivityLoggingDecorator(activity, _activityMPP).Execute();
        }

        public void LogLogin(int userId, string description = null)
        {
            IActivity activity = new BaseActivity(userId, ActivityActions.Login, null, description);
            new ActivityLoggingDecorator(activity, _activityMPP).Execute();
        }

        public void LogLogout(int userId, string description = null)
        {
            IActivity activity = new BaseActivity(userId, ActivityActions.Logout, null, description);
            new ActivityLoggingDecorator(activity, _activityMPP).Execute();
        }
        #endregion

        #region Consulta paginada
        public List<ActivityLogBE> GetByUserPaginated(int userId, int page, int pageSize)
        {
            return _activityMPP.GetByUserPaginated(userId, page, pageSize);
        }

        public int CountByUser(int userId)
        {
            return _activityMPP.CountByUser(userId);
        }

        public int TotalPages(int userId, int pageSize)
        {
            if (pageSize < 1)
            {
                pageSize = 10;
            }

            int total = _activityMPP.CountByUser(userId);
            int pages = (int)System.Math.Ceiling(total / (double)pageSize);
            return pages < 1 ? 1 : pages;
        }
        #endregion
    }

    public static class ActivityActions
    {
        public const string FormAccess = "FORM_ACCESS";
        public const string Login = "LOGIN";
        public const string Logout = "LOGOUT";
    }
}
