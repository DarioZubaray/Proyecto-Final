using System;
using System.Collections.Generic;
using BE.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class ClassroomMPPTests
    {
        private ClassroomMPP _classroomMPP;
        private string _connectionString;

        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.EnsureDatabaseExists();
            TestDatabaseHelper.CreateSchema();
            TestDatabaseHelper.SeedTestData();
            _connectionString = TestDatabaseHelper.TestConnectionString;
            _classroomMPP = new ClassroomMPP(_connectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDatabaseHelper.CleanDatabase();
        }

        [TestMethod]
        public void Save_NewClassroom_ReturnsTrue()
        {
            var classroom = new ClassroomBE
            {
                Name = "Aula Test 101",
                Capacity = 30,
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };

            bool result = _classroomMPP.Save(classroom);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FindAll_ReturnsClassrooms()
        {
            _classroomMPP.Save(new ClassroomBE
            {
                Name = "Aula 201",
                Capacity = 25,
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            });

            List<ClassroomBE> result = _classroomMPP.FindAll();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 1);
        }

        [TestMethod]
        public void FindById_ExistingClassroom_ReturnsClassroom()
        {
            var classroom = new ClassroomBE
            {
                Name = "Aula 301",
                Capacity = 20,
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            _classroomMPP.Save(classroom);

            List<ClassroomBE> all = _classroomMPP.FindAll();
            ClassroomBE found = _classroomMPP.FindById(all[0].Id);

            Assert.IsNotNull(found);
            Assert.AreEqual("Aula 301", found.Name);
        }

        [TestMethod]
        public void FindById_NonExisting_ReturnsNull()
        {
            ClassroomBE result = _classroomMPP.FindById(99999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void FindByName_FiltersPartialMatch()
        {
            _classroomMPP.Save(new ClassroomBE
            {
                Name = "Laboratorio A",
                Capacity = 15,
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            });
            _classroomMPP.Save(new ClassroomBE
            {
                Name = "Laboratorio B",
                Capacity = 20,
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            });

            List<ClassroomBE> result = _classroomMPP.FindByName("Laboratorio");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 2);
        }

        [TestMethod]
        public void Delete_DeactivatesClassroom()
        {
            var classroom = new ClassroomBE
            {
                Name = "Aula Delete Test",
                Capacity = 10,
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            _classroomMPP.Save(classroom);

            List<ClassroomBE> all = _classroomMPP.FindAll();
            ClassroomBE toDelete = all.Find(c => c.Name == "Aula Delete Test");
            Assert.IsNotNull(toDelete);

            bool result = _classroomMPP.Delete(toDelete);

            Assert.IsTrue(result);
            ClassroomBE deleted = _classroomMPP.FindById(toDelete.Id);
            Assert.IsNull(deleted);
        }

        [TestMethod]
        public void Count_ReturnsNumberOfActiveClassrooms()
        {
            _classroomMPP.Save(new ClassroomBE
            {
                Name = "Aula Count Test",
                Capacity = 10,
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            });

            int count = _classroomMPP.Count();

            Assert.IsTrue(count >= 1);
        }
    }
}
