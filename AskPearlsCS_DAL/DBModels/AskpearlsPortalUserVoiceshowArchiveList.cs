using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserVoiceshowArchiveList
{
    public long ArchiveVoiceShowId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? VoiceShowTicketId { get; set; }

    public string? CamFmsurl { get; set; }

    public string? VoiceShowId { get; set; }

    public string? VoiceShowSessionId { get; set; }

    public string? ModelId { get; set; }

    public string? ModelSessionId { get; set; }

    public string? ModelType { get; set; }

    public string? ModelRole { get; set; }

    public string? MemberId { get; set; }

    public string? MemberSessionId { get; set; }

    public string? MemberType { get; set; }

    public string? MemberRole { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
