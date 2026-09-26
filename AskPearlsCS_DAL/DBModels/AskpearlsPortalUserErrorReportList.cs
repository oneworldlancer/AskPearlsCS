using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserErrorReportList
{
    public long ReportId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? AppVersionCode { get; set; }

    public string? CustomData { get; set; }

    public string? StackTrace { get; set; }

    public string? PhoneModel { get; set; }

    public string? AndroidVersion { get; set; }

    public string? AppVersionName { get; set; }

    public string? Logcat { get; set; }

    public string? WhatDislike { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
