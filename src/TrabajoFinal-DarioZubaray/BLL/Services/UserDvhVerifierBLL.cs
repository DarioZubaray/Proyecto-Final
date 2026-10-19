using System;
using System.Collections.Generic;

using BE.DTOs;
using BE.Entities;
using BLL.Helpers;
using BLL.Interfaces;

namespace BLL.Services
{
    public class UserDvhVerifierBLL : IDvhTableVerifierBLL
    {
        #region Propiedades
        private readonly IUserBLL _userBLL;

        public const string TABLE_NAME = "users";
        #endregion

        #region Constructor
        public UserDvhVerifierBLL(IUserBLL userBLL)
        {
            _userBLL = userBLL;
        }
        #endregion

        #region Métodos
        public string TableName
        {
            get { return TABLE_NAME; }
        }

        public List<IntegrityViolationBE> FindViolations()
        {
            var violations = new List<IntegrityViolationBE>();

            foreach (UserBE user in _userBLL.FindAll())
            {
                string computed = DVHHelperBLL.Compute(user.GetDvhValues());

                if (!string.Equals(user.Dvh ?? string.Empty, computed, StringComparison.Ordinal))
                {
                    violations.Add(new IntegrityViolationBE
                    {
                        TableName = TABLE_NAME,
                        RowId = user.Id,
                        Description = user.UserName,
                        StoredHash = user.Dvh,
                        ComputedHash = computed
                    });
                }
            }

            return violations;
        }

        public bool AcceptAndRegenerate(IntegrityViolationBE violation)
        {
            if (violation == null || violation.RowId <= 0)
            {
                return false;
            }

            UserBE user = _userBLL.FindById(new UserBE { Id = violation.RowId });

            if (user == null)
            {
                return false;
            }

            return _userBLL.Save(user);
        }
        #endregion
    }
}
