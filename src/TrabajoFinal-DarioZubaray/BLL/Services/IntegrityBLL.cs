using System;
using System.Collections.Generic;

using BE.DTOs;
using BLL.Helpers;
using BLL.Interfaces;

namespace BLL.Services
{
    public class IntegrityBLL : IIntegrityBLL
    {
        #region Propiedades
        private readonly List<IDvhTableVerifierBLL> _verifiers;
        #endregion

        #region Constructor
        public IntegrityBLL(IEnumerable<IDvhTableVerifierBLL> verifiers)
        {
            _verifiers = new List<IDvhTableVerifierBLL>();

            if (verifiers != null)
            {
                _verifiers.AddRange(verifiers);
            }
        }
        #endregion

        #region Métodos
        public List<IntegrityViolationBE> Scan()
        {
            var violations = new List<IntegrityViolationBE>();

            foreach (IDvhTableVerifierBLL verifier in _verifiers)
            {
                violations.AddRange(verifier.FindViolations());
            }

            IntegrityStateBLL.RecordScan(violations.Count);
            return violations;
        }

        public List<IntegrityViolationBE> GetViolations()
        {
            return Scan();
        }

        public bool AcceptAndRegenerate(IntegrityViolationBE violation)
        {
            IDvhTableVerifierBLL verifier = FindVerifier(violation != null ? violation.TableName : null);

            if (verifier == null)
            {
                return false;
            }

            bool resolved = verifier.AcceptAndRegenerate(violation);
            Scan();
            return resolved;
        }

        private IDvhTableVerifierBLL FindVerifier(string tableName)
        {
            foreach (IDvhTableVerifierBLL verifier in _verifiers)
            {
                if (string.Equals(verifier.TableName, tableName, StringComparison.OrdinalIgnoreCase))
                {
                    return verifier;
                }
            }

            return null;
        }
        #endregion
    }
}
