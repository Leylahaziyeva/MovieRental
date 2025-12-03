using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Person;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.UI.Controllers
{
    public class PersonController : Controller
    {
        private readonly IPersonService _personService;
        private readonly IMovieService _movieService;
        private readonly IEventService _eventService;
        private readonly ISportService _sportService;

        public PersonController(
            IPersonService personService,
            IMovieService movieService,
            IEventService eventService,
            ISportService sportService)
        {
            _personService = personService;
            _movieService = movieService;
            _eventService = eventService;
            _sportService = sportService;
        }

        public async Task<IActionResult> Index(PersonType? filterByType, string? searchQuery, string? sortBy, int page = 1, int pageSize = 12)
        {
            var allPersons = await _personService.GetAllAsync(
                predicate: p =>
                    (!filterByType.HasValue || p.PersonType == filterByType.Value) &&
                    (string.IsNullOrEmpty(searchQuery) ||
                     p.PersonTranslations!.Any(pt => pt.Name.Contains(searchQuery))),
                orderBy: sortBy == "DateAdded"
                    ? query => query.OrderByDescending(p => p.CreatedAt)
                    : query => query.OrderByDescending(p => p.KnownCredits),
                AsNoTracking: true
            );

            var totalCount = allPersons.Count();
            var persons = allPersons
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PersonListViewModel
            {
                People = persons.ToList(),
                FilterByType = filterByType,
                SearchQuery = searchQuery,
                SortBy = sortBy ?? "ListOrder",
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var person = await _personService.GetByIdAsync(id);
            if (person == null)
                return NotFound();

            var model = new PersonDetailViewModel
            {
                Person = person
            };

            if (person.PersonType == PersonType.Actor)
            {
                var movies = await _movieService.GetAllAsync(
                    predicate: m => m.MoviePersons!.Any(mp => mp.PersonId == id && mp.Role == MoviePersonRole.Actor),
                    AsNoTracking: true
                );
                model.Movies = movies.ToList();
            }

            if (person.PersonType == PersonType.Artist)
            {
                var events = await _eventService.GetAllAsync(
                    predicate: e => e.Artists!.Any(a => a.Id == id),
                    AsNoTracking: true
                );
                model.Events = events.ToList();
            }

            if (person.PersonType == PersonType.Sportsman)
            {
                var sports = await _sportService.GetAllAsync(
                    predicate: s => s.Players!.Any(p => p.Id == id),
                    AsNoTracking: true
                );
                model.Sports = sports.ToList();
            }

            return View(model);
        }

        public async Task<IActionResult> Actors(string? searchQuery, int page = 1, int pageSize = 12)
        {
            var actors = await _personService.GetActorsAsync();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                actors = actors.Where(a => a.Name!.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = actors.Count();
            var pagedActors = actors
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PersonListViewModel
            {
                People = pagedActors,
                FilterByType = PersonType.Actor,
                SearchQuery = searchQuery,
                SortBy = "ListOrder",
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize
            };

            return View("Index", model);
        }

        public async Task<IActionResult> Artists(string? searchQuery, int page = 1, int pageSize = 12)
        {
            var artists = await _personService.GetArtistsAsync();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                artists = artists.Where(a => a.Name!.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = artists.Count();
            var pagedArtists = artists
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PersonListViewModel
            {
                People = pagedArtists,
                FilterByType = PersonType.Artist,
                SearchQuery = searchQuery,
                SortBy = "ListOrder",
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize
            };

            return View("Index", model);
        }

        public async Task<IActionResult> Sportsmen(string? searchQuery, int page = 1, int pageSize = 12)
        {
            var sportsmen = await _personService.GetSportsmenAsync();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                sportsmen = sportsmen.Where(s => s.Name!.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = sportsmen.Count();
            var pagedSportsmen = sportsmen
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PersonListViewModel
            {
                People = pagedSportsmen,
                FilterByType = PersonType.Sportsman,
                SearchQuery = searchQuery,
                SortBy = "ListOrder",
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize
            };

            return View("Index", model);
        }

        public async Task<IActionResult> Popular(int count = 10)
        {
            var popularPeople = await _personService.GetTopPersonsAsync(count);
            return PartialView("_PopularPeoplePartial", popularPeople);
        }

        // Search people by name
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new List<object>());

            var people = await _personService.GetAllAsync(
                predicate: p => p.PersonTranslations!.Any(pt => pt.Name.Contains(query)),
                AsNoTracking: true
            );

            var results = people.Take(10).Select(p => new
            {
                id = p.Id,
                name = p.Name,
                profileImage = p.ProfileImageUrl,
                knownFor = p.KnownFor,
                personType = p.PersonType.ToString()
            });

            return Json(results);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMorePeople(PersonType? filterByType, string? searchQuery, string? sortBy, int page = 1, int pageSize = 12)
        {
            var allPersons = await _personService.GetAllAsync(
                predicate: p =>
                    (!filterByType.HasValue || p.PersonType == filterByType.Value) &&
                    (string.IsNullOrEmpty(searchQuery) ||
                     p.PersonTranslations!.Any(pt => pt.Name.Contains(searchQuery))),
                orderBy: sortBy == "DateAdded"
                    ? query => query.OrderByDescending(p => p.CreatedAt)
                    : query => query.OrderByDescending(p => p.KnownCredits),
                AsNoTracking: true
            );

            var persons = allPersons
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return PartialView("_PeopleCardPartial", persons);
        }
    }
}