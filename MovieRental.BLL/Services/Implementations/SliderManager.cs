using AutoMapper;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels;
using MovieRental.BLL.ViewModels.Slider;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;

namespace MovieRental.BLL.Services.Implementations
{
    public class SliderManager : CrudManager<Slider, SliderViewModel, SliderCreateViewModel, SliderUpdateViewModel>, ISliderService
    {
        private readonly ISliderRepository _repository;
        private readonly IMapper _mapper;

        public SliderManager(ISliderRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<SliderViewModel>> GetAllActiveSlidersAsync(int languageId)
        {
            var sliders = await _repository.GetAllActiveSlidersAsync();

            return sliders
                .Select(slider => MapSliderWithTranslation(slider, languageId))
                .ToList();
        }

        public async Task<SliderViewModel?> GetSliderByIdAsync(int id, int languageId)
        {
            var slider = await _repository.GetSliderWithTranslationsAsync(id);
            if (slider == null)
                return null;

            return MapSliderWithTranslation(slider, languageId);
        }

        private SliderViewModel MapSliderWithTranslation(Slider slider, int languageId)
        {
            var translation = slider.SliderTranslations
                .FirstOrDefault(t => t.LanguageId == languageId);

            return new SliderViewModel
            {
                Id = slider.Id,
                ImageUrl = slider.ImageUrl,
                MovieId = slider.MovieId,
                ActionUrl = slider.ActionUrl,
                DisplayOrder = slider.DisplayOrder,
                IsActive = slider.IsActive,
                Title = translation?.Title,
                Subtitle = translation?.Subtitle,
                Description = translation?.Description,
                ButtonText = translation?.ButtonText ?? "Book Now"
            };
        }
    }
}