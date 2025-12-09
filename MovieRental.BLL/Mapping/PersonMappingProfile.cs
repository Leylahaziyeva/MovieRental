using AutoMapper;
using MovieRental.BLL.ViewModels.Person;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class PersonMappingProfile : Profile
    {
        public PersonMappingProfile()
        {
            CreateMap<Person, PersonViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.PersonTranslations!.FirstOrDefault() != null
                        ? src.PersonTranslations!.FirstOrDefault()!.Name
                        : "N/A"))
                .ForMember(dest => dest.Biography, opt => opt.MapFrom(src =>
                    src.PersonTranslations!.FirstOrDefault() != null
                        ? src.PersonTranslations!.FirstOrDefault()!.Biography
                        : ""))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl ?? "/img/default-avatar.png"))
                .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.CoverImageUrl))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.KnownCredits, opt => opt.MapFrom(src => src.KnownCredits))
                .ForMember(dest => dest.PlaceOfBirth, opt => opt.MapFrom(src => src.PlaceOfBirth))
                .ForMember(dest => dest.PersonType, opt => opt.MapFrom(src => src.PersonType))
                .ForMember(dest => dest.KnownFor, opt => opt.MapFrom(src => src.KnownFor))
                .ForMember(dest => dest.FacebookUrl, opt => opt.MapFrom(src => src.FacebookUrl))
                .ForMember(dest => dest.TwitterUrl, opt => opt.MapFrom(src => src.TwitterUrl))
                .ForMember(dest => dest.InstagramUrl, opt => opt.MapFrom(src => src.InstagramUrl))
                .ForMember(dest => dest.YoutubeUrl, opt => opt.MapFrom(src => src.YoutubeUrl))
                .ForMember(dest => dest.PersonTypeDisplay, opt => opt.MapFrom(src => src.PersonType.ToString()))
                .ForMember(dest => dest.FormattedDateOfBirth, opt => opt.MapFrom(src =>
                    src.DateOfBirth.HasValue ? src.DateOfBirth.Value.ToString("yyyy-MM-dd") : null));

            CreateMap<PersonCreateViewModel, Person>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.PersonTranslations, opt => opt.Ignore());

            CreateMap<PersonUpdateViewModel, Person>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.PersonTranslations, opt => opt.Ignore());

            CreateMap<PersonTranslation, PersonTranslationViewModel>();

            CreateMap<PersonTranslationCreateViewModel, PersonTranslation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Person, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.Ignore());

            CreateMap<PersonTranslationUpdateViewModel, PersonTranslation>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Person, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.Ignore());
        }
    }
}