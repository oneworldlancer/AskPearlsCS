using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class UserLogin
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public int? Age { get; set; }

    public string MailId { get; set; } = null!;

    public string? Password { get; set; }

    public string? Gender { get; set; }

    public string? LookingFor { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public int? ZipCode { get; set; }

    public string? ProfileLanguageId { get; set; }

    public byte[]? ProfilePicture { get; set; }

    public string? OnlineStatus { get; set; }

    public string? SecretQuestion { get; set; }

    public string? SecretAnswer { get; set; }

    public byte[]? BackgroundImage { get; set; }

    public DateOnly RegistrationDate { get; set; }

    public TimeOnly RegistrationTime { get; set; }

    public int Status { get; set; }

    public string? ProfilePictureStatus { get; set; }

    public string LoginType { get; set; } = null!;

    public string? SocialUserName { get; set; }

    public string? SocialUserId { get; set; }

    public string? States { get; set; }

    public string? Themes { get; set; }

    public string? UserMood { get; set; }

    public string? UserPopular { get; set; }

    public string? TodayFeeling { get; set; }

    public string? MyStatus { get; set; }
}
