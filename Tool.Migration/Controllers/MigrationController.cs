using DapperASPNetCore.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tool.VIP.Migrator.Index;

namespace DapperASPNetCore.Controllers
{
    [Route("api/migration")]
    [ApiController]
    public class MigrationController : ControllerBase
    {
        private readonly IMasterDBRepository _masterDbRepo;
        private readonly ILogger<MigrationController> _logger;
        private readonly IElasticClient _elasticClient;

        public MigrationController(IMasterDBRepository companyRepo, ILogger<MigrationController> logger, IElasticClient elasticClient)
        {
            _masterDbRepo = companyRepo;
            _logger = logger;
            _elasticClient = elasticClient;
        }

        [HttpGet]
        [Route("migrate-user")]
        public async Task<IActionResult> GetUserSettings()
        {
            try
            {
                // Get list user.
                var userSettings = await _masterDbRepo.GetUserSettings();
                Console.WriteLine($"Current active user setting: {userSettings.Count()}");

                await Task.WhenAll(userSettings.Select(async user =>
                {
                    var userIndex = new UserSettingIndex
                    {
                        Id = user.Id,
                        Inactive = true,
                        UserId = user.Id,
                        Username = user.Username,
                        Vehicles = user.Vehicles,
                    };
                    await IndexDocumentAsync(userIndex);
                    Console.WriteLine($"Indexed user id: {user.Id}");
                }));
                return Ok(true);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        private async Task<bool> IndexDocumentAsync(UserSettingIndex document)
        {
            try
            {
                if (document is null)
                {
                    throw new Exception("Document NULL");
                }
                var response = await _elasticClient.IndexDocumentAsync<UserSettingIndex>(document);
                return response.IsValid;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"ElasticSearchService - IndexDocumentAsync - Error: {ex.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("migrate-vehicle")]
        public async Task<IActionResult> GetVehicleDevices()
        {
            try
            {
                // Get list user.
                var vehicleDevices = await _masterDbRepo.GetVehicleDevices();
                Console.WriteLine($"Current total vehicles: {vehicleDevices.Count()}");

                var vehicleGroups = vehicleDevices.GroupBy(v => v.VehicleId);
                var tasks = new List<Task>();
                foreach (var vehicleGroup in vehicleGroups)
                {
                    var vehicleMonitorIndex = new VehicleMonitorIndex
                    {
                        ActualPlate = vehicleGroup.FirstOrDefault().ActualPlate,
                        CompanyName = vehicleGroup.FirstOrDefault().Company,
                        Id = vehicleGroup.FirstOrDefault().VehicleId,
                        Inactive = vehicleGroup.FirstOrDefault().Inactive,
                        Plate = vehicleGroup.FirstOrDefault().Plate,
                        Devices = vehicleGroup.Select(vg => new VehicleDeviceDto
                        {
                            DeviceName = vg.DeviceName,
                            DeviceType = vg.DeviceTypeId,
                            Id = vg.DeviceId,
                            IsLocked = vg.IsLocked,

                        })
                    };
                    await Console.Out.WriteLineAsync($"Indexing vehicle id: {vehicleMonitorIndex.Id} ");

                    tasks.Add(IndexVehicleAsync(vehicleMonitorIndex));

                    await Console.Out.WriteLineAsync($"Index vehicle id: {vehicleMonitorIndex.Id} success");
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }


        private async Task<bool> IndexVehicleAsync(VehicleMonitorIndex document)
        {
            try
            {
                if (document is null)
                {
                    throw new Exception("Document NULL");
                }
                var response = await _elasticClient.IndexDocumentAsync<VehicleMonitorIndex>(document);
                return response.IsValid;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"ElasticSearchService - IndexVehicleAsync - Error: {ex.Message}");
                throw;
            }
        }
    }
}
