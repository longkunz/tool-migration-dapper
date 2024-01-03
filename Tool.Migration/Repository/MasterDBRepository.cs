using Dapper;
using DapperASPNetCore.Context;
using DapperASPNetCore.Contracts;
using DapperASPNetCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tool.VIP.Migrator.Entities;

namespace DapperASPNetCore.Repository
{
    public class MasterDBRepository : IMasterDBRepository
    {
        private readonly DapperContext _context;

        public MasterDBRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserSetting>> GetUserSettings()
        {
            try
            {
                var query = "SELECT us.Id, us.Vehicles, u.Username, u.CompanyId, u.Email, u.RoleIdsJson, us.Inactive FROM dbo.tbl_UserSetting us (NOLOCK) JOIN dbo.tbl_User u (NOLOCK) ON us.Id = u.Id WHERE u.Inactive = 0 AND us.Inactive = 0;";

                using var connection = _context.CreateConnection();
                connection.Open();

                var userSettings = await connection.QueryAsync<UserSetting>(query);
                return userSettings;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VehicleDevice>> GetVehicleDevices()
        {
            try
            {
                //var query = "SELECT * FROM View_VehicleDeviceES";
                var query = @"SELECT d.Id AS DeviceId,
                                   d.DeviceTypeId,
                                   dt.Name as DeviceName,
                                   v.Id,
                                   v.Plate,
                                   v.ActualPlate,
                                   d.CompanyId,
                                   d.Imei,
                                   ISNULL(d.IsLocked, 0) IsLocked,
                                   com.Name AS Company,
		                           v.Inactive
                            FROM dbo.tbl_Device d
                                INNER JOIN dbo.tbl_DeviceType dt
                                    ON d.DeviceTypeId = dt.Id
                                INNER JOIN dbo.tbl_Vehicle AS v
                                    ON v.DeviceId = d.Id
                                LEFT JOIN dbo.tbl_Company com
                                    ON com.Id = d.CompanyId
                            WHERE d.Inactive = 0
                            UNION
                            SELECT d.Id AS DeviceId,
                                   d.DeviceTypeId,
                                   dt.Name as DeviceName,
                                   v.Id 'VehicleId',
                                   v.Plate,
                                   v.ActualPlate,
                                   d.CompanyId,
                                   d.Imei,
                                   ISNULL(d.IsLocked, 0) IsLocked,
                                   com.Name AS Company,
		                           v.Inactive
                            FROM dbo.tbl_Device d
                                INNER JOIN dbo.tbl_DeviceType dt
                                    ON d.DeviceTypeId = dt.Id
                                INNER JOIN dbo.tbl_VehicleDevice vd
                                    ON vd.DeviceId = d.Id
                                INNER JOIN dbo.tbl_Vehicle v
                                    ON v.Id = vd.VehicleId
                                LEFT JOIN dbo.tbl_Company com
                                    ON com.Id = d.CompanyId
                            WHERE d.Inactive = 0
                                  AND vd.IsActive = 1";

                using var connection = _context.CreateConnection();
                connection.Open();

                var vehicleDevices = await connection.QueryAsync<VehicleDevice>(query);
                return vehicleDevices;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
