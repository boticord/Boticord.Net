namespace Boticord.Net.Types
{
    public abstract class BaseService
    {
        protected readonly Boticord.NetClient Client;
        protected BaseService(Boticord.NetClient client) => Client = client;
    }
}