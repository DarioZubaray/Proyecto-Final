using System.Collections.Generic;
using BE.Entities;

namespace BLL.Interfaces
{
    public interface IAulaBLL
    {
        bool Save(AulaBE aula);
        bool Delete(AulaBE aula);
        AulaBE FindById(int id);
        List<AulaBE> FindAll();
        List<AulaBE> FindByName(string nombre);
        int Count();
    }
}
