using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Routine.Api.Models
{
    public class CompanyAddDto
    {
        [Display(Name = "公司名")]
        [Required(ErrorMessage = "{0}是必必填的")]
        [MaxLength(100, ErrorMessage = "{0}最大长度不能超过{1}")]
        public string Name { get; set; }

        [Display(Name = "简介")]
        [MinLength(10)]
        [StringLength(500,MinimumLength = 10,ErrorMessage = "{0}最大长度从{2}到{1}")]
        public string Introduction { get; set; }

        public ICollection<EmployeeAddDto> Employees { get; set; }=new List<EmployeeAddDto>();
    }
}