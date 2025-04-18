using AutoMapper;
using DataEF;
using DataEF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using TaxLedgerAPI.Profiles;
using TaxLedgerAPI.Services;
using TaxLedgerAPI.Structs;

namespace TaxLedgerApiTests.Services
{
    public class BracketServiceTests
    {
        private readonly List<Bracket> data;
        private readonly Mock<DbSet<Bracket>> mockSet;
        private readonly Mock<ContextParameterlessConstructor> mockContext;
        private readonly IMapper mapper;
        private readonly IBracketService service;

        public BracketServiceTests()
        {
            data = new List<Bracket>();

            mockSet = new Mock<DbSet<Bracket>>();
            mockSet.As<IQueryable<Bracket>>().Setup(m => m.Provider).Returns(data.AsQueryable().Provider);
            mockSet.As<IQueryable<Bracket>>().Setup(m => m.Expression).Returns(data.AsQueryable().Expression);
            mockSet.As<IQueryable<Bracket>>().Setup(m => m.ElementType).Returns(data.AsQueryable().ElementType);
            mockSet.As<IQueryable<Bracket>>().Setup(m => m.GetEnumerator()).Returns(data.AsQueryable().GetEnumerator());

            mockContext = new Mock<ContextParameterlessConstructor>();
            mockContext.Setup(c => c.Brackets).Returns(mockSet.Object);

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new BracketProfile()));
            mapper = new Mapper(configuration);

            service = service = new BracketService(new Mock<ILogger<BracketService>>().Object, mockContext.Object, mapper);
        }

        [Fact]
        public void GetBrackets_Success()
        {
            //Arrange
            data.AddRange(new Bracket { Id = 1, Category = 0.2M }, new Bracket { Id = 2, Category = 0.3M });

            //Act
            var result = service.GetBrackets();

            //Assert
            Assert.True(result?.Success);
            Assert.Equal(MessageStruct.RetrieveSuccess, result?.Message);
            Assert.Equal(2, result?.Data?.Count());
            Assert.Equal(data.First().Id, result?.Data?.First().Id);
            Assert.Equal(data.Last().Id, result?.Data?.Last().Id);
            mockContext.Verify(c => c.Brackets, Times.Once);
        }

        [Fact]
        public void GetBrackets_ThrowsException()
        {
            //Arrange
            mockContext.Setup(c => c.Brackets).Throws(new Exception());

            //Act
            var result = service.GetBrackets();

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.RetrieveFailed, result?.Message);
            Assert.Null(result?.Data);
            mockContext.Verify(c => c.Brackets, Times.Once);
        }

        [Fact]
        public void AddBracket_Success()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Bracket { Category = 0.2M };

            mockSet.Setup(m => m.Add(It.IsAny<Bracket>())).Callback<Bracket>(data.Add);

            //Act
            var result = service.AddBracket(model);

            //Assert
            Assert.True(result?.Success);
            Assert.Equal(MessageStruct.TransactionSuccess, result?.Message);
            Assert.NotEmpty(data);
            Assert.Equal(model.Category, data.First().Category);
            mockContext.Verify(c => c.Brackets.Add(It.IsAny<Bracket>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void AddBracket_EntityExist()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Bracket { Category = 0.2M };

            data.Add(new Bracket { Id = 1, Category = 0.2M });

            //Act
            var result = service.AddBracket(model);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.EntityExist, result?.Message);
            mockContext.Verify(c => c.Brackets.Add(It.IsAny<Bracket>()), Times.Never);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Fact]
        public void AddBracket_ThrowsException()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Bracket { Category = 0.2M };

            mockContext.Setup(c => c.SaveChanges()).Throws(new Exception());

            //Act
            var result = service.AddBracket(model);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.TransactionFailed, result?.Message);
            mockContext.Verify(c => c.Brackets.Add(It.IsAny<Bracket>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateBracket_Success()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Bracket { Id = 1, Category = 0.3M };

            data.Add(new Bracket { Id = 1, Category = 0.2M });

            //Act
            var result = service.UpdateBracket(model);

            //Assert
            Assert.True(result?.Success);
            Assert.Equal(MessageStruct.TransactionSuccess, result?.Message);
            Assert.Equal(model.Category, data.First(m => m.Id == model.Id).Category);
            mockContext.Verify(c => c.Brackets, Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateBracket_EntityNotFound()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Bracket { Id = 1, Category = 0.2M };

            //Act
            var result = service.UpdateBracket(model);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.EntityNotFound, result?.Message);
            mockContext.Verify(c => c.Brackets, Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Fact]
        public void UpdateBracket_ThrowsException()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Bracket { Id = 1, Category = 0.2M };

            data.Add(new Bracket { Id = 1, Category = 0.2M });

            mockContext.Setup(c => c.SaveChanges()).Throws(new Exception());

            //Act
            var result = service.UpdateBracket(model);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.TransactionFailed, result?.Message);
            mockContext.Verify(c => c.Brackets, Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteBracket_Success()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Bracket { Id = 1 };

            data.Add(new Bracket { Id = 1, Category = 0.2M });

            mockSet.Setup(m => m.Remove(It.IsAny<Bracket>())).Callback<Bracket>(entity => data.Remove(entity));

            //Act
            var result = service.DeleteBracket(model.Id);

            //Assert
            Assert.True(result?.Success);
            Assert.Equal(MessageStruct.TransactionSuccess, result?.Message);
            Assert.Empty(data);
            mockContext.Verify(c => c.Brackets.Remove(It.IsAny<Bracket>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteBracket_EntityNotFound()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Bracket { Id = 1 };

            //Act
            var result = service.DeleteBracket(model.Id);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.EntityNotFound, result?.Message);
            mockContext.Verify(c => c.Brackets, Times.Once);
            mockContext.Verify(c => c.Brackets.Remove(It.IsAny<Bracket>()), Times.Never);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Fact]
        public void DeleteBracket_ThrowsException()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Bracket { Id = 1 };

            data.Add(new Bracket { Id = 1, Category = 0.2M });

            mockContext.Setup(c => c.SaveChanges()).Throws(new Exception());

            //Act
            var result = service.DeleteBracket(model.Id);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.TransactionFailed, result?.Message);
            mockContext.Verify(c => c.Brackets.Remove(It.IsAny<Bracket>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }
    }
}
