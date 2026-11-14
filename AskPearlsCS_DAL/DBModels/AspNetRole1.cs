using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AspNetRole1
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; } = new List<AspNetRoleClaim>();

    public virtual ICollection<AspNetUser1> Users { get; set; } = new List<AspNetUser1>();
}
