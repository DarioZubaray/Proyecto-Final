using System;
using System.Collections.Generic;

using BE.DTOs;
using BE.Entities;
using BLL.Helpers;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class EvaluationDvhVerifierBLL : IDvhTableVerifierBLL
    {
        #region Propiedades
        private readonly IEvaluationMPP _evaluationMPP;

        public const string TABLE_NAME = "evaluations";
        #endregion

        #region Constructor
        public EvaluationDvhVerifierBLL(IEvaluationMPP evaluationMPP)
        {
            _evaluationMPP = evaluationMPP;
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

            foreach (EvaluationBE evaluation in _evaluationMPP.FindAll())
            {
                string computed = DVHHelperBLL.Compute(evaluation.GetDvhValues());

                if (!string.Equals(evaluation.Dvh ?? string.Empty, computed, StringComparison.Ordinal))
                {
                    violations.Add(new IntegrityViolationBE
                    {
                        TableName = TABLE_NAME,
                        RowId = evaluation.Id,
                        Description = evaluation.Name,
                        StoredHash = evaluation.Dvh,
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

            EvaluationBE evaluation = _evaluationMPP.FindById(violation.RowId);

            if (evaluation == null)
            {
                return false;
            }

            return DVHHelperBLL.Regenerate(_evaluationMPP, evaluation);
        }
        #endregion
    }
}
