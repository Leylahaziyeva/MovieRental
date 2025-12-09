using AutoMapper;
using MovieRental.BLL.ViewModels.SportType;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class SportTypeMappingProfile : Profile
    {
        public SportTypeMappingProfile()
        {
            CreateMap<SportType, SportTypeViewModel>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.SportCount, opt => opt.Ignore());

            CreateMap<SportTypeCreateViewModel, SportType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Translations, opt => opt.Ignore())
                .ForMember(dest => dest.Sports, opt => opt.Ignore());

            CreateMap<SportTypeUpdateViewModel, SportType>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Translations, opt => opt.Ignore())
                .ForMember(dest => dest.Sports, opt => opt.Ignore());

            CreateMap<SportTypeTranslation, SportTypeTranslationViewModel>();
        }
    }
}