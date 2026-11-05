using System;
using System.Collections.Generic;
using BE.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class StudentMPPTests
    {
        private StudentMPP _studentMPP;

        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.EnsureDatabaseExists();
            TestDatabaseHelper.CreateSchema();
            TestDatabaseHelper.SeedTestData();
            _studentMPP = new StudentMPP(TestDatabaseHelper.TestConnectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDatabaseHelper.CleanDatabase();
        }

        private StudentBE CreateStudent(string document, string firstName = "Test", string lastName = "Student", bool isActive = true)
        {
            var student = new StudentBE
            {
                FirstName = firstName,
                LastName = lastName,
                Document = document,
                Email = null,
                Phone = null,
                IsActive = isActive,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            _studentMPP.Insert(student);
            return _studentMPP.FindByDocument(document);
        }

        [TestMethod]
        public void Insert_NewStudent_ReturnsTrue()
        {
            var student = new StudentBE
            {
                FirstName = "Carlos",
                LastName = "Lopez",
                Document = "DNI-STU-001",
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };

            bool result = _studentMPP.Insert(student);

            Assert.IsTrue(result);
            StudentBE found = _studentMPP.FindByDocument("DNI-STU-001");
            Assert.IsNotNull(found);
            Assert.AreEqual("Carlos", found.FirstName);
            Assert.AreEqual("Lopez", found.LastName);
        }

        [TestMethod]
        public void FindAll_ReturnsStudents()
        {
            List<StudentBE> result = _studentMPP.FindAll();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 2);
        }

        [TestMethod]
        public void FindAllActive_ExcludesInactiveStudents()
        {
            CreateStudent("DNI-STU-INACT", isActive: false);

            List<StudentBE> all = _studentMPP.FindAll();
            List<StudentBE> active = _studentMPP.FindAllActive();

            Assert.IsTrue(all.Count > active.Count);
            Assert.IsFalse(active.Exists(s => s.Document == "DNI-STU-INACT"));
            Assert.IsTrue(all.Exists(s => s.Document == "DNI-STU-INACT"));
        }

        [TestMethod]
        public void FindById_ExistingStudent_ReturnsStudent()
        {
            StudentBE created = CreateStudent("DNI-STU-002", "Maria", "Gomez");

            StudentBE found = _studentMPP.FindById(created.Id);

            Assert.IsNotNull(found);
            Assert.AreEqual("Maria", found.FirstName);
            Assert.AreEqual("Gomez", found.LastName);
        }

        [TestMethod]
        public void FindById_NonExisting_ReturnsNull()
        {
            StudentBE result = _studentMPP.FindById(99999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void FindByDocument_ExistingStudent_ReturnsStudent()
        {
            CreateStudent("DNI-STU-003", "Pedro", "Ramirez");

            StudentBE found = _studentMPP.FindByDocument("DNI-STU-003");

            Assert.IsNotNull(found);
            Assert.AreEqual("Pedro", found.FirstName);
        }

        [TestMethod]
        public void FindByDocument_NonExisting_ReturnsNull()
        {
            StudentBE result = _studentMPP.FindByDocument("DNI-NO-EXISTE");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void FindByName_ReturnsMatches()
        {
            CreateStudent("DNI-STU-004", "Fernanda", "Diaz");

            List<StudentBE> byFirstName = _studentMPP.FindByName("Fernanda");
            Assert.IsTrue(byFirstName.Exists(s => s.Document == "DNI-STU-004"));

            List<StudentBE> bySeedLastName = _studentMPP.FindByName("Pérez");
            Assert.IsTrue(bySeedLastName.Exists(s => s.LastName == "Pérez"));
        }

        [TestMethod]
        public void Update_ModifiesStudent()
        {
            StudentBE created = CreateStudent("DNI-STU-005", "Juan", "Sanchez");
            created.LastName = "Modified";
            created.LastUpdate = DateTime.Now;

            bool result = _studentMPP.Update(created);

            Assert.IsTrue(result);
            StudentBE found = _studentMPP.FindByDocument("DNI-STU-005");
            Assert.AreEqual("Modified", found.LastName);
        }

        [TestMethod]
        public void Delete_DeactivatesStudent()
        {
            StudentBE created = CreateStudent("DNI-STU-006");

            bool result = _studentMPP.Delete(created.Id);

            Assert.IsTrue(result);
            StudentBE found = _studentMPP.FindById(created.Id);
            Assert.IsNotNull(found);
            Assert.IsFalse(found.IsActive);
        }
    }
}
