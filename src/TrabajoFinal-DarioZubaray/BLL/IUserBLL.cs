using System.Collections.Generic;

using BE;

namespace BLL
{
    public interface IUserBLL
    {
        bool Delete(UserBE user);
        bool Save(UserBE user);
        UserBE FindById(UserBE user);
        List<UserBE> FindAll();
    }
}
