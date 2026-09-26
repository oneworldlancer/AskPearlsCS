using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class Image
{
    public int ImageId { get; set; }

    public string? ImageType { get; set; }

    public string? ImageName { get; set; }

    public int? ImageLength { get; set; }

    public byte[]? ImageData { get; set; }
}
