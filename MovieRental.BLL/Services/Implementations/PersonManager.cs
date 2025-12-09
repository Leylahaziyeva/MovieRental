using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Person;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;
using System.Linq.Expressions;

namespace MovieRental.BLL.Services.Implementations
{
    public class PersonManager : CrudManager<Person, PersonViewModel, PersonCreateViewModel, PersonUpdateViewModel>, IPersonService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ICookieService _cookieService;

        public PersonManager(
            IRepositoryAsync<Person> repository,
            IMapper mapper,
            ICloudinaryService cloudinaryService,
            ICookieService cookieService) : base(repository, mapper)
        {
            _cloudinaryService = cloudinaryService;
            _cookieService = cookieService;
        }

        public override async Task<PersonViewModel?> GetByIdAsync(int id)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            var person = await Repository.GetAsync(
                predicate: x => x.Id == id,
                include: query => query
                    .Include(p => p.PersonTranslations!.Where(pt => pt.LanguageId == languageId)),
                AsNoTracking: true
            );

            if (person == null) return null;

            var viewModel = Mapper.Map<PersonViewModel>(person);

            var translation = person.PersonTranslations?.FirstOrDefault();
            if (translation != null)
            {
                viewModel.Name = translation.Name;
                viewModel.Biography = translation.Biography;
            }

            return viewModel;
        }

        public override async Task<IEnumerable<PersonViewModel>> GetAllAsync(
            Expression<Func<Person, bool>>? predicate = null,
            Func<IQueryable<Person>, IOrderedQueryable<Person>>? orderBy = null,
            Func<IQueryable<Person>, IIncludableQueryable<Person, object>>? include = null,
            bool AsNoTracking = false)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            Func<IQueryable<Person>, IIncludableQueryable<Person, object>> includeWithTranslations = query =>
            {
                var included = query.Include(p => p.PersonTranslations!.Where(pt => pt.LanguageId == languageId));
                return include != null ? include(included) : included;
            };

            var persons = await Repository.GetAllAsync(predicate, orderBy, includeWithTranslations, AsNoTracking);
            var viewModels = new List<PersonViewModel>();

            foreach (var person in persons)
            {
                var viewModel = Mapper.Map<PersonViewModel>(person);

                var translation = person.PersonTranslations?.FirstOrDefault();
                if (translation != null)
                {
                    viewModel.Name = translation.Name;
                    viewModel.Biography = translation.Biography;
                }

                viewModels.Add(viewModel);
            }

            return viewModels;
        }

        public override async Task<PersonViewModel> CreateAsync(PersonCreateViewModel createViewModel)
        {
            if (createViewModel.ProfileImageFile != null)
            {
                createViewModel.ProfileImageUrl = await _cloudinaryService.ImageCreateAsync(createViewModel.ProfileImageFile);
            }

            if (createViewModel.CoverImageFile != null)
            {
                createViewModel.CoverImageUrl = await _cloudinaryService.ImageCreateAsync(createViewModel.CoverImageFile);
            }

            return await base.CreateAsync(createViewModel);
        }

        public override async Task<bool> UpdateAsync(int id, PersonUpdateViewModel model)
        {
            var existingPerson = await Repository.GetByIdAsync(id);
            if (existingPerson == null) return false;

            string oldProfileImageUrl = existingPerson.ProfileImageUrl;
            string oldCoverImageUrl = existingPerson.CoverImageUrl;

            if (model.ProfileImageFile != null)
            {
                model.ProfileImageUrl = await _cloudinaryService.ImageCreateAsync(model.ProfileImageFile);

                if (!string.IsNullOrEmpty(oldProfileImageUrl))
                {
                    await _cloudinaryService.ImageDeleteAsync(oldProfileImageUrl);
                }
            }
            else
            {
                model.ProfileImageUrl = oldProfileImageUrl;
            }

            if (model.CoverImageFile != null)
            {
                model.CoverImageUrl = await _cloudinaryService.ImageCreateAsync(model.CoverImageFile);

                if (!string.IsNullOrEmpty(oldCoverImageUrl))
                {
                    await _cloudinaryService.ImageDeleteAsync(oldCoverImageUrl);
                }
            }
            else
            {
                model.CoverImageUrl = oldCoverImageUrl;
            }

            return await base.UpdateAsync(id, model);
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var person = await Repository.GetByIdAsync(id);
            if (person == null) return false;

            if (!string.IsNullOrEmpty(person.ProfileImageUrl))
            {
                await _cloudinaryService.ImageDeleteAsync(person.ProfileImageUrl);
            }

            if (!string.IsNullOrEmpty(person.CoverImageUrl))
            {
                await _cloudinaryService.ImageDeleteAsync(person.CoverImageUrl);
            }

            return await base.DeleteAsync(id);
        }

        public async Task<IEnumerable<PersonViewModel>> GetByPersonTypeAsync(PersonType personType)
        {
            return await GetAllAsync(
                predicate: x => x.PersonType == personType,
                orderBy: query => query.OrderByDescending(x => x.KnownCredits),
                AsNoTracking: true
            );
        }

        public async Task<IEnumerable<PersonViewModel>> GetTopPersonsAsync(int count = 10)
        {
            var allPersons = await GetAllAsync(
                orderBy: query => query.OrderByDescending(x => x.KnownCredits),
                AsNoTracking: true
            );

            return allPersons.Take(count);
        }
        public async Task<IEnumerable<PersonViewModel>> GetSportsmenAsync()
        {
            return await GetByPersonTypeAsync(PersonType.Sportsman);
        }
        public async Task<IEnumerable<PersonViewModel>> GetActorsAsync()
        {
            return await GetByPersonTypeAsync(PersonType.Actor);
        }

        public async Task<IEnumerable<PersonViewModel>> GetArtistsAsync()
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            var artists = await Repository.GetAllAsync(
                predicate: p => p.PersonType == PersonType.Artist && !p.IsDeleted,
                include: query => query
                    .Include(p => p.PersonTranslations!.Where(pt => pt.LanguageId == languageId)),
                AsNoTracking: true
            );

            return Mapper.Map<IEnumerable<PersonViewModel>>(artists);
        }
    }
}