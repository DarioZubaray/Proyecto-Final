using System.Collections.Generic;

using BE.Entities;

namespace MPP.Interfaces
{
    public interface IClassroomMPP
    {
        bool Save(ClassroomBE aula);
        bool Delete(ClassroomBE aula);
        ClassroomBE FindById(int id);
        List<ClassroomBE> FindAll();
        List<ClassroomBE> FindByName(string nombre);
        int Count();
    }
}
