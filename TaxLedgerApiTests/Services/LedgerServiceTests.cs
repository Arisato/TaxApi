using AutoMapper;
using DataEF;
using DataEF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaxLedgerAPI.Profiles;
using TaxLedgerAPI.Services;
using TaxLedgerAPI.Structs;

namespace TaxLedgerApiTests.Services
{
    public class LedgerServiceTests
    {
        private readonly List<Ledger> data;
        private readonly Mock<DbSet<Ledger>> mockSet;
        private readonly Mock<Context> mockContext;
        private readonly IMapper mapper;
        private readonly ILedgerService service;

        public LedgerServiceTests()
        {
            data = new List<Ledger>();

            mockSet = new Mock<DbSet<Ledger>>();
            mockSet.As<IQueryable<Ledger>>().Setup(m => m.Provider).Returns(data.AsQueryable().Provider);
            mockSet.As<IQueryable<Ledger>>().Setup(m => m.Expression).Returns(data.AsQueryable().Expression);
            mockSet.As<IQueryable<Ledger>>().Setup(m => m.ElementType).Returns(data.AsQueryable().ElementType);
            mockSet.As<IQueryable<Ledger>>().Setup(m => m.GetEnumerator()).Returns(data.AsQueryable().GetEnumerator());

            mockContext = new Mock<Context>();
            mockContext.Setup(c => c.Ledgers).Returns(mockSet.Object);

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new LedgerProfile()));
            mapper = new Mapper(configuration);

