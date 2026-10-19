using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using BE.Entities;
using BLL.Services;
using MPP.Interfaces;

namespace BLL.Tests
{
    [TestClass]
    public class TicketBLLTests
    {
        private Mock<ITicketMPP> _mockTicketMPP;
        private TicketBLL _ticketBLL;

        [TestInitialize]
        public void Setup()
        {
            _mockTicketMPP = new Mock<ITicketMPP>();
            _ticketBLL = new TicketBLL(_mockTicketMPP.Object);
        }

        [TestMethod]
        public void Create_ValidTicket_ReturnsTrue()
        {
            var ticket = new TicketBE { UserId = 1, Subject = "  Asunto  ", Description = "  Descripción  " };
            _mockTicketMPP.Setup(m => m.Insert(It.IsAny<TicketBE>())).Returns(true);

            bool result = _ticketBLL.Create(ticket);

            Assert.IsTrue(result);
            Assert.AreEqual("Asunto", ticket.Subject);
            Assert.AreEqual("Descripción", ticket.Description);
            Assert.IsFalse(ticket.IsClosed);
            Assert.IsNotNull(ticket.CreatedAt);
            Assert.IsNull(ticket.ClosedAt);
            Assert.IsNull(ticket.ClosedBy);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Create_NullTicket_Throws()
        {
            _ticketBLL.Create(null);
        }

        [TestMethod]
        public void Create_MissingSubject_Throws()
        {
            var ticket = new TicketBE { UserId = 1, Subject = "   ", Description = "desc" };

            Assert.ThrowsException<InvalidOperationException>(() => _ticketBLL.Create(ticket));
            _mockTicketMPP.Verify(m => m.Insert(It.IsAny<TicketBE>()), Times.Never);
        }

        [TestMethod]
        public void Create_MissingDescription_Throws()
        {
            var ticket = new TicketBE { UserId = 1, Subject = "asunto", Description = null };

            Assert.ThrowsException<InvalidOperationException>(() => _ticketBLL.Create(ticket));
            _mockTicketMPP.Verify(m => m.Insert(It.IsAny<TicketBE>()), Times.Never);
        }

        [TestMethod]
        public void FindAll_ReturnsMppResult()
        {
            var expected = new List<TicketBE> { new TicketBE { Id = 1, Subject = "t" } };
            _mockTicketMPP.Setup(m => m.FindAll()).Returns(expected);

            List<TicketBE> result = _ticketBLL.FindAll();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("t", result[0].Subject);
        }

        [TestMethod]
        public void FindByUser_ReturnsMppResult()
        {
            var expected = new List<TicketBE> { new TicketBE { Id = 2, UserId = 7, Subject = "own" } };
            _mockTicketMPP.Setup(m => m.FindByUser(7)).Returns(expected);

            List<TicketBE> result = _ticketBLL.FindByUser(7);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(7, result[0].UserId);
        }

        [TestMethod]
        public void Close_Valid_CallsMppWithClosedAt()
        {
            _mockTicketMPP.Setup(m => m.Close(5, 1, It.IsAny<DateTime>())).Returns(true);

            bool result = _ticketBLL.Close(5, 1);

            Assert.IsTrue(result);
            _mockTicketMPP.Verify(m => m.Close(5, 1, It.IsAny<DateTime>()), Times.Once);
        }

        [TestMethod]
        public void Close_InvalidIds_ReturnsFalse()
        {
            Assert.IsFalse(_ticketBLL.Close(0, 1));
            Assert.IsFalse(_ticketBLL.Close(5, 0));
            _mockTicketMPP.Verify(m => m.Close(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }
    }
}
