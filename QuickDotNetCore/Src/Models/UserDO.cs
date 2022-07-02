using QuickDotNetCore.Src.Attributrs;
using SqlSugar;

namespace QuickDotNetCore.Src.Models
{
    [AutoMapper]
    [SugarTable("User")]
    public class UserDO
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }
    }
}
