using System;
using System.Collections.Generic;

namespace DataEF.Models;

public partial class Municipality
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Ledger> Ledgers { get; set; } = new List<Ledger>();
}
