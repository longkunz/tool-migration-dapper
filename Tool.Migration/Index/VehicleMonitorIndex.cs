using System.Collections.Generic;

namespace Tool.VIP.Migrator.Index
{
    public class VehicleMonitorIndex
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Plate { get; set; }
        public string ActualPlate { get; set; }
        public bool? Inactive { get; set; }
        public int? Status { get; set; }
        public IEnumerable<VehicleDeviceDto> Devices { get; set; }
    }

    public class VehicleDeviceDto
    {
        public int? Id { get; set; }
        public int? DeviceType { get; set; }
        public string DeviceName { get; set; }
        public bool? IsLocked { get; set; }
    }
}
