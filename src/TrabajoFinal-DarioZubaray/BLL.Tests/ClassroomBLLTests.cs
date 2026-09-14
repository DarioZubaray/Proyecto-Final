using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Services;
using Moq;
using MPP.Interfaces;

namespace BLL.Tests
{
    [TestClass]
    public class ClassroomBLLTests
    {
        #region Propiedades
        private Mock<IClassroomMPP> _mockClassroomMPP;
        private ClassroomBLL _classroomBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockClassroomMPP = new Mock<IClassroomMPP>();
            _classroomBLL = new ClassroomBLL(_mockClassroomMPP.Object);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void Save_DelegatesToMPP()
        {
            var classroom = new ClassroomBE { Id = 0, Name = "Aula 101", Capacity = 30 };
            _mockClassroomMPP.Setup(m => m.Save(classroom)).Returns(true);

            bool result = _classroomBLL.Save(classroom);

            Assert.IsTrue(result);
            _mockClassroomMPP.Verify(m => m.Save(classroom), Times.Once);
        }

        [TestMethod]
        public void Save_ExistingClassroom_DelegatesToMPP()
        {
            var classroom = new ClassroomBE { Id = 5, Name = "Aula 102", Capacity = 25 };
            _mockClassroomMPP.Setup(m => m.Save(classroom)).Returns(true);

            bool result = _classroomBLL.Save(classroom);

            Assert.IsTrue(result);
            _mockClassroomMPP.Verify(m => m.Save(classroom), Times.Once);
        }

        [TestMethod]
        public void Delete_DelegatesToMPP()
        {
            var classroom = new ClassroomBE { Id = 3 };
            _mockClassroomMPP.Setup(m => m.Delete(classroom)).Returns(true);

            bool result = _classroomBLL.Delete(classroom);

            Assert.IsTrue(result);
            _mockClassroomMPP.Verify(m => m.Delete(classroom), Times.Once);
        }

        [TestMethod]
        public void FindById_DelegatesToMPP()
        {
            var expected = new ClassroomBE { Id = 1, Name = "Aula 101", Capacity = 30 };
            _mockClassroomMPP.Setup(m => m.FindById(1)).Returns(expected);

            ClassroomBE result = _classroomBLL.FindById(1);

            Assert.AreEqual(expected, result);
            _mockClassroomMPP.Verify(m => m.FindById(1), Times.Once);
        }

        [TestMethod]
        public void FindById_NonExisting_ReturnsNull()
        {
            _mockClassroomMPP.Setup(m => m.FindById(999)).Returns((ClassroomBE)null);

            ClassroomBE result = _classroomBLL.FindById(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void FindAll_DelegatesToMPP()
        {
            var expected = new List<ClassroomBE>
            {
                new ClassroomBE { Id = 1, Name = "Aula 101" },
                new ClassroomBE { Id = 2, Name = "Aula 102" }
            };
            _mockClassroomMPP.Setup(m => m.FindAll()).Returns(expected);

            List<ClassroomBE> result = _classroomBLL.FindAll();

            CollectionAssert.AreEqual(expected, result);
            _mockClassroomMPP.Verify(m => m.FindAll(), Times.Once);
        }

        [TestMethod]
        public void FindByName_DelegatesToMPP()
        {
            var expected = new List<ClassroomBE>
            {
                new ClassroomBE { Id = 1, Name = "Aula 101" }
            };
            _mockClassroomMPP.Setup(m => m.FindByName("101")).Returns(expected);

            List<ClassroomBE> result = _classroomBLL.FindByName("101");

            CollectionAssert.AreEqual(expected, result);
            _mockClassroomMPP.Verify(m => m.FindByName("101"), Times.Once);
        }

        [TestMethod]
        public void Count_DelegatesToMPP()
        {
            _mockClassroomMPP.Setup(m => m.Count()).Returns(5);

            int result = _classroomBLL.Count();

            Assert.AreEqual(5, result);
            _mockClassroomMPP.Verify(m => m.Count(), Times.Once);
        }

        [TestMethod]
        public void Count_NoClassrooms_ReturnsZero()
        {
            _mockClassroomMPP.Setup(m => m.Count()).Returns(0);

            int result = _classroomBLL.Count();

            Assert.AreEqual(0, result);
        }
        #endregion
    }
}
