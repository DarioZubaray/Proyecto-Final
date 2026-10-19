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
    }
}
