using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using ProductGrpc.Models;
using ProductGrpc.Protos;

namespace ProductGrpc.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductModel>()
                .ForMember(dest => dest.CreatedTime,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedTime)))
                .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => (Protos.ProductStatus)src.Status));

            CreateMap<ProductModel, Product>()
                .ForMember(dest => dest.CreatedTime,
                opt => opt.MapFrom(src => src.CreatedTime.ToDateTime()))
                .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => (Models.ProductStatus)src.Status));
        }
    }
}
