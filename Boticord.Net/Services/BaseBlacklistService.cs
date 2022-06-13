using System.Threading.Tasks;
using Boticord.Net.Types;
using Boticord.Net.Types.Blacklist;
using Boticord.Net.Types.Interfaces;

namespace Boticord.Net.Services
{
    public class BaseBlacklistService : BaseService
    {
        public BaseBlacklistService(BoticordNetClient client) : base(client) { }

        public async ValueTask<BaseUserWarns> GetWarns(ulong userId)
        {
            return await GetWarns<BaseUserWarns>(userId);
        }
        
        public async ValueTask<T> GetWarns<T>(ulong userId) where T: IUserWarns
        {
            return await Client.GetRequest<T>("warns/" + userId);
        }
    }
}