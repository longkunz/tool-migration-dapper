using DapperASPNetCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DapperASPNetCore.Contracts
{
    public interface IUserSettingRepository
    {
        public Task<IEnumerable<UserSetting>> GetUserSettings();
    }
}
