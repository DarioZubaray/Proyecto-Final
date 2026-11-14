using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Helpers;
using Moq;
using MPP.Interfaces;

namespace BLL.Tests
{
    [TestClass]
    public class DVHHelperBLLTests
    {
        #region Tests: Compute
        [TestMethod]
        public void Compute_KnownSha256Vector_ReturnsExpectedHash()
        {
            string hash = DVHHelperBLL.Compute("abc");

            Assert.AreEqual(
                "13e228567e8249fce53337f25d7970de3bd68ab2653424c7b8f9fd05e33caedf",
                hash);
        }

        [TestMethod]
        public void Compute_JoinsValuesWithComma_NoSpaces()
        {
            Assert.AreEqual(
                DVHHelperBLL.Compute("a", "b", "c"),
                DVHHelperBLL.Compute(new[] { "a", "b", "c" }));

            Assert.AreNotEqual(
                DVHHelperBLL.Compute("abc"),
                DVHHelperBLL.Compute("a", "b", "c"));
        }

        [TestMethod]
        public void Compute_IsDeterministic()
        {
            string first = DVHHelperBLL.Compute("x", "1", "");
            string second = DVHHelperBLL.Compute("x", "1", "");

            Assert.AreEqual(first, second);
        }

        [TestMethod]
        public void Compute_EmptyValues_ReturnsHashOfEmptyPayload()
        {
            string hash = DVHHelperBLL.Compute(new object[0]);

            Assert.AreEqual(
                "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
                hash);
        }

        [TestMethod]
        public void Compute_NonAsciiPayload_UsesUtf16NotUtf8()
        {
            string payload = "ñandú";
            string expected;
            string utf8Hash;

            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                expected = ToHex(sha.ComputeHash(Encoding.Unicode.GetBytes(payload)));
                utf8Hash = ToHex(sha.ComputeHash(Encoding.UTF8.GetBytes(payload)));
            }

            Assert.AreEqual(expected, DVHHelperBLL.Compute(payload));
            Assert.AreNotEqual(utf8Hash, DVHHelperBLL.Compute(payload));
        }

