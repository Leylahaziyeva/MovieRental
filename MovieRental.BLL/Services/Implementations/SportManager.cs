using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Sport;
using MovieRental.BLL.ViewModels.Person;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;
using System.Linq.Expressions;

namespace MovieRental.BLL.Services.Implementations
{
    public class SportManager : CrudManager<Sport, SportViewModel, SportCreateViewModel, SportUpdateViewModel>, ISportService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ICookieService _cookieService;
        private readonly ICurrencyService _currencyService;
        private readonly IRepositoryAsync<Person> _personRepository;
        private readonly IPersonService _personService;

        public SportManager(
            IRepositoryAsync<Sport> repository,
            IMapper mapper,
            ICloudinaryService cloudinaryService,
            ICookieService cookieService,
            ICurrencyService currencyService,
            IRepositoryAsync<Person> personRepository,
            IPersonService personService) 
            : base(repository, mapper)
        {
            _cloudinaryService = cloudinaryService;
            _cookieService = cookieService;
            _currencyService = currencyService;
            _personRepository = personRepository;
            _personService = personService; 
        }

        public override async Task<SportViewModel?> GetByIdAsync(int id)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            var sport = await Repository.GetAsync(
                predicate: x => x.Id == id,
                include: query => query
                    .Include(s => s.SportTranslations.Where(st => st.LanguageId == languageId))
                    .Include(s => s.Currency)
                    .Include(s => s.Players)
                        .ThenInclude(p => p.PersonTranslations!.Where(pt => pt.LanguageId == languageId)),
                AsNoTracking: true
            );

            if (sport == null) return null;

            var viewModel = Mapper.Map<SportViewModel>(sport);

            var translation = sport.SportTranslations?.FirstOrDefault();
            if (translation != null)
            {
                viewModel.Name = translation.Name;
                viewModel.Description = translation.Description;
                viewModel.Location = translation.Location;
            }
            else
            {
                viewModel.Name = $"[No translation for language {languageId}]";
                viewModel.Description = string.Empty;
                viewModel.Location = string.Empty;
            }

            if (sport.Players?.Any() == true)
            {
                viewModel.Players = new List<PersonViewModel>();
                foreach (var player in sport.Players)
                {
                    var personViewModel = Mapper.Map<PersonViewModel>(player);

                    var personTranslation = player.PersonTranslations?.FirstOrDefault();
                    if (personTranslation != null)
                    {
                        personViewModel.Name = personTranslation.Name;
                        personViewModel.Biography = personTranslation.Biography;
                    }

                    viewModel.Players.Add(personViewModel);
                }
            }

            if (sport.Price.HasValue && sport.Currency != null)
            {
                viewModel.FormattedPrice = $"{sport.Currency.Symbol}{sport.Price.Value:N2}";
            }

