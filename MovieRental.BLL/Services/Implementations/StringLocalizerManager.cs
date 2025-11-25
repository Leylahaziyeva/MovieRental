using Microsoft.Extensions.Localization;
using System.Text;

namespace MovieRental.BLL.Services.Implementations
{
    public class StringLocalizerManager
    {
        private readonly IStringLocalizer _stringLocalizer;

        public StringLocalizerManager(IStringLocalizerFactory stringLocalizerFactory)
        {
            _stringLocalizer = stringLocalizerFactory.Create("SharedResources", "MovieRental.MVC");
        }

        public string GetValue(string key)
        {
            return _stringLocalizer.GetString(key);
        }

        // Parametrli versiya 
        public string GetValue(string key, params object[] arguments)
        {
            return _stringLocalizer.GetString(key, arguments);
        }

        // Indexer (istəyə görə)
        public string this[string key] => GetValue(key);
    }
}