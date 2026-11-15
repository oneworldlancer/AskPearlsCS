namespace AskPearlsCS_PORTAL.app_Manager
{
    public interface ISupportEmailService
    {
        Task SendAsync(
            string name,
            string email,
            string message,
            string? reference,
            CancellationToken cancellationToken = default
        );
    }
}
