using System.Collections.Generic;

using BE.Entities;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class ClassroomBLL : IClassroomBLL
    {
        #region Propiedades
        private readonly IClassroomMPP _classroomMPP;
        #endregion

        #region Constructor
        public ClassroomBLL(IClassroomMPP classroomMPP)
        {
            _classroomMPP = classroomMPP;
        }
        #endregion

        #region Métodos
        public bool Save(ClassroomBE classroom)
        {
            return _classroomMPP.Save(classroom);
        }

        public bool Delete(ClassroomBE classroom)
        {
            return _classroomMPP.Delete(classroom);
        }

        public ClassroomBE FindById(int id)
        {
            return _classroomMPP.FindById(id);
        }

        public List<ClassroomBE> FindAll()
        {
            return _classroomMPP.FindAll();
        }

        public List<ClassroomBE> FindByName(string name)
        {
            return _classroomMPP.FindByName(name);
        }

        public int Count()
        {
            return _classroomMPP.Count();
        }
        #endregion
    }
}
