using QuickDotNetCore.Src.Attributrs;
using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickDotNetCore.Src.Models
{
    [AutoMapper(true)]
    [SugarTable("TestData")]
    public class TestDataDO
    {
        [Key]
        public long Id { get; set; }

        public string Des { get; set; }
    }
}
