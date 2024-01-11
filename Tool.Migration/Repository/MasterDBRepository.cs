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

        public async Task<IEnumerable<VehicleDevice>> GetDeletedVehicles()
        {
            try
            {
                try
                {
                    var query = @"SELECT d.Id AS DeviceId,
                                       d.DeviceTypeId,
                                       dt.Name AS DeviceName,
                                       v.Id,
                                       v.Plate,
                                       v.ActualPlate,
                                       d.CompanyId,
                                       d.Imei,
                                       ISNULL(d.IsLocked, 0) IsLocked,
                                       comp.Name AS Company,
                                       v.Inactive
                                FROM dbo.tbl_Vehicle v
                                    INNER JOIN dbo.tbl_Company comp
                                        ON v.CompanyId = comp.Id
                                    LEFT JOIN dbo.tbl_Device d
                                        ON d.Id = v.DeviceId
                                    LEFT JOIN dbo.tbl_DeviceType dt
                                        ON dt.Id = d.DeviceTypeId
                                WHERE v.Inactive = 1;";

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
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<UserSetting>> GetUserSettings()
        {
            try
            {
                var query = "SELECT * FROM dbo.View_UserSettingES";

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
                var query = @"SELECT * FROM dbo.View_VehicleDeviceES";

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

        public async Task<IEnumerable<VehicleDevice>> GetVehicleDevicesById(int id)
        {
            try
            {
                var query = $"SELECT * FROM dbo.View_VehicleDeviceES WHERE ID = {id}";

                using var connection = _context.CreateConnection();
                connection.Open();

                var userSettings = await connection.QueryAsync<VehicleDevice>(query);
                return userSettings;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
