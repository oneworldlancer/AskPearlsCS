namespace AskPearls.Domain;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

public class User : Entity
{
    public string Email { get; set; } = "";
    public string DisplayName { get; set; } = "";
}

public class Organization : Entity
{
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Status { get; set; } = "Active";
}

public class OrganizationMember : Entity
{
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = "Member";
}

public class Website : Entity
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = "";
    public string Domain { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public class Widget : Entity
{

    //////////    Code        → internal/business identifier
    //////////Type        → type of widget
    //////////Name        → administrative/display name
    //////////Title       → visitor-facing widget title
    //////////Description → visitor-facing explanation
    //////////PublicKey   → secure public identifier used by embed
    //////////Status      → Draft / Published / Archived...
    //////////Version     → currently published version

    public Guid OrganizationId { get; set; }
    public Guid WebsiteId { get; set; }

    // Business / application identity
    public string WidgetTokenId { get; set; } = "";

    public string WidgetCode { get; set; } = "";
    public string WidgetType { get; set; } = "";

    public string WidgetName { get; set; } = "";
    public string WidgetTitle { get; set; } = "";
    public string WidgetDescription { get; set; } = "";

    // Public identifier used by the embed script
    public string WidgetPublicKey { get; set; } = "";

    public string WidgetStatusCode { get; set; } = "draft";
    public string WidgetStatusText { get; set; } = "draft";
       public string WidgetStatus { get; set; } = "draft";

    public int WidgetPublishedVersion { get; set; }
}

public class WidgetVersion : Entity
{
    public Guid WidgetId { get; set; }

    public string VersionTokenId { get; set; } = string.Empty;

    public int VersionNumber { get; set; }

    public string Status { get; set; } = "Draft";

    public string ConfigurationJson { get; set; } = "{}";

    public DateTime? PublishedAtUtc { get; set; }
}


public class Question : Entity
{
    public string QuestionTokenId { get; set; } = string.Empty;

    public string WidgetTokenId { get; set; } = string.Empty;

    public string WidgetVersionTokenId { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsVisible { get; set; } = true;

    public List<Answer> Answers { get; set; } = new();
}

public class Answer : Entity
{
    public string AnswerTokenId { get; set; } = string.Empty;

    public string WidgetTokenId { get; set; } = string.Empty;

    public string QuestionTokenId { get; set; } = string.Empty;

    public string WidgetVersionTokenId { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string? ImageTokenId { get; set; }

    public string? ImageServerUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsVisible { get; set; } = true;
}

public class DecisionRule : Entity
{
    public string DecisionRuleTokenId { get; set; }
        = string.Empty;

    public string WidgetTokenId { get; set; }
        = string.Empty;

    public string WidgetVersionTokenId { get; set; }
        = string.Empty;

     //public string WidgetVersionTokenId { get; set; }
     //   = string.Empty;

     public string WidgetQuestionTokenId { get; set; }
        = string.Empty;

     public string WidgetQuestionKey { get; set; }
        = string.Empty;

     public string WidgetAnswerTokenId { get; set; }
        = string.Empty;

     public string WidgetAnswerKey { get; set; }
        = string.Empty;

    public string QuestionTokenId { get; set; }
        = string.Empty;

    public string AnswerTokenId { get; set; }
        = string.Empty;

    public string RecommendationTokenId { get; set; }
        = string.Empty;

    public string AnswerValue { get; set; }
        = string.Empty;

    public string RecommendationKey { get; set; }
        = string.Empty;

    public int Score { get; set; }

    public string? Reason { get; set; }

    public bool IsActive { get; set; } = true;
}

public class WidgetEvent : Entity
{
    public string WidgetEventTokenId { get; set; } = "";

    public Guid OrganizationId { get; set; }

    public Guid WebsiteId { get; set; }

    public Guid WidgetId { get; set; }

    public Guid? WidgetVersionId { get; set; }

    public string SessionId { get; set; } = "";

    public string? VisitorId { get; set; }

    public string EventType { get; set; } = "";

    public Guid? QuestionId { get; set; }

    public Guid? AnswerId { get; set; }

    public Guid? RecommendationItemId { get; set; }

    public string? EventDataJson { get; set; }

    public string? PageUrl { get; set; }

    public string? ReferrerUrl { get; set; }

    public string? UserAgent { get; set; }

    public string? IpAddress { get; set; }
}

public class Subscription : Entity
{
    public Guid OrganizationId { get; set; }
    public string Plan { get; set; } = "Free";
    public string Status { get; set; } = "Active";
}




public class RecommendationItem : Entity
{
    public Guid WidgetVersionId { get; set; }

    public string RecommendationTokenId { get; set; } = "";

    public string Key { get; set; } = "";

    public string Code { get; set; } = "";

    public string Type { get; set; } = "";

    public string Name { get; set; } = "";

    public string Title { get; set; } = "";

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? Url { get; set; }

    public string? CtaText { get; set; }
    public List<string> Highlights { get; set; } = new();
    public string? ImageTokenId { get; set; }
    public string? Text { get; set; }
    public string? WidgetTokenId { get; set; }
    public string? WidgetVersionTokenId { get; set; }
    //public string? CtaText { get; set; }

    public string? HighlightsJson { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsVisible { get; set; } = true;

    public WidgetVersion? WidgetVersion { get; set; }

    public ICollection<DecisionRule> DecisionRules { get; set; }
        = new List<DecisionRule>();
}


public class WidgetSession : Entity
{
    public string WidgetSessionTokenId { get; set; } = "";

    public Guid OrganizationId { get; set; }

    public Guid WebsiteId { get; set; }

    public Guid WidgetId { get; set; }

    public Guid? WidgetVersionId { get; set; }

    public string SessionId { get; set; } = "";

    public string? VisitorId { get; set; }

    public string SessionStatus { get; set; } = "Active";

    public DateTime StartedAtUtc { get; set; }

    public DateTime LastActivityAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public int? DurationSeconds { get; set; }

    public int QuestionCount { get; set; }

    public int AnsweredQuestionCount { get; set; }

    public Guid? RecommendationItemId { get; set; }

    public int? RecommendationScore { get; set; }

    public int? RecommendationPercentage { get; set; }

    public string? PageUrl { get; set; }

    public string? ReferrerUrl { get; set; }

    public string? UserAgent { get; set; }

    public string? IpAddress { get; set; }

    public ICollection<WidgetEvent> Events { get; set; }
        = new List<WidgetEvent>();
}
