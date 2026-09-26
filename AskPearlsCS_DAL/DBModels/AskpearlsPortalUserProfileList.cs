using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserProfileList
{
    public long UprofileId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? Gender { get; set; }

    public DateTime? BirthDate { get; set; }

    public int? BirthDateDay { get; set; }

    public int? BirthDateMonth { get; set; }

    public int? BirthDateYear { get; set; }

    public string? PersonalAvatarColorCode { get; set; }

    public string? DatingAvatarColorCode { get; set; }

    public string? Horoscope { get; set; }

    public string? ZodiacSign { get; set; }

    public string? UserStats { get; set; }

    public string? UserStatus { get; set; }

    public string? UserMoan { get; set; }

    public string? UserMood { get; set; }

    public string? UserDoingNow { get; set; }

    public bool? AutoClick { get; set; }

    public int? GenderId { get; set; }

    public int? SexualId { get; set; }

    public int? MaritalStatusId { get; set; }

    public int? EthnicityId { get; set; }

    public int? EyeId { get; set; }

    public int? HairId { get; set; }

    public int? BodyId { get; set; }

    public int? SmokeId { get; set; }

    public int? DrinkId { get; set; }

    public int? DrugId { get; set; }

    public string? Occupation { get; set; }

    public string? School { get; set; }

    public string? Pets { get; set; }

    public string? Car { get; set; }

    public int? WeightValue { get; set; }

    public string? WeightMeasureText { get; set; }

    public int? HeightValue { get; set; }

    public string? HeightMeasureText { get; set; }

    public string? ThingLike { get; set; }

    public string? ListenTo { get; set; }

    public string? Profession { get; set; }

    public string? InterestIn { get; set; }

    public string? FavoriteFood { get; set; }

    public string? FavoriteBooks { get; set; }

    public string? FavoriteMovies { get; set; }

    public string? FavoriteSongs { get; set; }

    public string? Hobbies { get; set; }

    public string? Talent { get; set; }

    public string? PerfectMate { get; set; }

    public string? TurnOnOff { get; set; }

    public string? PerfectDate { get; set; }

    public string? MeaningLife { get; set; }

    public string? CrazyDone { get; set; }

    public string? AnywhereNow { get; set; }

    public string? NotLiveWithout { get; set; }

    public string? DoForFun { get; set; }

    public string? BestKnowMe { get; set; }

    public string? DatingOrientation { get; set; }

    public string? DatingFor { get; set; }

    public string? DatingWith { get; set; }

    public string? AboutMe { get; set; }

    public string? VideoTitle { get; set; }

    public string? VideoDescription { get; set; }

    public string? VideoYouTubeUrl { get; set; }

    public bool? IsDating { get; set; }

    public bool? IsBusiness { get; set; }

    public bool? VideoIsSubmit { get; set; }

    public bool? VideoIsActive { get; set; }

    public bool? VideoIsValidate { get; set; }

    public int? DatingJoinDay { get; set; }

    public int? DatingJoinMonth { get; set; }

    public int? DatingJoinYear { get; set; }

    public DateTime? DatingJoinDate { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSystemUser { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }

    public bool? IsDisplayToken { get; set; }
}
