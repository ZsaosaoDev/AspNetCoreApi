using Asp.NETCoreApi.Data;
using Asp.NETCoreApi.Dto;
using AutoMapper;

namespace Asp.NETCoreApi.Helper {
    public class MappingProfile : Profile {

        public MappingProfile () {

            CreateMap<ProductCategory, ProductCategoryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductCategoryId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description)).ReverseMap();
        }


    }
}
