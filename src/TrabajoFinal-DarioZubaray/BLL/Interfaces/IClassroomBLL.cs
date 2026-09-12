using System.Collections.Generic;

using BE.Entities;

namespace BLL.Interfaces
{
    public interface IClassroomBLL
    {
        bool Save(ClassroomBE aula);
        bool Delete(ClassroomBE aula);
        ClassroomBE FindById(int id);
        List<ClassroomBE> FindAll();
        List<ClassroomBE> FindByName(string nombre);
        int Count();
    }
}
