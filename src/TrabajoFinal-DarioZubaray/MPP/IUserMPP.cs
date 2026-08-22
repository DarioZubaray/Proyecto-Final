using System;
using System.Collections.Generic;

using BE;

namespace MPP
{
    public interface IUserMPP
    {
        UserBE GetByUserName(string userName);
        bool UpdateLastUpdate(int userId, DateTime lastUpdate);
        bool UpdateRetries(int userId, int retriesCount);
        bool Deactivate(int userId);
        bool Delete(UserBE user);
        bool Save(UserBE user);
        UserBE FindById(UserBE user);
        List<UserBE> FindAll();
    }
}
