namespace Tool.VIP.Migrator.Index
{
    public class UserSettingIndex
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Vehicles { get; set; }
        public int? CompanyId { get; set; }
        public string Email { get; set; }
        public string RoleIdsJson { get; set; }
        public bool Inactive { get; set; }
    }
}
