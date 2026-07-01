namespace auth.Application.Interfaces
{
    public interface IClientInfoProvider
    {
        string GetIpAddress();
        string GetUserAgent();
    }
}
