using System;
using System.Collections.Generic;

using BE.Entities;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class StudentBLL : IStudentBLL
    {
        #region Propiedades
        private readonly IStudentMPP _studentMPP;
        #endregion

        #region Constructor
        public StudentBLL(IStudentMPP studentMPP)
        {
            _studentMPP = studentMPP;
        }
        #endregion

        #region Métodos
        public List<StudentBE> FindAll()
        {
            return _studentMPP.FindAll();
        }

        public List<StudentBE> FindAllActive()
        {
            return _studentMPP.FindAllActive();
        }

        public StudentBE FindById(int id)
        {
            return _studentMPP.FindById(id);
        }

        public List<StudentBE> FindByName(string searchText)
        {
            return _studentMPP.FindByName(searchText);
        }

        public bool Save(StudentBE student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student));
            }

            if (string.IsNullOrWhiteSpace(student.FirstName))
            {
                throw new InvalidOperationException("El nombre del alumno es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(student.LastName))
            {
                throw new InvalidOperationException("El apellido del alumno es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(student.Document))
            {
                throw new InvalidOperationException("El documento del alumno es obligatorio.");
            }

            student.FirstName = student.FirstName.Trim();
            student.LastName = student.LastName.Trim();
            student.Document = student.Document.Trim();

            StudentBE existing = _studentMPP.FindByDocument(student.Document);
            if (existing != null && existing.Id != student.Id)
            {
                throw new InvalidOperationException("Ya existe un alumno registrado con ese documento.");
            }

            student.LastUpdate = DateTime.Now;

            if (student.Id == 0)
            {
                student.CreatedAt = DateTime.Now;
                student.IsActive = true;
                return _studentMPP.Insert(student);
            }

            return _studentMPP.Update(student);
        }

        public bool Delete(StudentBE student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student));
            }

            return _studentMPP.Delete(student.Id);
        }
        #endregion
    }
}
