using System.Collections.Generic;

using BE.Entities;

namespace MPP.Interfaces
{
    public interface IClassroomMPP
    {
        bool Save(ClassroomBE classroom);
        bool Delete(ClassroomBE classroom);
        ClassroomBE FindById(int id);
        List<ClassroomBE> FindAll();
        List<ClassroomBE> FindByName(string name);
        int Count();
    }
}
