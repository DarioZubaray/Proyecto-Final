using System.Collections.Generic;

using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class ClassroomBE : IClassroomBLL
    {
        #region Propiedades
        private readonly IClassroomMPP _aulaMPP;
        #endregion

        #region Constructor
        public ClassroomBE(IClassroomMPP aulaMPP)
        {
            _aulaMPP = aulaMPP;
        }
        #endregion

        #region Métodos
        public bool Save(BE.Entities.ClassroomBE aula)
        {
            return _aulaMPP.Save(aula);
        }

        public bool Delete(BE.Entities.ClassroomBE aula)
        {
            return _aulaMPP.Delete(aula);
        }

        public BE.Entities.ClassroomBE FindById(int id)
        {
            return _aulaMPP.FindById(id);
        }

        public List<BE.Entities.ClassroomBE> FindAll()
        {
            return _aulaMPP.FindAll();
        }

        public List<BE.Entities.ClassroomBE> FindByName(string nombre)
        {
            return _aulaMPP.FindByName(nombre);
        }

        public int Count()
        {
            return _aulaMPP.Count();
        }
        #endregion
    }
}
