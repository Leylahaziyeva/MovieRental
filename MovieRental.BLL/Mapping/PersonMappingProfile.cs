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
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Biography, opt => opt.Ignore())
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