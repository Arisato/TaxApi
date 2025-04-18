using System.Text.Json.Serialization;

namespace TaxLedgerAPI.Models.Responses
{
    public class ResponseGeneric<T> : Response
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; } 
    }
}
