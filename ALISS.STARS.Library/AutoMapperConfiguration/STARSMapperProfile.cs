using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using ALISS.STARS.DTO;
using ALISS.STARS.Library.Models;

namespace ALISS.STARS.Library.AutoMapperConfiguration
{
    public class STARSMapperProfile : Profile
    {
        public STARSMapperProfile()
        {
            CreateMap<TRSTARSMapping, STARSMappingDataDTO>();
            CreateMap<STARSMappingDataDTO, TRSTARSMapping>();

            CreateMap<TRSTARSWHONetMapping, STARSWHONetMappingDataDTO>();
            CreateMap<STARSWHONetMappingDataDTO, TRSTARSWHONetMapping>();

            CreateMap<TRSTARSSpecimenMapping, STARSSpecimenMappingDataDTO>();
            CreateMap<STARSSpecimenMappingDataDTO, TRSTARSSpecimenMapping>();

            CreateMap<TRSTARSOrganismMapping, STARSOrganismMappingDataDTO>();
            CreateMap<STARSOrganismMappingDataDTO, TRSTARSOrganismMapping>();
        }
    }
}
