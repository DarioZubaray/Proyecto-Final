using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Services;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class TicketIntegrationTests
    {
        #region Propiedades
        private TicketMPP _ticketMPP;
        private TicketBLL _ticketBLL;
        private string _connectionString;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.EnsureDatabaseExists();
            TestDatabaseHelper.CreateSchema();
            TestDatabaseHelper.SeedTestData();
            _connectionString = TestDatabaseHelper.TestConnectionString;
            _ticketMPP = new TicketMPP(_connectionString);
            _ticketBLL = new TicketBLL(_ticketMPP);
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDatabaseHelper.CleanDatabase();
        }
        #endregion

        #region Tests
        [TestMethod]
        public void Create_NewTicket_PersistsAndFindsByUser()
        {
            var ticket = new TicketBE { UserId = 1, Subject = "Problema con el aula", Description = "El proyector no enciende." };

            Assert.IsTrue(_ticketBLL.Create(ticket));
            Assert.IsTrue(ticket.Id > 0);

            List<TicketBE> own = _ticketBLL.FindByUser(1);

            Assert.AreEqual(1, own.Count);
            Assert.AreEqual("Problema con el aula", own[0].Subject);
            Assert.AreEqual("testuser", own[0].UserName);
            Assert.IsFalse(own[0].IsClosed);
            Assert.IsNull(own[0].ClosedAt);
        }

        [TestMethod]
        public void Create_TicketFromOtherUser_NotInFindByUser()
        {
            var ticket = new TicketBE { UserId = 2, Subject = "Otro usuario", Description = "Descripcion." };

            Assert.IsTrue(_ticketBLL.Create(ticket));

            Assert.AreEqual(0, _ticketBLL.FindByUser(1).Count);
            Assert.AreEqual(1, _ticketBLL.FindByUser(2).Count);
            Assert.AreEqual(1, _ticketBLL.FindAll().Count);
        }

        [TestMethod]
        public void Close_OpenTicket_MarksClosedWithAdmin()
        {
            var ticket = new TicketBE { UserId = 1, Subject = "A cerrar", Description = "detalle" };
            Assert.IsTrue(_ticketBLL.Create(ticket));

            Assert.IsTrue(_ticketBLL.Close(ticket.Id, 2));

            TicketBE persisted = _ticketBLL.FindAll().First(t => t.Id == ticket.Id);

            Assert.IsTrue(persisted.IsClosed);
            Assert.IsNotNull(persisted.ClosedAt);
            Assert.AreEqual(2, persisted.ClosedBy);
            Assert.AreEqual("seconduser", persisted.ClosedByName);
        }

        [TestMethod]
        public void Close_AlreadyClosed_ReturnsFalse()
        {
            var ticket = new TicketBE { UserId = 1, Subject = "Doble cierre", Description = "detalle" };
            Assert.IsTrue(_ticketBLL.Create(ticket));
            Assert.IsTrue(_ticketBLL.Close(ticket.Id, 2));

            Assert.IsFalse(_ticketBLL.Close(ticket.Id, 2));
        }
        #endregion
    }
}
