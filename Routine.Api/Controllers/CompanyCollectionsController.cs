using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Routine.Api.Entities;
using Routine.Api.Helpers;
using Routine.Api.Models;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route("api/companyCollections")]
    public class CompanyCollectionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;

        public CompanyCollectionsController(IMapper mapper, ICompanyRepository companyRepository)
        {
            _mapper = mapper;
            _companyRepository = companyRepository;
        }


        [HttpGet("{ids}", Name = nameof(GetCompanyCollection))]
        public async Task<IActionResult> GetCompanyCollection(
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            var entities = await _companyRepository.GetCompaniesAsync(ids);

            if (ids.Count() != entities.Count())
            {
                return NotFound();
            }

            var returnDtos = _mapper.Map<IEnumerable<CompanyDto>>(entities);
            return Ok(returnDtos);
        }


        [HttpPost]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> CreateCompanyCollection(
            IEnumerable<CompanyAddDto> companyCollection
            )
        {
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);

            foreach (var company in companyEntities)
            {
                _companyRepository.AddCompany(company);

            }
            await _companyRepository.SaveAsync();
            var returnDtos = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var idsString = string.Join(",", returnDtos.Select(x => x.Id));

            return CreatedAtRoute(nameof(GetCompanyCollection), new {ids = idsString}, returnDtos);
        }
    }
}