        private static string ToHex(byte[] hash)
        {
            var builder = new System.Text.StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
        #endregion

        #region Tests: Normalize / FormatDate
        [TestMethod]
        public void Normalize_NullOrDbNull_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, DVHHelperBLL.Normalize(null));
            Assert.AreEqual(string.Empty, DVHHelperBLL.Normalize(DBNull.Value));
        }

        [TestMethod]
        public void Normalize_Bool_ReturnsOneOrZero()
        {
            Assert.AreEqual("1", DVHHelperBLL.Normalize(true));
            Assert.AreEqual("0", DVHHelperBLL.Normalize(false));
        }

        [TestMethod]
        public void Normalize_DateTime_UsesStyle126Format()
        {
            var date = new DateTime(2026, 10, 9, 13, 45, 59, 123);

            Assert.AreEqual("2026-10-09T13:45:59.123", DVHHelperBLL.Normalize(date));
            Assert.AreEqual("2026-10-09T13:45:59.123", DVHHelperBLL.FormatDate(date));
        }

        [TestMethod]
        public void FormatDate_ZeroMilliseconds_OmitsFractionLikeSql()
        {
            var date = new DateTime(2026, 10, 9, 13, 46, 1, 0);

            Assert.AreEqual("2026-10-09T13:46:01", DVHHelperBLL.FormatDate(date));
        }

        [TestMethod]
        public void Normalize_Numbers_AreCultureInvariant()
        {
            Assert.AreEqual("42", DVHHelperBLL.Normalize(42));
            Assert.AreEqual("-7", DVHHelperBLL.Normalize(-7));
        }
        #endregion

        #region Tests: UserBE.GetDvhValues + DVH
        [TestMethod]
        public void Dvh_SameRowTwice_ReturnsSameHash()
        {
            UserBE user = CreateSampleUser();

            string first = DVHHelperBLL.Compute(user.GetDvhValues());
            string second = DVHHelperBLL.Compute(user.GetDvhValues());

            Assert.AreEqual(first, second);
            Assert.AreEqual(64, first.Length);
        }

        [TestMethod]
        public void Dvh_AnyFieldChange_ChangesHash()
        {
            UserBE user = CreateSampleUser();
            string baseline = DVHHelperBLL.Compute(user.GetDvhValues());

            user.UserName = "otro_usuario";
            Assert.AreNotEqual(baseline, DVHHelperBLL.Compute(user.GetDvhValues()));

            UserBE other = CreateSampleUser();
            other.RoleId = 0;
            other.Dvh = null;
            Assert.AreNotEqual(
                DVHHelperBLL.Compute(CreateSampleUser().GetDvhValues()),
                DVHHelperBLL.Compute(other.GetDvhValues()));
        }

        [TestMethod]
        public void Dvh_NullRoleId_SerializesAsEmpty()
        {
            UserBE withRole = CreateSampleUser();
            UserBE withoutRole = CreateSampleUser();
            withoutRole.RoleId = 0;

            Assert.AreNotEqual(
                DVHHelperBLL.Compute(withRole.GetDvhValues()),
                DVHHelperBLL.Compute(withoutRole.GetDvhValues()));
            Assert.AreEqual(string.Empty, DVHHelperBLL.Normalize(withoutRole.GetDvhValues()[7]));
        }

        [TestMethod]
        public void GetDvhValues_ExcludesDvhColumn()
        {
            UserBE user = CreateSampleUser();
            user.Dvh = "valor-que-no-debe-entrar-al-hash";

            object[] values = user.GetDvhValues();

            Assert.AreEqual(10, values.Length);

            foreach (object value in values)
            {
                Assert.AreNotEqual(user.Dvh, DVHHelperBLL.Normalize(value));
            }

            Assert.AreNotEqual(user.Dvh, DVHHelperBLL.Compute(values));
        }

        [TestMethod]
        public void Regenerate_SavedEntity_PersistsComputedHash()
        {
            UserBE user = CreateSampleUser();
            var mpp = new Mock<IUserMPP>();
            mpp.Setup(m => m.UpdateDvh(user.Id, It.IsAny<string>())).Returns(true);

            bool result = DVHHelperBLL.Regenerate(mpp.Object, user);

            Assert.IsTrue(result);
            Assert.AreEqual(DVHHelperBLL.Compute(user.GetDvhValues()), user.Dvh);
            mpp.Verify(m => m.UpdateDvh(user.Id, user.Dvh), Times.Once);
        }

        private UserBE CreateSampleUser()
        {
            return new UserBE
            {
                Id = 7,
                UserName = "usuario_prueba",
                PasswordHash = "$2a$11$hash",
                IsActive = true,
                RetriesCount = 0,
                LastUpdate = new DateTime(2026, 10, 9, 12, 0, 0),
                CreatedAt = new DateTime(2026, 1, 1, 8, 30, 0),
                RoleId = 4,
                Language = "es",
                Theme = "System"
            };
        }
        #endregion

        #region Tests: EvaluationBE / GradeBE + Regenerate
        [TestMethod]
        public void Evaluation_GetDvhValues_ExcludesDvhColumn()
        {
            EvaluationBE evaluation = CreateSampleEvaluation();
            evaluation.Dvh = "valor-que-no-debe-entrar-al-hash";

            object[] values = evaluation.GetDvhValues();

            Assert.AreEqual(7, values.Length);

            foreach (object value in values)
            {
                Assert.AreNotEqual(evaluation.Dvh, DVHHelperBLL.Normalize(value));
            }

            Assert.AreNotEqual(evaluation.Dvh, DVHHelperBLL.Compute(values));
        }

        [TestMethod]
        public void Grade_GetDvhValues_ExcludesDvhColumn()
        {
            GradeBE grade = CreateSampleGrade();
            grade.Dvh = "valor-que-no-debe-entrar-al-hash";

            object[] values = grade.GetDvhValues();

            Assert.AreEqual(4, values.Length);

            foreach (object value in values)
            {
                Assert.AreNotEqual(grade.Dvh, DVHHelperBLL.Normalize(value));
            }

            Assert.AreNotEqual(grade.Dvh, DVHHelperBLL.Compute(values));
        }

        [TestMethod]
        public void Grade_AnyFieldChange_ChangesHash()
        {
            GradeBE grade = CreateSampleGrade();
            string baseline = DVHHelperBLL.Compute(grade.GetDvhValues());

            grade.Grade = 9.9m;
            Assert.AreNotEqual(baseline, DVHHelperBLL.Compute(grade.GetDvhValues()));

            GradeBE other = CreateSampleGrade();
            other.StudentId = 99;
            Assert.AreNotEqual(baseline, DVHHelperBLL.Compute(other.GetDvhValues()));
        }

        [TestMethod]
        public void Normalize_Decimal_UsesInvariantWithScale()
        {
            Assert.AreEqual("7.30", DVHHelperBLL.Normalize(7.30m));
            Assert.AreEqual("10.00", DVHHelperBLL.Normalize(10.00m));
        }

        [TestMethod]
        public void Regenerate_Evaluation_PersistsComputedHash()
        {
            EvaluationBE evaluation = CreateSampleEvaluation();
            var mpp = new Mock<IEvaluationMPP>();
            mpp.Setup(m => m.UpdateDvh(evaluation.Id, It.IsAny<string>())).Returns(true);

            bool result = DVHHelperBLL.Regenerate(mpp.Object, evaluation);

            Assert.IsTrue(result);
            Assert.AreEqual(DVHHelperBLL.Compute(evaluation.GetDvhValues()), evaluation.Dvh);
            mpp.Verify(m => m.UpdateDvh(evaluation.Id, evaluation.Dvh), Times.Once);
        }

        [TestMethod]
        public void Regenerate_Grade_PersistsComputedHash()
        {
            GradeBE grade = CreateSampleGrade();
            var mpp = new Mock<IEvaluationMPP>();
            mpp.Setup(m => m.UpdateGradeDvh(grade.Id, It.IsAny<string>())).Returns(true);

            bool result = DVHHelperBLL.Regenerate(mpp.Object, grade);

            Assert.IsTrue(result);
            Assert.AreEqual(DVHHelperBLL.Compute(grade.GetDvhValues()), grade.Dvh);
            mpp.Verify(m => m.UpdateGradeDvh(grade.Id, grade.Dvh), Times.Once);
        }

        [TestMethod]
        public void RegenerateEvaluation_RereadsRowBeforeComputing()
        {
            EvaluationBE stored = CreateSampleEvaluation();
            stored.Dvh = "hash-previo";

            var mpp = new Mock<IEvaluationMPP>();
            mpp.Setup(m => m.FindById(stored.Id)).Returns(stored);
            mpp.Setup(m => m.UpdateDvh(stored.Id, It.IsAny<string>())).Returns(true);

            bool result = DVHHelperBLL.RegenerateEvaluation(mpp.Object, stored.Id);

            Assert.IsTrue(result);
            mpp.Verify(m => m.FindById(stored.Id), Times.Once);
            mpp.Verify(m => m.UpdateDvh(stored.Id, DVHHelperBLL.Compute(stored.GetDvhValues())), Times.Once);
        }

        [TestMethod]
        public void RegenerateGrade_RereadsRowBeforeComputing()
        {
            GradeBE stored = CreateSampleGrade();
            stored.Dvh = "hash-previo";

            var mpp = new Mock<IEvaluationMPP>();
            mpp.Setup(m => m.FindGradeById(stored.Id)).Returns(stored);
            mpp.Setup(m => m.UpdateGradeDvh(stored.Id, It.IsAny<string>())).Returns(true);

            bool result = DVHHelperBLL.RegenerateGrade(mpp.Object, stored.Id);

            Assert.IsTrue(result);
            mpp.Verify(m => m.FindGradeById(stored.Id), Times.Once);
            mpp.Verify(m => m.UpdateGradeDvh(stored.Id, DVHHelperBLL.Compute(stored.GetDvhValues())), Times.Once);
        }

        [TestMethod]
        public void RegenerateEvaluation_RowNotFound_ReturnsTrueWithoutWriting()
        {
            var mpp = new Mock<IEvaluationMPP>();
            mpp.Setup(m => m.FindById(99)).Returns((EvaluationBE)null);

            Assert.IsTrue(DVHHelperBLL.RegenerateEvaluation(mpp.Object, 99));
            mpp.Verify(m => m.UpdateDvh(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        private EvaluationBE CreateSampleEvaluation()
        {
            return new EvaluationBE
            {
                Id = 5,
                CourseId = 2,
                Name = "Parcial 1",
                Date = new DateTime(2026, 9, 15),
                CreatedBy = 3,
                CreatedAt = new DateTime(2026, 9, 1, 10, 30, 0),
                LastUpdate = new DateTime(2026, 9, 2, 11, 45, 30, 500)
            };
        }

        private GradeBE CreateSampleGrade()
        {
            return new GradeBE
            {
                Id = 11,
                EvaluationId = 5,
                StudentId = 4,
                Grade = 7.30m
            };
        }
        #endregion
    }
}
