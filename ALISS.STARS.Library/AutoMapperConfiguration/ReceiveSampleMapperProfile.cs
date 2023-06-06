using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using ALISS.STARS.DTO;
using ALISS.STARS.Library.Models;

namespace ALISS.STARS.Library.AutoMapperConfiguration
{
    public class ReceiveSampleMapperProfile : Profile
    {
        public ReceiveSampleMapperProfile()
        {
            CreateMap<TRStarsResult, ReceiveSampleListsDTO>();
            CreateMap<ReceiveSampleListsDTO, TRStarsResult>();

            CreateMap<TRStarsReceiveSample, ReceiveSampleListsDTO>();
            CreateMap<ReceiveSampleListsDTO, TRStarsReceiveSample>();
        }
    }
}
