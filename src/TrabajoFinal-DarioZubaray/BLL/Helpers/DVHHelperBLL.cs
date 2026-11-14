using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

using BE.Entities;
using MPP.Interfaces;

namespace BLL.Helpers
{
    public static class DVHHelperBLL
    {
        #region Propiedades
        public const string Separator = ",";
        #endregion

        #region Métodos
        public static string Compute(IEnumerable<object> values)
        {
            var normalized = new List<string>();

            if (values != null)
            {
                foreach (object value in values)
                {
                    normalized.Add(Normalize(value));
                }
            }

            string payload = string.Join(Separator, normalized);

            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.Unicode.GetBytes(payload));

                var builder = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public static string Compute(params object[] values)
        {
            return Compute((IEnumerable<object>)values);
        }

        public static string Normalize(object value)
        {
            if (value == null || value is DBNull)
            {
                return string.Empty;
            }

            if (value is bool)
            {
                return (bool)value ? "1" : "0";
            }

            if (value is DateTime)
            {
                return FormatDate((DateTime)value);
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        public static string FormatDate(DateTime value)
        {
            if (value.Millisecond == 0)
            {
                return value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            }

            return value.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        public static bool Regenerate(IUserMPP mpp, UserBE user)
        {
            if (mpp == null || user == null)
            {
                return false;
            }

            user.Dvh = Compute(user.GetDvhValues());

            return mpp.UpdateDvh(user.Id, user.Dvh);
        }

        public static bool Regenerate(IUserMPP mpp, int userId)
        {
            if (mpp == null)
            {
                return false;
            }

            UserBE user = mpp.FindById(new UserBE { Id = userId });

            if (user == null)
            {
                return true;
            }

            return Regenerate(mpp, user);
        }

        public static bool Regenerate(IEvaluationMPP mpp, EvaluationBE evaluation)
        {
            if (mpp == null || evaluation == null)
            {
                return false;
            }

            evaluation.Dvh = Compute(evaluation.GetDvhValues());

            return mpp.UpdateDvh(evaluation.Id, evaluation.Dvh);
        }

        public static bool RegenerateEvaluation(IEvaluationMPP mpp, int evaluationId)
        {
            if (mpp == null)
            {
                return false;
            }

            EvaluationBE evaluation = mpp.FindById(evaluationId);

            if (evaluation == null)
            {
                return true;
            }

            return Regenerate(mpp, evaluation);
        }

        public static bool Regenerate(IEvaluationMPP mpp, GradeBE grade)
        {
            if (mpp == null || grade == null)
            {
                return false;
            }

            grade.Dvh = Compute(grade.GetDvhValues());

            return mpp.UpdateGradeDvh(grade.Id, grade.Dvh);
        }

        public static bool RegenerateGrade(IEvaluationMPP mpp, int gradeId)
        {
            if (mpp == null)
            {
                return false;
            }

            GradeBE grade = mpp.FindGradeById(gradeId);

            if (grade == null)
            {
                return true;
            }

            return Regenerate(mpp, grade);
        }
        #endregion
    }
}
