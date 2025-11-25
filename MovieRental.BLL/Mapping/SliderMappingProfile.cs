using AutoMapper;
using MovieRental.BLL.ViewModels;
using MovieRental.BLL.ViewModels.Slider;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class SliderMappingProfile : Profile
    {
        public SliderMappingProfile()
        {
            CreateMap<Slider, SliderViewModel>()
                .ForMember(dest => dest.Title, opt => opt.Ignore())
                .ForMember(dest => dest.Subtitle, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.ButtonText, opt => opt.Ignore());

            CreateMap<SliderCreateViewModel, Slider>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.SliderTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.Movie, opt => opt.Ignore());

            CreateMap<SliderUpdateViewModel, Slider>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.SliderTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.Movie, opt => opt.Ignore());

            CreateMap<SliderTranslation, SliderTranslationViewModel>();

            CreateMap<SliderTranslationCreateViewModel, SliderTranslation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SliderId, opt => opt.Ignore())
                .ForMember(dest => dest.Slider, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<SliderTranslationUpdateViewModel, SliderTranslation>()
                .ForMember(dest => dest.SliderId, opt => opt.Ignore())
                .ForMember(dest => dest.Slider, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}