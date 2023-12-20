namespace DapperASPNetCore.Entities
{
    public class UserSetting
    {
        public int Id { get; set; }
        public string Vehicles { get; set; }
        public string Username { get; set; }
        public int? CompanyId { get; set; }
        public string Email { get; set; }
        public string RoleIdsJson { get; set; }
        public bool Inactive { get; set; }
    }
}
