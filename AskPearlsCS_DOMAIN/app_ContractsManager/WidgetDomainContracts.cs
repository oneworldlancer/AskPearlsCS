namespace AskPearlsCS_DOMAIN.app_ContractsManager;

public sealed record DomainQuotaDto(string PlanCode, long Used, long? Limit, long? Remaining);
public sealed record DomainEntryDto(string Domain, string DomainTokenId, string VerificationStatus, DateTime? VerifiedAtUtc);
public sealed record DomainChallengeDto(string Domain, string VerificationStatus, string RecordType, string Host, string Value, DateTime ExpiresAtUtc);
