using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SqlTypes;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Routine.Api.ActionConstraints;
using Routine.Api.DtoParameters;
using Routine.Api.Entities;
using Routine.Api.Helpers;
using Routine.Api.Models;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route("api/companies")]
    //[Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyChckerService _propertyChckerService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompaniesController(ICompanyRepository companyRepository, IMapper mapper,IPropertyMappingService propertyMappingService,IPropertyChckerService propertyChckerService)
        {
            _propertyMappingService = propertyMappingService?? throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyChckerService = propertyChckerService?? throw new ArgumentNullException(nameof(_propertyChckerService));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name = nameof(GetCompanies))]
        [HttpHead]
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyDtoParameters parameters,[FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            //throw  new Exception("An Exception");
            if (!_propertyMappingService.ValidMappingExistsFor<CompanyDto, Company>(parameters.OrderBy))
            {
                return  BadRequest();
            }

            if (!_propertyChckerService.TypeHasProperties<CompanyDto>(parameters.Fields))
            {
                return BadRequest();
            }


            var companies = await _companyRepository.GetCompaniesAsync(parameters);

            //var companyDtos = new List<CompanyDto>();

            //var previousPageLink = companies.HasPrevious
            //    ? CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage):null;

            //var nextPageLine = companies.HasNext
            //    ? CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage) : null;

            var paginationMeatdata = new
            {
                totalCount = companies.TotalCount,
                pageSize = companies.PageSize,
                currentPage = companies.CurrentPage,
                totalPages = companies.TotalPages,
            };
            Response.Headers.Add("X-Pagination",JsonSerializer.Serialize(paginationMeatdata,new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            if (parsedMediaType.MediaType == "application/vnd.company.hateoas+json")
            {
                
                var shapdData = companyDtos.ShapeData(parameters.Fields);

                var links = CreateLinksForCompany(parameters, companies.HasPrevious, companies.HasNext);

                var shapedCompaniesWithLinks = shapdData.Select(c =>
                    {
                        var companyDict = c as IDictionary<string, object>;
                        var companyLinks = CreateLinksForCompany((Guid) companyDict["Id"], null);
                        companyDict.Add("links", companyLinks);
                        return companyDict;

                    }
                );

                var linkedCollectionResource = new
                {
                    values = shapedCompaniesWithLinks,
                    links
                };
                return Ok(linkedCollectionResource);
            }
            else
            {
                return Ok(companyDtos.ShapeData(parameters.Fields));
            }
        }
        [Produces("application/json", "application/vnd.company.hateoas+json",
            "application/vnd.company.company.full+json",
            "application/vnd.company.company.full.hateoas+json",
            "application/vnd.company.company.friendly+json",
            "application/vnd.company.company.friendly.hateoas+json"
            )]
        [HttpGet("{companyId}", Name = nameof(GetCompany))]//   api/companies/{companyId}
        public async Task<ActionResult<CompanyDto>> GetCompany([FromRoute] Guid companyId,[FromQuery]string fields,[FromHeader(Name = "Accept")] string mediaType )
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyChckerService.TypeHasProperties<CompanyDto>(fields))
            {
                return BadRequest();
            }
            var company = await _companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas",StringComparison.InvariantCultureIgnoreCase);
            IEnumerable<LinkDto> myLinks = new List<LinkDto>();
            if (includeLinks)
            {
                myLinks = CreateLinksForCompany(companyId, fields);
            }

            var primaryMediaType = includeLinks
                ? parsedMediaType.SubTypeWithoutSuffix.Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                : parsedMediaType.SubTypeWithoutSuffix;

            if (primaryMediaType == "vnd.company.company.full")
            {
                var full = _mapper.Map<CompanyFullDto>(company).shapeData(fields) as IDictionary<string,object>;
                if (includeLinks)
                {
                    full.Add("links",myLinks);
                }

                return Ok(full);
            }

            var friendly = _mapper.Map<CompanyDto>(company).shapeData(fields) as IDictionary<string, object>;
            if (includeLinks)
            {
                friendly.Add("links",myLinks);
                return Ok(friendly);
            }


            //if (parsedMediaType.MediaType == "application/vnd.company.hateoas+json")
            //{
            //    var links = CreateLinksForCompany(companyId, fields);

            //    var linkedDict = _mapper.Map<CompanyDto>(company).shapeData(fields) as IDictionary<string, object>;

            //    linkedDict.Add("links", links);
            //    return Ok(linkedDict);
            //}

            return Ok(_mapper.Map<CompanyDto>(company).shapeData(fields));
        }

        [HttpPost(Name = nameof(CreateCompanyWithBankruptTime))]
        [RequestHeaderMatchesMediaType("Content-Type", "application/vnd.company.company.companyforcreationwithbankrupttime+json")]
        [Consumes("application/vnd.company.company.companyforcreationwithbankrupttime+json")]
        public async Task<ActionResult<CompanyDto>> CreateCompanyWithBankruptTime(CompanyAddWithBankruptTimeDto company)
        {
            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();
            var returnDto = _mapper.Map<CompanyDto>(entity);

            var links = CreateLinksForCompany(returnDto.Id, null);

            var linkedDict = returnDto.shapeData(null) as IDictionary<string, object>;
            linkedDict.Add("link", links);

            return CreatedAtRoute(nameof(GetCompany), new { companyId = linkedDict["Id"] }, linkedDict);
        }

        [HttpPost(Name = nameof(CreateCompany))]
        [RequestHeaderMatchesMediaType("Content-Type","application/json","application/vnd.company.company.companyforcreation+json")]
        [Consumes("application/json", "application/vnd.company.company.companyforcreation+json")]
        public async Task<ActionResult<CompanyDto>> CreateCompany(CompanyAddDto company)
        {
            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();
            var returnDto = _mapper.Map<CompanyDto>(entity);

            var links = CreateLinksForCompany(returnDto.Id, null);

            var linkedDict = returnDto.shapeData(null) as IDictionary<string, object>;
            linkedDict.Add("link", links);

            return CreatedAtRoute(nameof(GetCompany), new { companyId = linkedDict["Id"] }, linkedDict);
        }


        [HttpDelete("{companyId}",Name = nameof(DeleteCompany))]
        public async Task<IActionResult> DeleteCompany(Guid companyId)
        {
            var company = await _companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return NotFound();
            }

            await _companyRepository.GetEmployeesAsync(companyId, null);

            _companyRepository.DeleteCompany(company);
            await _companyRepository.SaveAsync();
            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET,POST,OPTIONS");
            return Ok();
        }

        private string CreateCompaniesResourceUri(CompanyDtoParameters parameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        oderby = parameters.OrderBy,
                        pageNumber = parameters.PageNumber - 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm

                    });
                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        oderby = parameters.OrderBy,
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm

                    });
                case ResourceUriType.CurrentPage:
                    default:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        oderby = parameters.OrderBy,
                        pageNumber = parameters.PageNumber,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm

                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(Guid companyId,string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(
                        Url.Link(nameof(GetCompany),new {companyId}),
                        "self",
                        "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(
                        Url.Link(nameof(GetCompany), new { companyId,fields }),
                        "self",
                        "GET"));
            }

            links.Add(
                new LinkDto(
                    Url.Link(nameof(DeleteCompany), new { companyId}),
                    "delete_company",
                    "GET"));


            links.Add(
                new LinkDto(
                    Url.Link(nameof(EmployeesController.CreateEmployeeForCompany), new { companyId }),
                    "create_employee_for_company",
                    "POST"));

            links.Add(
                new LinkDto(
                    Url.Link(nameof(EmployeesController.GetEmployeesForCompany), new { companyId }),
                    "employees",
                    "GET"));
            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(CompanyDtoParameters parameters,bool hasPrevious,bool hasNext)
        {
            var links = new List<LinkDto>();
            links.Add(new LinkDto(CreateCompaniesResourceUri(parameters,ResourceUriType.CurrentPage),"self","GET"));

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage), "previous_page", "GET"));
            }
            if (hasNext)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage), "next_page", "GET"));
            }

            return links;
        }
    }
}