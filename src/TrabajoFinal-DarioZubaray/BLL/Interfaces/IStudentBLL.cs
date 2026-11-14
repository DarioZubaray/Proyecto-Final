using System;
using System.Collections.Generic;

using BE.Entities;

namespace BLL.Interfaces
{
    public interface IStudentBLL
    {
        List<StudentBE> FindAll();
        List<StudentBE> FindAllActive();
        StudentBE FindById(int id);
        List<StudentBE> FindByName(string searchText);
        bool Save(StudentBE student);
        bool Delete(StudentBE student);
    }
}