            service = new LedgerService(new Mock<ILogger<LedgerService>>().Object, mockContext.Object, mapper);
        }

        [Fact]
        public void GetLedgerByNameAndDate_Success()
        {
            //Arrange
            data.AddRange(new Ledger
            {
                Id = 1,
                MunicipalityId = 1,
                BracketId = 2,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 12, 31),
                Municipality = new Municipality { Id = 1, Name = "Copenhagen" },
                Bracket = new Bracket { Id = 2, Category = 0.2M }
            },
            new Ledger
            {
                Id = 2,
                MunicipalityId = 1,
                BracketId = 1,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 01, 01),
                Municipality = new Municipality { Id = 1, Name = "Copenhagen" },
                Bracket = new Bracket { Id = 1, Category = 0.1M }
            });

            //Act
            var result = service.GetLedger(data.Last().Municipality.Name, data.Last().StartDate);

            //Assert
            Assert.True(result?.Success);
            Assert.Equal(MessageStruct.RetrieveSuccess, result?.Message);
            Assert.Equal(data.Last().Bracket.Category, result?.Data);
            mockContext.Verify(c => c.Ledgers, Times.Once);
        }

        [Fact]
        public void GetLedgerByNameAndDate_EntityNotFound()
        {
            //Arrange
            data.Add(new Ledger
            {
                Id = 1,
                MunicipalityId = 1,
                BracketId = 2,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 12, 31),
                Municipality = new Municipality { Id = 1, Name = "Copenhagen" },
                Bracket = new Bracket { Id = 2, Category = 0.2M }
            });

            //Act
            var result = service.GetLedger("Wrong Name", data.Last().StartDate);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.EntityNotFound, result?.Message);
            Assert.Equal(0, result?.Data);
            mockContext.Verify(c => c.Ledgers, Times.Once);
        }

        [Fact]
        public void GetLedgers_ThrowsException()
        {
            //Arrange
            data.Add(new Ledger
            {
                Id = 1,
                MunicipalityId = 1,
                BracketId = 2,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 12, 31),
                Municipality = new Municipality { Id = 1, Name = "Copenhagen" },
                Bracket = new Bracket { Id = 2, Category = 0.2M }
            });

            mockContext.Setup(c => c.Ledgers).Throws(new Exception());

            //Act
            var result = service.GetLedger(data.First().Municipality.Name, data.First().StartDate);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.RetrieveFailed, result?.Message);
            Assert.Equal(0, result?.Data);
            mockContext.Verify(c => c.Ledgers, Times.Once);
        }

        [Fact]
        public void AddLedger_Success()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Ledger
            {
                MunicipalityId = 1,
                BracketId = 1,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 12, 31),
            };

            mockSet.Setup(m => m.Add(It.IsAny<Ledger>())).Callback<Ledger>(data.Add);

            //Act
            var result = service.AddLedger(model);

            //Assert
            Assert.True(result?.Success);
            Assert.Equal(MessageStruct.TransactionSuccess, result?.Message);
            Assert.NotEmpty(data);
            Assert.Equal(model.MunicipalityId, data.First().MunicipalityId);
            Assert.Equal(model.BracketId, data.First().BracketId);
            Assert.Equal(model.StartDate, data.First().StartDate);
            Assert.Equal(model.EndDate, data.First().EndDate);
            mockContext.Verify(c => c.Ledgers.Add(It.IsAny<Ledger>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void AddLedger_EntityExist()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Ledger
            {
                MunicipalityId = 1,
                BracketId = 2,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 12, 31),
            };

            data.Add(new Ledger
            {
                Id = 1,
                MunicipalityId = 1,
                BracketId = 2,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 12, 31),
            });

            //Act
            var result = service.AddLedger(model);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.EntityExist, result?.Message);
            mockContext.Verify(c => c.Ledgers.Add(It.IsAny<Ledger>()), Times.Never);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Fact]
        public void AddLedger_ThrowsException()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Ledger
            {
                MunicipalityId = 1,
                BracketId = 2,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 12, 31),
            };

            mockContext.Setup(c => c.SaveChanges()).Throws(new Exception());

            //Act
            var result = service.AddLedger(model);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.TransactionFailed, result?.Message);
            mockContext.Verify(c => c.Ledgers.Add(It.IsAny<Ledger>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateLedger_Success()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Ledger
            {
                Id = 1,
                MunicipalityId = 2,
                BracketId = 1,
                StartDate = new DateOnly(2024, 02, 02),
                EndDate = new DateOnly(2024, 12, 31),
            };

            data.Add(new Ledger
            {
                Id = 1,
                MunicipalityId = 1,
                BracketId = 2,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 12, 31),
            });

            //Act
            var result = service.UpdateLedger(model);

            //Assert
            Assert.True(result?.Success);
            Assert.Equal(MessageStruct.TransactionSuccess, result?.Message);
            Assert.Equal(model.MunicipalityId, data.First(m => m.Id == model.Id).MunicipalityId);
            Assert.Equal(model.BracketId, data.First(m => m.Id == model.Id).BracketId);
            Assert.Equal(model.StartDate, data.First(m => m.Id == model.Id).StartDate);
            Assert.Equal(model.EndDate, data.First(m => m.Id == model.Id).EndDate);
            mockContext.Verify(c => c.Ledgers, Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateLedger_EntityNotFound()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Ledger
            {
                Id = 1,
                MunicipalityId = 2,
                BracketId = 1,
                StartDate = new DateOnly(2024, 02, 02),
                EndDate = new DateOnly(2024, 12, 31),
            };

            //Act
            var result = service.UpdateLedger(model);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.EntityNotFound, result?.Message);
            mockContext.Verify(c => c.Ledgers, Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Fact]
        public void UpdateLedger_ThrowsException()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Ledger
            {
                Id = 1,
                MunicipalityId = 1,
                BracketId = 2,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 12, 31),
            };

            data.Add(new Ledger
            {
                Id = 1,
                MunicipalityId = 1,
                BracketId = 2,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 12, 31),
            });

            mockContext.Setup(c => c.SaveChanges()).Throws(new Exception());

            //Act
            var result = service.UpdateLedger(model);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.TransactionFailed, result?.Message);
            mockContext.Verify(c => c.Ledgers, Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteLedger_Success()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Ledger { Id = 1 };

            data.Add(new Ledger
            {
                Id = 1,
                MunicipalityId = 1,
                BracketId = 2,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 12, 31),
            });

            mockSet.Setup(m => m.Remove(It.IsAny<Ledger>())).Callback<Ledger>(entity => data.Remove(entity));

            //Act
            var result = service.DeleteLedger(model.Id);

            //Assert
            Assert.True(result?.Success);
            Assert.Equal(MessageStruct.TransactionSuccess, result?.Message);
            Assert.Empty(data);
            mockContext.Verify(c => c.Ledgers.Remove(It.IsAny<Ledger>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteLedger_EntityNotFound()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Ledger { Id = 1 };

            //Act
            var result = service.DeleteLedger(model.Id);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.EntityNotFound, result?.Message);
            mockContext.Verify(c => c.Ledgers, Times.Once);
            mockContext.Verify(c => c.Ledgers.Remove(It.IsAny<Ledger>()), Times.Never);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Fact]
        public void DeleteLedger_ThrowsException()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Ledger { Id = 1 };

            data.Add(new Ledger
            {
                Id = 1,
                MunicipalityId = 1,
                BracketId = 2,
                StartDate = new DateOnly(2024, 01, 01),
                EndDate = new DateOnly(2024, 12, 31),
            });

            mockContext.Setup(c => c.SaveChanges()).Throws(new Exception());

            //Act
            var result = service.DeleteLedger(model.Id);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.TransactionFailed, result?.Message);
            mockContext.Verify(c => c.Ledgers.Remove(It.IsAny<Ledger>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }
    }
}
