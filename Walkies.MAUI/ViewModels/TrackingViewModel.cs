using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Walkies.MAUI.Models;
using Walkies.MAUI.Services;
using MapsuiMap = Mapsui.Map;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for the owner tracking page. Polls the API at regular
    /// intervals to display the walkers real time location on a map.
    /// Related to US15 - GPS Tracking During Walk
    /// </summary>
    public partial class TrackingViewModel(ApiService apiService, AuthService authService) : BaseViewModel
    {
        private readonly ApiService _apiService = apiService;
        private readonly AuthService _authService = authService;
        private CancellationTokenSource? _pollCts;
        private const int PollingIntervalSeconds = 10;

        /// <summary>
        /// Gets or sets the mapsui map instance
        /// </summary>
        [ObservableProperty]
        public partial MapsuiMap? MapInstance { get; set; }

        /// <summary>
        /// Gets or sets the active booking being tracked
        /// </summary>
        [ObservableProperty]
        public partial BookingModel? ActiveBooking { get; set; }

        /// <summary>
        /// Gets or sets whether tracking is currently active
        /// </summary>
        [ObservableProperty]
        public partial bool IsTracking { get; set; }

        /// <summary>
        /// Gets or sets whether no active walk is found
        /// </summary>
        [ObservableProperty]
        public partial bool NoActiveWalk { get; set; }

        /// <summary>
        /// Gets or sets the last known location update time
        /// </summary>
        [ObservableProperty]
        public partial string LastUpdated { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether GPS signal has been lost
        /// </summary>
        [ObservableProperty]
        public partial bool SignalLost { get; set; }

        /// <summary>
        /// Loads the active booking and starts location polling.
        /// Related to US15 - GPS Tracking During Walk
        /// </summary>
        [RelayCommand]
        public async Task LoadTrackingAsync()
        {
            IsBusy = true;
            ClearError();
            NoActiveWalk = false;
            IsTracking = false;

            try
            {
                var ownerId = await _authService.GetUserIdAsync();
                var bookings = await _apiService.GetBookingsAsync(ownerId);

                var activeBooking = bookings?.FirstOrDefault(b => b.Status == "Active");

                if (activeBooking == null)
                {
                    NoActiveWalk = true;
                    return;
                }

                ActiveBooking = activeBooking;
                IsTracking = true;

                var map = new MapsuiMap();
                map.Layers.Add(OpenStreetMap.CreateTileLayer());
                MapInstance = map;

                await UpdateMapAsync(activeBooking);
                StartPolling();
            }
            catch (Exception ex)
            {
                SetError($"An error occurred: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Updates the map with the walkers current location
        /// </summary>
        private async Task UpdateMapAsync(BookingModel booking)
        {
            try
            {
                var updated = await _apiService.GetBookingAsync(booking.Id);
                if (updated == null)
                { 
                    return;
                }

                ActiveBooking = updated;

                if (updated.CurrentLatitude == null || updated.CurrentLongitude == null)
                {
                    SignalLost = true;
                    LastUpdated = $"Last updated: {DateTime.Now:HH:mm:ss} (signal lost)";
                    return;
                }

                SignalLost = false;
                LastUpdated = $"Last Updated: {DateTime.Now:HH:mm:ss}";

                if (MapInstance == null)
                {
                    return;
                }

                var walkerLayer = MapInstance.Layers.FirstOrDefault(l => l.Name == "Walker");
                if (walkerLayer != null)
                {
                    MapInstance.Layers.Remove(walkerLayer);
                }

                var (x, y) = SphericalMercator.FromLonLat(updated.CurrentLongitude.Value,
                    updated.CurrentLatitude.Value);

                var feature = new PointFeature(new MPoint(x, y));
                feature.Styles.Add(new SymbolStyle
                {
                    SymbolScale = 0.8,
                    Fill = new Mapsui.Styles.Brush(
                        Mapsui.Styles.Color.FromArgb(255, 255, 100, 0)),
                    Outline = new Pen(Mapsui.Styles.Color.White, 2)
                });

                MapInstance.Layers.Add(new MemoryLayer
                {
                    Name = "Walker",
                    Features = [feature],
                    Style = null
                });
                MapInstance.Navigator.CenterOnAndZoomTo(
                    new MPoint(x, y), MapInstance.Navigator.Resolutions[14]);
            }
            catch (Exception ex)
            {
                SignalLost = true;
                LastUpdated = $"Signal lost at {DateTime.Now:HH:mm:ss}: {ex.Message}";
            }
        }

        /// <summary>
        /// Starts polling the api at regular intervals for location updates
        /// </summary>
        private void StartPolling()
        {
            StopPolling();
            _pollCts = new CancellationTokenSource();
            var token = _pollCts.Token;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(PollingIntervalSeconds), token);
                    if (token.IsCancellationRequested)
                        break;

                    var booking = ActiveBooking;
                    if (booking != null)
                        await UpdateMapAsync(booking);
                }
            }, token);
        }

        /// <summary>
        /// Stops the polling loop
        /// </summary>
        public void StopPolling()
        {
            _pollCts?.Cancel();
            _pollCts?.Dispose();
            _pollCts = null;
        }

        /// <summary>
        /// Exposes LoadTrackingAsync aas the basepage loadcommand
        /// </summary>
        public override IAsyncRelayCommand? LoadCommand => LoadTrackingCommand;
    }
}
