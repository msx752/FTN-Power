using FTNPower.Core.ApplicationService;
using FTNPower.Core.Interfaces;
using FTNPower.Model.Enums;
using FTNPower.Model.Tables;
using System.Linq;
using System.Threading.Tasks;

namespace FTNPower.Core.DomainService.SubRepositories
{
    public class UserRepo
    {
        private readonly IRedisService _redis;
        private readonly IUnitOfWork _uow;

        public UserRepo(IUnitOfWork uow, IRedisService redisService)
        {
            _uow = uow;
            _redis = redisService;
        }

        public Task<NameState> AddOrGetNameStateAsync(FortniteUser user, string guildId)
        {
            return Task.Run(() =>
            {
                var ns = _uow.Db<NameState>().Where(f => f.FortniteUserId == user.Id && f.DiscordServerId == guildId).FirstOrDefault();
                if (ns == null)
                {
                    ns = new NameState()
                    {
                        DiscordServerId = guildId,
                        FortniteUserId = user.Id
                    };
                    _uow.Db<NameState>().Add(ns);
                    _uow.Commit();
                }
                return ns;
            });
        }

        public Task<FortniteUser> AddOrGetUserAsync(string userId, string guildId, GameUserMode gameUserMode = GameUserMode.PVE)
        {
            return Task.Run(async () =>
            {
                var nUser = _uow.Db<FortniteUser>().GetById(userId);
                if (nUser == null)
                {
                    nUser = new FortniteUser()
                    {
                        Id = userId,
                        GameUserMode = gameUserMode,
                        EpicId = null,
                    };
                    _uow.Db<FortniteUser>().Add(nUser);
                    _uow.Commit();
                }
                await AddOrGetNameStateAsync(nUser, guildId);
                return nUser;
            });
        }
    }
}