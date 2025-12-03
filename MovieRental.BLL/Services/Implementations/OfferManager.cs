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
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ICookieService _cookieService;

        public OfferManager(IRepositoryAsync<Offer> repository, IMapper mapper, ICloudinaryService cloudinaryService, ICookieService cookieService)
            : base(repository, mapper)
        {
            _cloudinaryService = cloudinaryService;
            _cookieService = cookieService;
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

        public async Task<IEnumerable<OfferViewModel>> GetAllOffersAsync(int languageId)
        {
            var offers = await Repository.GetAllAsync(
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
            var languageId = await _cookieService.GetLanguageIdAsync();

            var imageUrl = await _cloudinaryService.ImageCreateAsync(model.ImageFile);

            string? logoUrl = null;
            if (model.LogoFile != null)
            {
                logoUrl = await _cloudinaryService.ImageCreateAsync(model.LogoFile);
            }

            var offer = new Offer
            {
                ImageUrl = imageUrl,
                LogoUrl = logoUrl!,
                ValidFrom = model.ValidFrom,
                ValidTo = model.ValidTo,
                DiscountPercentage = model.DiscountPercentage,
                DiscountAmount = model.DiscountAmount,
                CurrencyId = model.CurrencyId,
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                OfferTranslations = new List<OfferTranslation>
                {
                    new OfferTranslation
                    {
                        Title = model.Title,
                        Description = model.Description,
                        LanguageId = languageId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                }
            };

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

            var languageId = await _cookieService.GetLanguageIdAsync();

            if (model.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(offer.ImageUrl))
                {
                    await _cloudinaryService.ImageDeleteAsync(offer.ImageUrl);
                }
                offer.ImageUrl = await _cloudinaryService.ImageCreateAsync(model.ImageFile);
            }

            if (model.LogoFile != null)
            {
                if (!string.IsNullOrEmpty(offer.LogoUrl))
                {
                    await _cloudinaryService.ImageDeleteAsync(offer.LogoUrl);
                }
                offer.LogoUrl = await _cloudinaryService.ImageCreateAsync(model.LogoFile);
            }

            offer.ValidFrom = model.ValidFrom;
            offer.ValidTo = model.ValidTo;
            offer.DiscountPercentage = model.DiscountPercentage;
            offer.DiscountAmount = model.DiscountAmount;
            offer.CurrencyId = model.CurrencyId;
            offer.IsActive = model.IsActive;
            offer.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrEmpty(model.Title) && !string.IsNullOrEmpty(model.Description))
            {
                var existingTranslation = offer.OfferTranslations
                    .FirstOrDefault(t => t.LanguageId == languageId);

                if (existingTranslation != null)
                {
                    existingTranslation.Title = model.Title;
                    existingTranslation.Description = model.Description;
                    existingTranslation.UpdatedAt = DateTime.Now;
                }
                else
                {
                    offer.OfferTranslations.Add(new OfferTranslation
                    {
                        Title = model.Title,
                        Description = model.Description,
                        LanguageId = languageId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }
            }

            await Repository.UpdateAsync(offer);
            return true;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var offer = await Repository.GetByIdAsync(id);
            if (offer == null)
                return false;

            if (!string.IsNullOrEmpty(offer.ImageUrl))
            {
                await _cloudinaryService.ImageDeleteAsync(offer.ImageUrl);
            }

            if (!string.IsNullOrEmpty(offer.LogoUrl))
            {
                await _cloudinaryService.ImageDeleteAsync(offer.LogoUrl);
            }

            await Repository.DeleteAsync(offer);
            return true;
        }

        public async Task<bool> IsOfferValidAsync(int id)
        {
            var offer = await Repository.GetByIdAsync(id);
            if (offer == null)
                return false;

            var now = DateTime.Now;
            return offer.IsActive && offer.ValidFrom <= now && offer.ValidTo >= now;
        }
    }
}