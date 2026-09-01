using System.Collections.Generic;
using BE.Entities;

namespace BLL.Interfaces
{
    public interface IActivityBLL
    {
        void LogFormAccess(int userId, string formName, string description = null);
        void LogLogin(int userId, string description = null);
        void LogLogout(int userId, string description = null);
        List<ActivityLogBE> GetByUserPaginated(int userId, int page, int pageSize);
        int CountByUser(int userId);
        int TotalPages(int userId, int pageSize);
    }
}
