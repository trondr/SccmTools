using System;
using System.Collections.Generic;
using AutoMapper;
using Common.Logging;
using SccmTools.Library.Infrastructure.LifeStyles;

namespace SccmTools.Library.Infrastructure
{
    /// <summary>
    /// Source: https://github.com/trondr/NCmdLiner.SolutionCreator/blob/master/src/NCmdLiner.SolutionCreator.Library/BootStrap/TypeMapper.cs
    /// </summary>
    [Singleton]
    public class TypeMapper : ITypeMapper
    {
        private readonly IEnumerable<Profile> _typeMapperProfiles;
        private readonly ILog _logger;
        private IMapper _mapper;
        private object _synch = new object();

        public TypeMapper(IEnumerable<Profile> typeMapperProfiles, ILog logger)
        {
            _typeMapperProfiles = typeMapperProfiles;
            _logger = logger;
        }

        private IMapper Mapper
        {
            get
            {
                if (_mapper == null)
                {
                    lock (_synch)
                    {
                        if (_mapper == null)
                        {
                            _mapper = ConfigureAndCreateMapper();
                        }
                    }
                }
                return _mapper;
            }
        }
        
        public T Map<T>(object source)
        {
            return Mapper.Map<T>(source);            
        }

        private IMapper ConfigureAndCreateMapper()
        {
            var mapperConfiguration = new MapperConfiguration(ConfigureTypeMappers);
            var mapper = mapperConfiguration.CreateMapper();
            ValidateMapper(mapper);
            return mapper;
        }

        private void ValidateMapper(IMapper mapper)
        {
            int numberOfValidationErrors = 0;
            foreach (var typeMap in mapper.ConfigurationProvider.GetAllTypeMaps())
            {
                numberOfValidationErrors += ValidateTypeMap(mapper,typeMap);                                
            }
            if(numberOfValidationErrors > 0)
            {
                throw new Exception("Number of AutoMapper configurations errors: " + numberOfValidationErrors);
            }
        }

        private int ValidateTypeMap(IMapper mapper, TypeMap typeMap)
        {
            try
            {
                mapper.ConfigurationProvider.AssertConfigurationIsValid(typeMap);
            }
            catch (AutoMapperConfigurationException ex)
            {
                _logger.Fatal(ex.Message);
                return 1;
            }
            return 0;
        }

        private void ConfigureTypeMappers(IMapperConfiguration mapperConfiguration)
        {
            foreach (var profile in _typeMapperProfiles)
            {
                mapperConfiguration.AddProfile(profile);
                //((MapperConfiguration)mapperConfiguration).AssertConfigurationIsValid(profile.ProfileName);
            }
        }
    }
}