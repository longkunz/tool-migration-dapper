﻿namespace Tool.VIP.Migrator.Entities
{
    public class VehicleDevice
    {
        public string Company { get; set; }
        public int VehicleId { get; set; }
        public string Plate { get; set; }
        public string ActualPlate { get; set; }
        public int? DeviceId { get; set; }
        public string DeviceName { get; set; }
        public int DeviceTypeId { get; set; }
        public bool? IsLocked { get; set; }
        public bool? Inactive { get; set; }
    }
}