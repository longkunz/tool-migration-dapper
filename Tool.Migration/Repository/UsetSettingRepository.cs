using Dapper;
using DapperASPNetCore.Context;
using DapperASPNetCore.Contracts;
using DapperASPNetCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DapperASPNetCore.Repository
{
    public class UsetSettingRepository : IUserSettingRepository
    {
        private readonly DapperContext _context;

        public UsetSettingRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserSetting>> GetUserSettings()
        {
            try
            {
                var query = "SELECT us.Id,us.Vehicles,u.Username,us.Inactive FROM dbo.tbl_UserSetting us (NOLOCK) JOIN tbl_User u (NOLOCK) ON us.Id = u.Id WHERE u.Inactive = 0 AND us.Inactive = 0;";

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
    }
}
