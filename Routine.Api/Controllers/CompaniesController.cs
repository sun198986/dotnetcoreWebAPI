﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompaniesController(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name = nameof(GetCompanies))]
        [HttpHead]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies([FromQuery] CompanyDtoParameters parameters)
        {
            //throw  new Exception("An Exception");

            var companies = await _companyRepository.GetCompaniesAsync(parameters);

            //var companyDtos = new List<CompanyDto>();

            var previousPageLink = companies.HasPrevious
                ? CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage):null;

            var nextPageLine = companies.HasPrevious
                ? CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage) : null;

            var paginationMeatdata = new
            {
                totalCount = companies.TotalCount,
                pageSize = companies.PageSize,
                currentPage = companies.CurrentPage,
                totalPages = companies.TotalPages,
                previousPageLink,
                nextPageLine
            };
            Response.Headers.Add("X-Pagination",JsonSerializer.Serialize(paginationMeatdata,new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));

            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            return Ok(companyDtos);
        }

        [HttpGet("{companyId}", Name = nameof(GetCompany))]//   api/companies/{companyId}
        public async Task<ActionResult<CompanyDto>> GetCompany([FromRoute] Guid companyId)
        {
            var company = await _companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CompanyDto>(company));
        }

        [HttpPost]
        public async Task<ActionResult<CompanyDto>> CreateCompany(CompanyAddDto company)
        {
            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();
            var returnDto = _mapper.Map<CompanyDto>(entity);
            return CreatedAtRoute(nameof(GetCompany), new { companyId = returnDto.Id }, returnDto);
        }


        [HttpDelete("{companyId}")]
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
                        pageNumber = parameters.PageNumber - 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm

                    });
                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm

                    });
                default:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        pageNumber = parameters.PageNumber,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm

                    });
            }
        }
    }
}