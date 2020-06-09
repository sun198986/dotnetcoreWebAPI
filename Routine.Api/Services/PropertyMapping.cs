using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Routine.Api.Services
{
    public class PropertyMapping<TSource,TDestination>:IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> MappingDictionary { get;private set; }

        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = mappingDictionary ?? throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}