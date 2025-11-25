using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Offer;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;
namespace MovieRental.BLL.Services.Implementations
{
    public class OfferManager : CrudManager<Offer, OfferViewModel, OfferCreateViewModel, OfferUpdateViewModel>, IOfferService
    {
        public OfferManager(IRepositoryAsync<Offer> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
        public async Task<IEnumerable<OfferViewModel>> GetActiveOffersAsync(int languageId)
        {
            var now = DateTime.Now;
            var offers = await Repository.GetAllAsync(
                predicate: o => o.IsActive && o.ValidFrom <= now && o.ValidTo >= now,
                include: query => query
                    .Include(o => o.OfferTranslations.Where(ot => ot.LanguageId == languageId))
                    .Include(o => o.Currency)!,
                orderBy: query => query.OrderByDescending(o => o.CreatedAt),
                AsNoTracking: true
            );
            return Mapper.Map<IEnumerable<OfferViewModel>>(offers);
        }
        public async Task<OfferViewModel?> GetOfferByIdWithTranslationsAsync(int id, int languageId)
        {
            var offer = await Repository.GetAsync(
                predicate: o => o.Id == id,
                include: query => query
                    .Include(o => o.OfferTranslations.Where(ot => ot.LanguageId == languageId))
                    .Include(o => o.Currency)!,
                AsNoTracking: true
            );
            return Mapper.Map<OfferViewModel>(offer);
        }
        public override async Task<OfferViewModel> CreateAsync(OfferCreateViewModel model)
        {
            var offer = Mapper.Map<Offer>(model);
            offer.CreatedAt = DateTime.Now;
            offer.UpdatedAt = DateTime.Now;
            if (model.Translations != null && model.Translations.Any())
            {
                offer.OfferTranslations = model.Translations.Select(t => new OfferTranslation
                {
                    Title = t.Title,
                    Description = t.Description,
                    LanguageId = t.LanguageId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }).ToList();
            }
            var createdOffer = await Repository.AddAsync(offer);
            return Mapper.Map<OfferViewModel>(createdOffer);
        }
        public override async Task<bool> UpdateAsync(int id, OfferUpdateViewModel model)
        {
            var offer = await Repository.GetAsync(
                predicate: o => o.Id == id,
                include: query => query.Include(o => o.OfferTranslations)
            );
            if (offer == null)
                return false;
            Mapper.Map(model, offer);
            offer.UpdatedAt = DateTime.Now;
            if (model.Translations != null)
            {
                offer.OfferTranslations.Clear();
                foreach (var translation in model.Translations)
                {
                    offer.OfferTranslations.Add(new OfferTranslation
                    {
                        Title = translation.Title,
                        Description = translation.Description,
                        LanguageId = translation.LanguageId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }
            }
            await Repository.UpdateAsync(offer);
            return true;
        }
    }
}