using DapperASPNetCore.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DapperASPNetCore.Controllers
{
    [Route("api/migration")]
    [ApiController]
    public class MigrationController : ControllerBase
    {
        private readonly IUserSettingRepository _userSettingRepo;

        public MigrationController(IUserSettingRepository companyRepo)
        {
            _userSettingRepo = companyRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserSettings()
        {
            try
            {
                var userSettings = await _userSettingRepo.GetUserSettings();
                return Ok(userSettings);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
    }
}
