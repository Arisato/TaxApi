using TaxLedgerAPI.Structs;
using TaxLedgerAPI.Helpers;
using System.Net;

namespace TaxLedgerApiTests.Helpers
{
    public class StatusCodeHelperTests
    {
        [Fact]
        public void GetStatusCode_OK_For_RetrieveSuccess()
        {
            //Arrange
            var message = MessageStruct.RetrieveSuccess;

            //Act
            var result = StatusCodeHelper.GetStatusCode(message);

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, result);
        }

        [Fact]
        public void GetStatusCode_OK_For_TransactionSuccess()
        {
            //Arrange
            var message = MessageStruct.TransactionSuccess;

            //Act
            var result = StatusCodeHelper.GetStatusCode(message);

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, result);
        }

        [Fact]
        public void GetStatusCode_InternalServerError_For_EntityExist()
        {
            //Arrange
            var message = MessageStruct.EntityExist;

            //Act
            var result = StatusCodeHelper.GetStatusCode(message);

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, result);
        }

        [Fact]
        public void GetStatusCode_InternalServerError_For_RetrieveFailed()
        {
            //Arrange
            var message = MessageStruct.RetrieveFailed;

            //Act
            var result = StatusCodeHelper.GetStatusCode(message);

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, result);
        }

        [Fact]
        public void GetStatusCode_InternalServerError_For_TransactionFailed()
        {
            //Arrange
            var message = MessageStruct.TransactionFailed;

            //Act
            var result = StatusCodeHelper.GetStatusCode(message);

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, result);
        }

        [Fact]
        public void GetStatusCode_NotFound_For_EntityNotFound()
        {
            //Arrange
            var message = MessageStruct.EntityNotFound;

            //Act
            var result = StatusCodeHelper.GetStatusCode(message);

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, result);
        }
    }
}
