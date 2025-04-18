using System;
using System.Collections.Generic;

namespace DataEF.Models;

public partial class Ledger
{
    public int Id { get; set; }

    public int MunicipalityId { get; set; }

    public int BracketId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual Bracket Bracket { get; set; } = null!;

    public virtual Municipality Municipality { get; set; } = null!;
}
