using BE.Entities;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class ActivityLoggingDecoratorBLL : IActivityLogEntryBLL
    {
        #region Propiedades
        private readonly IActivityLogEntryBLL _activity;
        private readonly IActivityMPP _activityMPP;
        #endregion

        #region Constructor
        public ActivityLoggingDecoratorBLL(IActivityLogEntryBLL activity, IActivityMPP activityMPP)
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
