using System.Collections.Generic;
using BE.Entities;
using BLL.Interfaces;
using MPP;

namespace BLL.Services
{
    public class AulaBLL : IAulaBLL
    {
        #region Propiedades
        private readonly IAulaMPP _aulaMPP;
        #endregion

        #region Constructor
        public AulaBLL(IAulaMPP aulaMPP)
        {
            _aulaMPP = aulaMPP;
        }
        #endregion

        #region Métodos
        public bool Save(AulaBE aula)
        {
            return _aulaMPP.Save(aula);
        }

        public bool Delete(AulaBE aula)
        {
            return _aulaMPP.Delete(aula);
        }

        public AulaBE FindById(int id)
        {
            return _aulaMPP.FindById(id);
        }

        public List<AulaBE> FindAll()
        {
            return _aulaMPP.FindAll();
        }

        public List<AulaBE> FindByName(string nombre)
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