            return viewModel;
        }

        public override async Task<IEnumerable<SportViewModel>> GetAllAsync(
            Expression<Func<Sport, bool>>? predicate = null,
            Func<IQueryable<Sport>, IOrderedQueryable<Sport>>? orderBy = null,
            Func<IQueryable<Sport>, IIncludableQueryable<Sport, object>>? include = null,
            bool AsNoTracking = false)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            Func<IQueryable<Sport>, IIncludableQueryable<Sport, object>> includeWithTranslations = query =>
            {
                var included = query
                    .Include(s => s.SportTranslations.Where(st => st.LanguageId == languageId))
                    .Include(s => s.Currency)
                    .Include(s => s.Players)
                        .ThenInclude(p => p.PersonTranslations!.Where(pt => pt.LanguageId == languageId));

                return include != null ? include(included) : included;
            };

            var sports = await Repository.GetAllAsync(predicate, orderBy, includeWithTranslations, AsNoTracking);
            return await MapToViewModelsAsync(sports.ToList(), languageId);
        }

        private Task<IEnumerable<SportViewModel>> MapToViewModelsAsync(IList<Sport> sports, int languageId)
        {
            var viewModels = new List<SportViewModel>();

            foreach (var sport in sports)
            {
                var viewModel = Mapper.Map<SportViewModel>(sport);

                var translation = sport.SportTranslations?.FirstOrDefault();
                if (translation != null)
                {
                    viewModel.Name = translation.Name;
                    viewModel.Description = translation.Description;
                    viewModel.Location = translation.Location;
                }

                if (sport.Players?.Any() == true)
                {
                    viewModel.Players = new List<PersonViewModel>();
                    foreach (var player in sport.Players)
                    {
                        var personViewModel = Mapper.Map<PersonViewModel>(player);
                        var personTranslation = player.PersonTranslations?.FirstOrDefault();
                        if (personTranslation != null)
                        {
                            personViewModel.Name = personTranslation.Name;
                            personViewModel.Biography = personTranslation.Biography;
                        }
                        viewModel.Players.Add(personViewModel);
                    }
                }

                if (sport.Price.HasValue && sport.Currency != null)
                {
                    viewModel.FormattedPrice = $"{sport.Currency.Symbol}{sport.Price.Value:N2}";
                }

                viewModels.Add(viewModel);
            }

            return Task.FromResult<IEnumerable<SportViewModel>>(viewModels);
        }

        public async Task<IEnumerable<SportViewModel>> GetSportsByLocationAsync(string location)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            var sports = await Repository.GetAllAsync(
                predicate: x => x.IsActive && x.SportTranslations.Any(st =>
                    st.LanguageId == languageId &&
                    st.Location.Contains(location)),
                orderBy: query => query.OrderBy(x => x.EventDate),
                include: query => query
                    .Include(s => s.SportTranslations.Where(st => st.LanguageId == languageId))
                    .Include(s => s.Currency)
                    .Include(s => s.Players)
                        .ThenInclude(p => p.PersonTranslations!.Where(pt => pt.LanguageId == languageId)),
                AsNoTracking: true
            );

            return await MapToViewModelsAsync(sports.ToList(), languageId);
        }

        public override async Task<SportViewModel> CreateAsync(SportCreateViewModel createViewModel)
        {
            if (createViewModel.ImageFile != null)
            {
                createViewModel.ImageUrl = await _cloudinaryService.ImageCreateAsync(createViewModel.ImageFile);
            }

            if (createViewModel.CoverImageFile != null)
            {
                createViewModel.CoverImageUrl = await _cloudinaryService.ImageCreateAsync(createViewModel.CoverImageFile);
            }

            return await base.CreateAsync(createViewModel);
        }

        public override async Task<bool> UpdateAsync(int id, SportUpdateViewModel model)
        {
            var existingSport = await Repository.GetByIdAsync(id);
            if (existingSport == null) return false;

            string oldImageUrl = existingSport.ImageUrl ?? string.Empty;
            string oldCoverImageUrl = existingSport.CoverImageUrl ?? string.Empty;

            if (model.ImageFile != null)
            {
                model.ImageUrl = await _cloudinaryService.ImageCreateAsync(model.ImageFile);

                if (!string.IsNullOrEmpty(oldImageUrl))
                {
                    await _cloudinaryService.ImageDeleteAsync(oldImageUrl);
                }
            }
            else
            {
                model.ImageUrl = oldImageUrl;
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
            var sport = await Repository.GetByIdAsync(id);
            if (sport == null) return false;

            if (!string.IsNullOrEmpty(sport.ImageUrl))
            {
                await _cloudinaryService.ImageDeleteAsync(sport.ImageUrl);
            }

            if (!string.IsNullOrEmpty(sport.CoverImageUrl))
            {
                await _cloudinaryService.ImageDeleteAsync(sport.CoverImageUrl);
            }

            return await base.DeleteAsync(id);
        }

        public async Task<IEnumerable<SportViewModel>> GetFeaturedSportsAsync(int count = 4)
        {
            var sports = await GetAllAsync(
                predicate: x => x.IsFeatured && x.IsActive && x.EventDate > DateTime.Now,
                orderBy: query => query.OrderBy(x => x.EventDate),
                AsNoTracking: true
            );

            return sports.Take(count);
        }

        public async Task<IEnumerable<SportViewModel>> GetUpcomingSportsAsync()
        {
            return await GetAllAsync(
                predicate: x => x.IsActive && x.EventDate > DateTime.Now,
                orderBy: query => query.OrderBy(x => x.EventDate),
                AsNoTracking: true
            );
        }

        public async Task<IEnumerable<SportViewModel>> GetPastSportsAsync()
        {
            return await GetAllAsync(
                predicate: x => x.EventDate < DateTime.Now,
                orderBy: query => query.OrderByDescending(x => x.EventDate),
                AsNoTracking: true
            );
        }

        public async Task<IEnumerable<SportViewModel>> GetSportsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await GetAllAsync(
                predicate: x => x.IsActive && x.EventDate >= startDate && x.EventDate <= endDate,
                orderBy: query => query.OrderBy(x => x.EventDate),
                AsNoTracking: true
            );
        }

        public async Task<IEnumerable<SportViewModel>> GetSportsByCategoryAsync(string category)
        {
            return await GetAllAsync(
                predicate: x => x.IsActive && x.Categories != null && x.Categories.Contains(category),
                orderBy: query => query.OrderBy(x => x.EventDate),
                AsNoTracking: true
            );
        }

        public async Task<SportViewModel?> GetSportDetailAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<bool> AddPlayersToSportAsync(int sportId, List<int> playerIds)
        {
            var sport = await Repository.GetAsync(
                predicate: x => x.Id == sportId,
                include: query => query.Include(s => s.Players)
            );

            if (sport == null) return false;

            foreach (var playerId in playerIds)
            {
                var player = await _personRepository.GetByIdAsync(playerId);
                if (player != null && !sport.Players.Any(p => p.Id == playerId))
                {
                    sport.Players.Add(player);
                }
            }

            await Repository.UpdateAsync(sport);
            return true;
        }

        public async Task<bool> RemovePlayerFromSportAsync(int sportId, int playerId)
        {
            var sport = await Repository.GetAsync(
                predicate: x => x.Id == sportId,
                include: query => query.Include(s => s.Players)
            );

            if (sport == null) return false;

            var player = sport.Players.FirstOrDefault(p => p.Id == playerId);
            if (player != null)
            {
                sport.Players.Remove(player);
                await Repository.UpdateAsync(sport);
                return true;
            }

            return false;
        }

        public async Task<(IEnumerable<SportViewModel> Sports, int TotalCount)> GetSportsPagedAsync(int page = 1, int pageSize = 12, string? location = null, string? category = null)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            Expression<Func<Sport, bool>> predicate = x => x.IsActive && x.EventDate > DateTime.Now;

            if (!string.IsNullOrEmpty(location))
            {
                predicate = predicate.And(x => x.SportTranslations.Any(st =>
                    st.LanguageId == languageId &&
                    st.Location.Contains(location)));
            }

            if (!string.IsNullOrEmpty(category))
            {
                predicate = predicate.And(x => x.Categories != null && x.Categories.Contains(category));
            }

            Func<IQueryable<Sport>, IIncludableQueryable<Sport, object>> include = query =>
                query.Include(s => s.SportTranslations.Where(st => st.LanguageId == languageId))
                     .Include(s => s.Currency).Include(s => s.Players).ThenInclude(p => p.PersonTranslations!.Where(pt => pt.LanguageId == languageId));

            var (items, totalCount) = await Repository.GetPagedAsync(
                predicate: predicate,
                orderBy: query => query.OrderBy(x => x.EventDate),
                include: include,
                page: page,
                pageSize: pageSize,
                AsNoTracking: true
            );

            var sports = await MapToViewModelsAsync(items, languageId);

            return (sports, totalCount);
        }
    }

    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left!, right!), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression? Visit(Expression? node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }
}