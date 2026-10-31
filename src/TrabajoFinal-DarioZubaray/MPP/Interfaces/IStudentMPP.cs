using System;
using System.Collections.Generic;

using BE.Entities;

namespace MPP.Interfaces
{
    public interface IStudentMPP
    {
        List<StudentBE> FindAll();
        List<StudentBE> FindAllActive();
        StudentBE FindById(int id);
        List<StudentBE> FindByName(string searchText);
        StudentBE FindByDocument(string document);
        bool Insert(StudentBE student);
        bool Update(StudentBE student);
        bool Delete(int id);
    }
}
