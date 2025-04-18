using Moq;
using DataEF;
using Microsoft.EntityFrameworkCore;
using DataEF.Models;
using TaxLedgerAPI.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;
using TaxLedgerAPI.Profiles;
using TaxLedgerAPI.Structs;
using System.Net;

namespace TaxLedgerApiTests.Services
{
    public class MunicipalityServiceTests
    {
        private readonly List<Municipality> data;
        private readonly Mock<DbSet<Municipality>> mockSet;
        private readonly Mock<ContextParameterlessConstructor> mockContext;
        private readonly IMapper mapper;
        private readonly IMunicipalityService service;

        public MunicipalityServiceTests() 
        {
            data = new List<Municipality>();

            mockSet = new Mock<DbSet<Municipality>>();
            mockSet.As<IQueryable<Municipality>>().Setup(m => m.Provider).Returns(data.AsQueryable().Provider);
            mockSet.As<IQueryable<Municipality>>().Setup(m => m.Expression).Returns(data.AsQueryable().Expression);
            mockSet.As<IQueryable<Municipality>>().Setup(m => m.ElementType).Returns(data.AsQueryable().ElementType);
            mockSet.As<IQueryable<Municipality>>().Setup(m => m.GetEnumerator()).Returns(data.AsQueryable().GetEnumerator());

            mockContext = new Mock<ContextParameterlessConstructor>();
            mockContext.Setup(c => c.Municipalities).Returns(mockSet.Object);

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new MunicipalityProfile()));
            mapper = new Mapper(configuration);

            service = service = new MunicipalityService(new Mock<ILogger<MunicipalityService>>().Object, mockContext.Object, mapper);
        }

        [Fact]
        public void GetMunicipalities_Success()
        {
            //Arrange
            data.AddRange(new Municipality { Id = 1, Name = "Copenhagen" }, new Municipality { Id = 2, Name = "London" });

            //Act
            var result = service.GetMunicipalities();

            //Assert
            Assert.True(result?.Success);
            Assert.Equal(MessageStruct.RetrieveSuccess, result?.Message);
            Assert.Equal(2, result?.Data?.Count());
            Assert.Equal(data.First().Id, result?.Data?.First().Id);
            Assert.Equal(data.Last().Id, result?.Data?.Last().Id);
            mockContext.Verify(c => c.Municipalities, Times.Once);
        }

        [Fact]
        public void GetMunicipalities_ThrowsException()
        {
            //Arrange
            mockContext.Setup(c => c.Municipalities).Throws(new Exception());

            //Act
            var result = service.GetMunicipalities();

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.RetrieveFailed, result?.Message);
            Assert.Null(result?.Data);
            mockContext.Verify(c => c.Municipalities, Times.Once);
        }

        [Fact]
        public void AddMunicipality_Success()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Municipality { Name = "Copenhagen" };

            mockSet.Setup(m => m.Add(It.IsAny<Municipality>())).Callback<Municipality>(data.Add);

            //Act
            var result = service.AddMunicipality(model);

            //Assert
            Assert.True(result?.Success);
            Assert.Equal(MessageStruct.TransactionSuccess, result?.Message);
            Assert.NotEmpty(data);
            Assert.Equal(model.Name, data.First().Name);
            mockContext.Verify(c => c.Municipalities.Add(It.IsAny<Municipality>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void AddMunicipality_EntityExist()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Municipality { Name = "Copenhagen" };

            data.Add(new Municipality { Id = 1, Name = "Copenhagen" });

            //Act
            var result = service.AddMunicipality(model);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.EntityExist, result?.Message);
            mockContext.Verify(c => c.Municipalities.Add(It.IsAny<Municipality>()), Times.Never);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Fact]
        public void AddMunicipality_ThrowsException()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Municipality { Name = "Copenhagen" };

            mockContext.Setup(c => c.SaveChanges()).Throws(new Exception());

            //Act
            var result = service.AddMunicipality(model);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.TransactionFailed, result?.Message);
            mockContext.Verify(c => c.Municipalities.Add(It.IsAny<Municipality>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateMunicipality_Success()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Municipality { Id = 1, Name = "New Name" };

            data.Add(new Municipality { Id = 1, Name = "Copenhagen" });

            //Act
            var result = service.UpdateMunicipality(model);

            //Assert
            Assert.True(result?.Success);
            Assert.Equal(MessageStruct.TransactionSuccess, result?.Message);
            Assert.Equal(model.Name, data.First(m => m.Id == model.Id).Name);
            mockContext.Verify(c => c.Municipalities, Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateMunicipality_EntityNotFound()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Municipality { Id = 1, Name = "New Name" };

            //Act
            var result = service.UpdateMunicipality(model);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.EntityNotFound, result?.Message);
            mockContext.Verify(c => c.Municipalities, Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Fact]
        public void UpdateMunicipality_ThrowsException()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Municipality { Id = 1, Name = "New Name" };

            data.Add(new Municipality { Id = 1, Name = "Copenhagen" });

            mockContext.Setup(c => c.SaveChanges()).Throws(new Exception());

            //Act
            var result = service.UpdateMunicipality(model);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.TransactionFailed, result?.Message);
            mockContext.Verify(c => c.Municipalities, Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteMunicipality_Success()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Municipality { Id = 1 };

            data.Add(new Municipality { Id = 1, Name = "Copenhagen" });

            mockSet.Setup(m => m.Remove(It.IsAny<Municipality>())).Callback<Municipality>(entity => data.Remove(entity));

            //Act
            var result = service.DeleteMunicipality(model.Id);

            //Assert
            Assert.True(result?.Success);
            Assert.Equal(MessageStruct.TransactionSuccess, result?.Message);
            Assert.Empty(data);
            mockContext.Verify(c => c.Municipalities.Remove(It.IsAny<Municipality>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteMunicipality_EntityNotFound()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Municipality { Id = 1 };

            //Act
            var result = service.DeleteMunicipality(model.Id);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.EntityNotFound, result?.Message);
            mockContext.Verify(c => c.Municipalities, Times.Once);
            mockContext.Verify(c => c.Municipalities.Remove(It.IsAny<Municipality>()), Times.Never);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Fact]
        public void DeleteMunicipality_ThrowsException()
        {
            //Arrange
            var model = new TaxLedgerAPI.Models.Municipality { Id = 1 };

            data.Add(new Municipality { Id = 1, Name = "Copenhagen" });

            mockContext.Setup(c => c.SaveChanges()).Throws(new Exception());

            //Act
            var result = service.DeleteMunicipality(model.Id);

            //Assert
            Assert.False(result?.Success);
            Assert.Equal(MessageStruct.TransactionFailed, result?.Message);
            mockContext.Verify(c => c.Municipalities.Remove(It.IsAny<Municipality>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }
    }
}
