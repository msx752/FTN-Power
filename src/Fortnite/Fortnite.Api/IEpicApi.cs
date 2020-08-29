using Fortnite.Core.Services;
using Fortnite.Model.Enums;
using Fortnite.Model.Responses;
using Fortnite.Model.Responses.QueryProfile;
using Fortnite.Model.Responses.WorldInfo;
using fortniteLib.Responses.Catalog;
using fortniteLib.Responses.Pvp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fortnite.Api
{
    public interface IEpicApi
    {
        Task StartVerifier();
        KeyValuePair<string, Catalog> GetCatalog();
        Task<KeyValuePair<string, IQueryProfile>> GetPVEProfileById(string epicId);
        Task<KeyValuePair<string, IQueryProfile>> GetPVEProfileByName(string userName);
        Task<KeyValuePair<string, BattleRoyaleStats>> GetPVPProfileById(string epicId);
        Task<KeyValuePair<string, BattleRoyaleStats>> GetPVPProfileByName(string userName);
        KeyValuePair<string, PveCurrentGameInfo> GetUserGameInfo(string epicId);
        KeyValuePair<string, Lookup> GetUserIdByName(string userName);
        KeyValuePair<string, Lookup> GetUserNameById(string epicId);
        KeyValuePair<string, List<Lookup>> GetUserNameByIds(params string[] epicIds);
        string GetVersion();
        KeyValuePair<string, WorldInfo> GetWorldInfo();
    }
}