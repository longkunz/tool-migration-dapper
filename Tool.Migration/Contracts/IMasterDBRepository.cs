using DapperASPNetCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tool.VIP.Migrator.Entities;

namespace DapperASPNetCore.Contracts
{
    public interface IMasterDBRepository
    {
        public Task<IEnumerable<UserSetting>> GetUserSettings();

        Task<IEnumerable<VehicleDevice>> GetVehicleDevices();
    }
}
