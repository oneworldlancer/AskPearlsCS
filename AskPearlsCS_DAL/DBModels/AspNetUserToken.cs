using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AspNetUserToken
{
    public string UserId { get; set; } = null!;

    public string LoginProvider { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Value { get; set; }

    public virtual AspNetUser1 User { get; set; } = null!;
}
