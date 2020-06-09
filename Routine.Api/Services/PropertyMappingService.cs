using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Routine.Api.Entities;
using Routine.Api.Models;

namespace Routine.Api.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly Dictionary<string, PropertyMappingValue> _companyPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id",new PropertyMappingValue(new List<string>{"Id"})},
                {"CompanyName",new PropertyMappingValue(new List<string>{"Name"})},
                {"Country",new PropertyMappingValue(new List<string>{"Country"})},
                {"Industry",new PropertyMappingValue(new List<string>{"Industry"})},
                {"Product",new PropertyMappingValue(new List<string>{"Product"})},
                {"Introduction",new PropertyMappingValue(new List<string>{"Introduction"})}
            };

        private readonly Dictionary<string, PropertyMappingValue> _employeePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id",new PropertyMappingValue(new List<string>{"Id"})},
                {"CompanyId",new PropertyMappingValue(new List<string>{"CompanyId"})},
                {"EmployeeNo",new PropertyMappingValue(new List<string>{"EmployeeNo"})},
                {"Name",new PropertyMappingValue(new List<string>{"FirstName","LastName"})},
                {"GenderDisplay",new PropertyMappingValue(new List<string>{"Gender"})},
                {"Age",new PropertyMappingValue(new List<string>{"DateOfBirth"},true)}
            };

        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();


        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<CompanyDto, Company>(_companyPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<EmployeeDto,Employee>(_employeePropertyMapping));
        }
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();
            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }
            throw new Exception($"无法找到唯一的依赖关系:{typeof(TSource)},{typeof(TDestination)}");
        }

    }
}