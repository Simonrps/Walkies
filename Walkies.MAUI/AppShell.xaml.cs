namespace Walkies.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    private static void RegisterRoutes()
    {
        // Auth
        Routing.RegisterRoute("login", typeof(Views.LoginPage));
        Routing.RegisterRoute("register", typeof(Views.RegisterPage));

        // Owner routes
        Routing.RegisterRoute("owner/dashboard", typeof(Views.OwnerDashboardPage));
        Routing.RegisterRoute("owner/profile", typeof(Views.OwnerProfilePage));
        Routing.RegisterRoute("owner/dogs", typeof(Views.OwnerDogsPage));
        Routing.RegisterRoute("owner/adddog", typeof(Views.OwnerAddDogPage));
        Routing.RegisterRoute("owner/walkrequest", typeof(Views.OwnerWalkRequestPage));
        Routing.RegisterRoute("owner/searchwalkers", typeof(Views.OwnerSearchWalkersPage));
        Routing.RegisterRoute("owner/bookings", typeof(Views.OwnerBookingsPage));
        Routing.RegisterRoute("owner/map", typeof(Views.OwnerMapPage));
        Routing.RegisterRoute("owner/tracking", typeof(Views.OwnerTrackingPage));
        Routing.RegisterRoute("owner/messages", typeof(Views.OwnerMessagesPage));
        Routing.RegisterRoute("owner/payment", typeof(Views.OwnerPaymentPage));

        // Walker routes
        Routing.RegisterRoute("walker/dashboard", typeof(Views.WalkerDashboardPage));
        Routing.RegisterRoute("walker/profile", typeof(Views.WalkerProfilePage));
        Routing.RegisterRoute("walker/searchrequests", typeof(Views.WalkerSearchRequestsPage));
        Routing.RegisterRoute("walker/availability", typeof(Views.WalkerAvailabilityPage));
        Routing.RegisterRoute("walker/checkin", typeof(Views.WalkerCheckInPage));
        Routing.RegisterRoute("walker/messages", typeof(Views.WalkerMessagesPage));
        Routing.RegisterRoute("walker/payments", typeof(Views.WalkerPaymentsPage));
    }
}