using AutoMapper;
using MovieRental.BLL.ViewModels.Currency;
using MovieRental.BLL.ViewModels.Language;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LanguageViewModel, Language>().ReverseMap();
            CreateMap<CurrencyViewModel, Currency>().ReverseMap();
        }
    }
}