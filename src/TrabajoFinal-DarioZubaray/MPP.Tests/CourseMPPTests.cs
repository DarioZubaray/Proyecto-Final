using System;
using System.Collections.Generic;
using BE.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class CourseMPPTests
    {
        private ClassroomMPP _classroomMPP;
        private CourseMPP _courseMPP;
        private string _connectionString;

        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.EnsureDatabaseExists();
            TestDatabaseHelper.CreateSchema();
            TestDatabaseHelper.SeedTestData();
            _connectionString = TestDatabaseHelper.TestConnectionString;
            _classroomMPP = new ClassroomMPP(_connectionString);
            _courseMPP = new CourseMPP(_connectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDatabaseHelper.CleanDatabase();
        }

        private int CreateTestClassroom()
        {
            var classroom = new ClassroomBE
            {
                Name = "Aula para Cursos",
                Capacity = 30,
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            _classroomMPP.Save(classroom);
            List<ClassroomBE> all = _classroomMPP.FindAll();
            return all[all.Count - 1].Id;
        }

        [TestMethod]
        public void Save_NewCourse_ReturnsTrue()
        {
            int classroomId = CreateTestClassroom();
            var course = new CourseBE
            {
                Name = "Ingles Basico Test",
                Description = "Curso de prueba",
                StartDate = new DateTime(2026, 8, 1),
                EndDate = new DateTime(2026, 12, 31),
                ClassroomId = classroomId,
                DayOfWeek = 1,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(10, 0, 0),
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };

            bool result = _courseMPP.Save(course);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FindAll_ReturnsCourses()
        {
            int classroomId = CreateTestClassroom();
            _courseMPP.Save(new CourseBE
            {
                Name = "Curso FindAll",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(6),
                ClassroomId = classroomId,
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            });

            List<CourseBE> result = _courseMPP.FindAll();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 1);
        }

        [TestMethod]
        public void FindById_ExistingCourse_ReturnsCourse()
        {
            int classroomId = CreateTestClassroom();
            var course = new CourseBE
            {
                Name = "Curso FindById",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(6),
                ClassroomId = classroomId,
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            _courseMPP.Save(course);

            List<CourseBE> all = _courseMPP.FindAll();
            CourseBE found = _courseMPP.FindById(all[0].Id);

            Assert.IsNotNull(found);
            Assert.AreEqual("Curso FindById", found.Name);
        }

        [TestMethod]
        public void FindById_NonExisting_ReturnsNull()
        {
            CourseBE result = _courseMPP.FindById(99999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Delete_DeactivatesCourse()
        {
            int classroomId = CreateTestClassroom();
            var course = new CourseBE
            {
                Name = "Curso Delete Test",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(6),
                ClassroomId = classroomId,
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            _courseMPP.Save(course);

            List<CourseBE> all = _courseMPP.FindAll();
            CourseBE toDelete = all.Find(c => c.Name == "Curso Delete Test");
            Assert.IsNotNull(toDelete);

            bool result = _courseMPP.Delete(toDelete);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExistsClassroomOverlap_NoOverlap_ReturnsFalse()
        {
            int classroomId = CreateTestClassroom();

            bool result = _courseMPP.ExistsClassroomOverlap(
                classroomId,
                new DateTime(2026, 8, 1),
                new DateTime(2026, 12, 31),
                0,
                1,
                new TimeSpan(8, 0, 0),
                new TimeSpan(10, 0, 0));

            Assert.IsFalse(result);
        }
    }
}
