using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Routine.Api.Models;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>
            {
                new LinkDto(Url.Link(nameof(GetRoot), new { }), "self", "GET"),
                new LinkDto(Url.Link(nameof(CompaniesController.GetCompanies), new { }), "companies", "GET"),
                new LinkDto(Url.Link(nameof(CompaniesController.CreateCompany), new { }), "create_companies",
                    "POST")
            };
            return Ok(links);
        }
    }
}