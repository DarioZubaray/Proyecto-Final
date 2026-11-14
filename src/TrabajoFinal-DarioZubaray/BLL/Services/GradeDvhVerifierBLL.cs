using System;
using System.Collections.Generic;
using System.Globalization;

using BE.DTOs;
using BE.Entities;
using BLL.Helpers;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class GradeDvhVerifierBLL : IDvhTableVerifierBLL
    {
        #region Propiedades
        private readonly IEvaluationMPP _evaluationMPP;

        public const string TABLE_NAME = "grades";
        #endregion

        #region Constructor
        public GradeDvhVerifierBLL(IEvaluationMPP evaluationMPP)
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

            foreach (GradeBE grade in _evaluationMPP.FindAllGrades())
            {
                string computed = DVHHelperBLL.Compute(grade.GetDvhValues());

                if (!string.Equals(grade.Dvh ?? string.Empty, computed, StringComparison.Ordinal))
                {
                    violations.Add(new IntegrityViolationBE
                    {
                        TableName = TABLE_NAME,
                        RowId = grade.Id,
                        Description = Describe(grade),
                        StoredHash = grade.Dvh,
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

            GradeBE grade = _evaluationMPP.FindGradeById(violation.RowId);

            if (grade == null)
            {
                return false;
            }

            return DVHHelperBLL.Regenerate(_evaluationMPP, grade);
        }

        private static string Describe(GradeBE grade)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Eval {0} / Alum {1} = {2}",
                grade.EvaluationId,
                grade.StudentId,
                DVHHelperBLL.Normalize(grade.Grade));
        }
        #endregion
    }
}
