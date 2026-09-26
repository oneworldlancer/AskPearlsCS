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
    public Guid OrganizationId { get; set; }
    public Guid WebsiteId { get; set; }
    public string Name { get; set; } = "";
    public string PublicKey { get; set; } = "";
    public string Status { get; set; } = "Draft";
    public int PublishedVersion { get; set; }
}

public class WidgetVersion : Entity
{
    public Guid WidgetId { get; set; }
    public int VersionNumber { get; set; }
    public string Status { get; set; } = "Draft";
    public string ConfigurationJson { get; set; } = "{}";
    public DateTime? PublishedAtUtc { get; set; }
}

public class Question : Entity
{
    public Guid WidgetVersionId { get; set; }
    public int SortOrder { get; set; }
    public string Text { get; set; } = "";
    public string Key { get; set; } = "";
}

public class Answer : Entity
{
    public Guid QuestionId { get; set; }
    public string Text { get; set; } = "";
    public string Value { get; set; } = "";
}

public class DecisionRule : Entity
{
    public Guid WidgetVersionId { get; set; }
    public string AnswerValue { get; set; } = "";
    public string RecommendationKey { get; set; } = "";
    public int Score { get; set; }
}

public class Event : Entity
{
    public Guid OrganizationId { get; set; }
    public Guid WidgetId { get; set; }
    public string SessionId { get; set; } = "";
    public string EventType { get; set; } = "";
    public string PayloadJson { get; set; } = "{}";
}

public class Subscription : Entity
{
    public Guid OrganizationId { get; set; }
    public string Plan { get; set; } = "Free";
    public string Status { get; set; } = "Active";
}
