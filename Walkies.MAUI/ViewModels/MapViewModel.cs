using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using MapsuiMap = Mapsui.Map;
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.Layers;
using Mapsui.Styles;
using Walkies.MAUI.Models;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for the owner map page. loads available walkers
    /// and displays them as markers on the map.
    /// Related to US14 - Map Display for Walkers
    /// </summary>
    public partial class MapViewModel(ApiService apiService, AuthService authService) : BaseViewModel
    {
        private readonly ApiService _apiService = apiService;
        private readonly AuthService _authService = authService;

        /// <summary>
        /// Gets or sets the Mapsui map instance
        /// </summary>
        [ObservableProperty] 
        public partial MapsuiMap? MapInstance { get; set; }

        /// <summary>
        /// Gets or sets the the search radius in kilometres
        /// </summary>
        [ObservableProperty]
        public partial double DistanceKm { get; set; } = 10;

        /// <summary>
        /// Gets or sets the date to search walker availability for
        /// </summary>
        [ObservableProperty]
        public partial DateTime SearchDate { get; set; } = DateTime.Today.AddDays(1);

        /// <summary>
        /// Gets or sets whether no walkers were found
        /// </summary>
        [ObservableProperty]
        public partial bool NoWalkers { get; set; }

        /// <summary>
        /// Gets or sets available distance options
        /// </summary>
        public List<double> DistanceOptions { get; } = [1, 5, 10, 25, 50];

        /// <summary>
        /// Loads walkers and displays them on the map. Related to US14 - Map Display of Walkers
        /// </summary>
        [RelayCommand]
        public async Task LoadMapAsync()
        {
            IsBusy = true;
            ClearError();
            NoWalkers = false;

            try
            {
                var ownerId = await _authService.GetUserIdAsync();
                var owner = await _apiService.GetUserAsync(ownerId);

                if (owner?.Latitude == null || owner?.Longitude == null)
                {
                    SetError("Please turn on your GPS settings or set your " +
                        "location details in your profile before using the map.");
                    return;
                }
                var latitude = owner.Latitude.Value;
                var longitude = owner.Longitude.Value;

                var map = new MapsuiMap();
                map.Layers.Add(OpenStreetMap.CreateTileLayer());

                var walkers = await _apiService.GetWalkersAsync(latitude, longitude,
                    DistanceKm, SearchDate);

                if (walkers == null || walkers.Count == 0)
                {
                    NoWalkers = true;
                }
                else
                {
                    var markerLayer = CreateWalkerMarkerLayer(walkers);
                    map.Layers.Add(markerLayer);
                }

                var (x, y) = SphericalMercator.FromLonLat(longitude, latitude);
                map.Navigator.CenterOnAndZoomTo(new MPoint(x, y),
                    map.Navigator.Resolutions[10]);

                MapInstance = map;
            }
            catch (Exception ex)
            {
                SetError($"An error occured:{ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Creates a memory layer with markers for each walker
        /// </summary>
        private static MemoryLayer CreateWalkerMarkerLayer(List<UserModel> walkers)
        {
            var features = new List<IFeature>();

            foreach (var walker in walkers.Where(w => w.Latitude.HasValue && w.Longitude.HasValue))
            {
                var (x, y) = SphericalMercator.FromLonLat(
                    walker.Longitude!.Value, walker.Latitude!.Value);

                var feature = new PointFeature(new MPoint(x, y));
                feature["name"] = $"{walker.FirstName} {walker.LastName}";
                feature.Styles.Add(new SymbolStyle
                {
                    SymbolScale = 0.7,
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.FromArgb(255, 100, 149, 237)),
                    Outline = new Pen(Mapsui.Styles.Color.White, 2)
                });

                features.Add(feature);
            }

            return new MemoryLayer
            {
                Name = "Walkers",
                Features = features,
                Style = null
            };
        }

        /// <summary>
        /// Exposes loadmapasync as the basepage loadcommand
        /// </summary>
        public override IAsyncRelayCommand? LoadCommand => LoadMapCommand;
    }
}
