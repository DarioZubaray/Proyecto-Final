using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Services;
using Moq;
using MPP.Interfaces;

namespace BLL.Tests
{
    [TestClass]
    public class StudentBLLTests
    {
        #region Propiedades
        private Mock<IStudentMPP> _mockStudentMPP;
        private StudentBLL _studentBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockStudentMPP = new Mock<IStudentMPP>();
            _studentBLL = new StudentBLL(_mockStudentMPP.Object);
        }
        #endregion

        #region Tests - Save
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Save_NullStudent_ThrowsArgumentNullException()
        {
            _studentBLL.Save(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Save_MissingFirstName_ThrowsException()
        {
            _studentBLL.Save(new StudentBE { FirstName = " ", LastName = "Perez", Document = "DNI-1" });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Save_MissingLastName_ThrowsException()
        {
            _studentBLL.Save(new StudentBE { FirstName = "Juan", LastName = "  ", Document = "DNI-1" });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Save_MissingDocument_ThrowsException()
        {
            _studentBLL.Save(new StudentBE { FirstName = "Juan", LastName = "Perez", Document = null });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Save_DuplicateDocument_ThrowsException()
        {
            _mockStudentMPP.Setup(m => m.FindByDocument("DNI-10000001"))
                .Returns(new StudentBE { Id = 99, Document = "DNI-10000001" });

            _studentBLL.Save(new StudentBE
            {
                Id = 0,
                FirstName = "Juan",
                LastName = "Perez",
                Document = "DNI-10000001"
            });
        }

        [TestMethod]
        public void Save_NewStudent_TrimsAndInserts()
        {
            _mockStudentMPP.Setup(m => m.FindByDocument("DNI-NEW")).Returns((StudentBE)null);
            _mockStudentMPP.Setup(m => m.Insert(It.IsAny<StudentBE>())).Returns(true);

            bool result = _studentBLL.Save(new StudentBE
            {
                Id = 0,
                FirstName = "  Ana  ",
                LastName = " Gomez ",
                Document = " DNI-NEW "
            });

            Assert.IsTrue(result);
            _mockStudentMPP.Verify(m => m.Insert(It.Is<StudentBE>(s =>
                s.FirstName == "Ana" &&
                s.LastName == "Gomez" &&
                s.Document == "DNI-NEW" &&
                s.IsActive)), Times.Once);
            _mockStudentMPP.Verify(m => m.Update(It.IsAny<StudentBE>()), Times.Never);
        }

        [TestMethod]
        public void Save_ExistingStudent_Updates()
        {
            _mockStudentMPP.Setup(m => m.FindByDocument("DNI-1")).Returns((StudentBE)null);
            _mockStudentMPP.Setup(m => m.Update(It.IsAny<StudentBE>())).Returns(true);

            bool result = _studentBLL.Save(new StudentBE
            {
                Id = 10,
                FirstName = "Juan",
                LastName = "Perez",
                Document = "DNI-1"
            });

            Assert.IsTrue(result);
            _mockStudentMPP.Verify(m => m.Update(It.IsAny<StudentBE>()), Times.Once);
            _mockStudentMPP.Verify(m => m.Insert(It.IsAny<StudentBE>()), Times.Never);
        }

        [TestMethod]
        public void Save_SameStudentDocument_UpdatesWithoutDuplicateError()
        {
            _mockStudentMPP.Setup(m => m.FindByDocument("DNI-1"))
                .Returns(new StudentBE { Id = 10, Document = "DNI-1" });
            _mockStudentMPP.Setup(m => m.Update(It.IsAny<StudentBE>())).Returns(true);

            bool result = _studentBLL.Save(new StudentBE
            {
                Id = 10,
                FirstName = "Juan",
                LastName = "Perez",
                Document = "DNI-1"
            });

            Assert.IsTrue(result);
            _mockStudentMPP.Verify(m => m.Update(It.IsAny<StudentBE>()), Times.Once);
        }
        #endregion

        #region Tests - Delete
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Delete_NullStudent_ThrowsArgumentNullException()
        {
            _studentBLL.Delete(null);
        }

        [TestMethod]
        public void Delete_DelegatesToMPP()
        {
            _mockStudentMPP.Setup(m => m.Delete(5)).Returns(true);

            bool result = _studentBLL.Delete(new StudentBE { Id = 5 });

            Assert.IsTrue(result);
            _mockStudentMPP.Verify(m => m.Delete(5), Times.Once);
        }
        #endregion

        #region Tests - Find
        [TestMethod]
        public void FindAll_DelegatesToMPP()
        {
            var expected = new List<StudentBE>
            {
                new StudentBE { Id = 1 },
                new StudentBE { Id = 2 }
            };
            _mockStudentMPP.Setup(m => m.FindAll()).Returns(expected);

            List<StudentBE> result = _studentBLL.FindAll();

            CollectionAssert.AreEqual(expected, result);
            _mockStudentMPP.Verify(m => m.FindAll(), Times.Once);
        }

        [TestMethod]
        public void FindAllActive_DelegatesToMPP()
        {
            var expected = new List<StudentBE> { new StudentBE { Id = 1, IsActive = true } };
            _mockStudentMPP.Setup(m => m.FindAllActive()).Returns(expected);

            List<StudentBE> result = _studentBLL.FindAllActive();

            CollectionAssert.AreEqual(expected, result);
            _mockStudentMPP.Verify(m => m.FindAllActive(), Times.Once);
        }

        [TestMethod]
        public void FindById_DelegatesToMPP()
        {
            var expected = new StudentBE { Id = 7, FirstName = "Juan" };
            _mockStudentMPP.Setup(m => m.FindById(7)).Returns(expected);

            StudentBE result = _studentBLL.FindById(7);

            Assert.AreEqual(expected, result);
            _mockStudentMPP.Verify(m => m.FindById(7), Times.Once);
        }

        [TestMethod]
        public void FindByName_DelegatesToMPP()
        {
            var expected = new List<StudentBE> { new StudentBE { Id = 1, LastName = "Perez" } };
            _mockStudentMPP.Setup(m => m.FindByName("Per")).Returns(expected);

            List<StudentBE> result = _studentBLL.FindByName("Per");

            CollectionAssert.AreEqual(expected, result);
            _mockStudentMPP.Verify(m => m.FindByName("Per"), Times.Once);
        }
        #endregion
    }
}
