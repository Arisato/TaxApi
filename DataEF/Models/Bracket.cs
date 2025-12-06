namespace DataEF.Models;

public class Bracket
{
    public int Id { get; set; }

    public decimal Category { get; set; }

    public virtual ICollection<Ledger> Ledgers { get; set; } = new List<Ledger>();
}
