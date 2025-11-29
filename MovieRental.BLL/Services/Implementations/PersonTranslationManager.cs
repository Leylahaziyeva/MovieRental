using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Person;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;

namespace MovieRental.BLL.Services.Implementations
{
    public class PersonTranslationManager : CrudManager<PersonTranslation, PersonTranslationViewModel, PersonTranslationCreateViewModel, PersonTranslationUpdateViewModel>, IPersonTranslationService
    {
        public PersonTranslationManager(IRepositoryAsync<PersonTranslation> repository, IMapper mapper)
            : base(repository, mapper) { }

        public async Task<IEnumerable<PersonTranslationViewModel>> GetAllByPersonIdAsync(int personId)
        {
            return await GetAllAsync(
                predicate: pt => pt.PersonId == personId,
                include: query => query
                    .Include(pt => pt.Language!)
                    .Include(pt => pt.Person!),
                AsNoTracking: true
            );
        }

        public async Task<PersonTranslationViewModel?> GetByPersonAndLanguageAsync(int personId, int languageId)
        {
            var translations = await GetAllAsync(
                predicate: pt => pt.PersonId == personId && pt.LanguageId == languageId,
                include: query => query
                    .Include(pt => pt.Language!)
                    .Include(pt => pt.Person!),
                AsNoTracking: true
            );

            return translations.FirstOrDefault();
        }

        public async Task<bool> ExistsAsync(int personId, int languageId)
        {
            var translation = await Repository.GetAsync(
                predicate: pt => pt.PersonId == personId && pt.LanguageId == languageId
            );

            return translation != null;
        }

        public override async Task<PersonTranslationViewModel> CreateAsync(PersonTranslationCreateViewModel model)
        {
            var exists = await ExistsAsync(model.PersonId, model.LanguageId);
            if (exists)
            {
                throw new InvalidOperationException("Translation for this language already exists.");
            }

            return await base.CreateAsync(model);
        }

        public override async Task<bool> UpdateAsync(int id, PersonTranslationUpdateViewModel model)
        {
            var existingTranslation = await Repository.GetByIdAsync(id);
            if (existingTranslation == null)
                return false;

            if (existingTranslation.LanguageId != model.LanguageId)
            {
                var duplicate = await ExistsAsync(model.PersonId, model.LanguageId);
                if (duplicate)
                {
                    throw new InvalidOperationException($"Bu şəxs üçün {model.LanguageId} dilində tərcümə artıq mövcuddur.");
                }
            }

            return await base.UpdateAsync(id, model);
        }
    }
}