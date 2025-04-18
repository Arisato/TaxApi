namespace TaxLedgerAPI.Models
{
    public class Ledger
    {
        public int Id { get; set; }

        public int MunicipalityId { get; set; }

        public int BracketId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }
    }
}
