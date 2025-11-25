using MovieRental.BLL.ViewModels.Offer;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IOfferService : ICrudService<Offer, OfferViewModel, OfferCreateViewModel, OfferUpdateViewModel>
    {
        Task<IEnumerable<OfferViewModel>> GetActiveOffersAsync(int languageId);
        Task<OfferViewModel?> GetOfferByIdWithTranslationsAsync(int id, int languageId);
    }
}