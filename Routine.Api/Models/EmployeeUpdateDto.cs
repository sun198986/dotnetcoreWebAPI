using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Routine.Api.Entities;
using Routine.Api.ValidationAttributes;

namespace Routine.Api.Models
{
    //[EmployeeNoMustDifferentFromFirstName(ErrorMessage = "员工编号必须和名不一样!!!")]
    public class EmployeeUpdateDto : EmployeeAddOrUpdateDto
    {
       
    }
}