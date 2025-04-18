using System.Net;
using TaxLedgerAPI.Structs;

namespace TaxLedgerAPI.Helpers
{
    public static class StatusCodeHelper
    {
        public static int GetStatusCode(string message)
        {
            switch (message)
            {
                case MessageStruct.EntityExist:
                case MessageStruct.RetrieveFailed:
                case MessageStruct.TransactionFailed:
                    return (int)HttpStatusCode.InternalServerError;
                case MessageStruct.EntityNotFound:
                    return (int)HttpStatusCode.NotFound;
                default:
                    return (int)HttpStatusCode.OK;
            }
        }
    }
}
