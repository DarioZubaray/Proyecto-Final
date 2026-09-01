using System.Collections.Generic;
using BE.Entities;

namespace MPP
{
    public interface IActivityMPP
    {
        bool Save(ActivityLogBE log);
        List<ActivityLogBE> GetByUserPaginated(int userId, int page, int pageSize);
        int CountByUser(int userId);
    }
}
