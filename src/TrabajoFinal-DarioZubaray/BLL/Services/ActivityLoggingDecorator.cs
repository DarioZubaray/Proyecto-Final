using BE.Entities;
using MPP;

namespace BLL.Services
{
    public class ActivityLoggingDecorator : IActivity
    {
        #region Propiedades
        private readonly IActivity _activity;
        private readonly IActivityMPP _activityMPP;
        #endregion

        #region Constructor
        public ActivityLoggingDecorator(IActivity activity, IActivityMPP activityMPP)
        {
            _activity = activity;
            _activityMPP = activityMPP;
        }
        #endregion

        #region Propiedades delegadas
        public int UserId => _activity.UserId;
        public string Action => _activity.Action;
        public string FormName => _activity.FormName;
        public string Description => _activity.Description;
        #endregion

        #region Métodos
        public bool Execute()
        {
            bool result = _activity.Execute();

            var log = new ActivityLogBE(
                _activity.UserId,
                _activity.Action,
                _activity.FormName,
                _activity.Description);

            _activityMPP.Save(log);

            return result;
        }
        #endregion
    }
}
