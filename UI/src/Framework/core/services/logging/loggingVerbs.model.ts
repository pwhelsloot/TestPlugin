export class LoggingVerbs {
    static readonly AppLoading = 'App Loading';
    static readonly PageSearch = 'Global Search';
    static readonly PageMyProfile = 'My Profile';
    static readonly PageLogin = 'Login';
    static readonly PageCustomerDashboard = 'Customer Dashboard';
    static readonly PageCustomerDashboardCalendar = 'Customer Dashboard Calendar';
    static readonly PageCustomerDashboardFinance = 'Customer Dashboard Finance';
    static readonly PageCustomerDashboardOperations = 'Customer Dashboard Operations';
    static readonly PageCustomerContactEditor = 'Customer Contact Editor';
    static readonly PageCustomerContactBrowser = 'Customer ContactBrowser';
    static readonly PageCustomerServiceLocations = 'Customer Service Locations';
    static readonly PageCustomerCommunications = 'Customer Communications';
    static readonly PageCustomerMissedCollection = 'Customer Missed Collection';
    static readonly PageServiceLocationWizard = 'Service Location Wizard';
    static readonly PageActiveServiceResidential = 'Active Service Residential';

    static readonly ComponentOnInit = 'Init Component';
    static readonly ComponentOnDestroy = 'Destroy Component';

    static readonly ViewModeChange = 'View Change';
    static readonly ViewModeInline = 'Inline';
    static readonly ViewModeSideBySide = 'Side By Side';
    static readonly ViewModeCalendarChange = 'Customer Calendar View Change';

    static readonly EventPageManualTimedView = 'Manually Timed Page View';
    static readonly UnTimedPageView = 'UnTimed Page View';
    static readonly EventLoginPassed = 'Login Passed';
    static readonly EventLoginFailed = 'Login Failed - Invalid Credentials';
    static readonly EventLoginLocked = 'Login Failed - AccountLocked';
    static readonly EventAuthError = 'Authentication Error';

    static readonly EventLaunchDesktop = 'Launch Desktop';
    static readonly EventLaunchDesktopTokenisedCall = 'Launch Desktop Token';

    static readonly EventCustomerOpertionsSelectJob = 'Customer Operations Job Selected';
    static readonly EventCustomerOpertionsSelectTab = 'Customer Operations Drill Down Tab Selected';

    static readonly ErrorHttp = 'Http Error!';
    static readonly ErrorGlobal = 'Global Error';

    static readonly RequestHttp = 'Http Request';

    static readonly CustomerEdit = 'Customer Edit';

    static readonly BrowserContact = 'Customer Contact Browser';
    static readonly EditorContactEdit = 'Customer Contact Editor Edit';
    static readonly EditorContactAdd = 'Customer Contact Editor Add';

    static readonly ServiceAgreementView = ' Customer Service Agreement View';

    static readonly BrowserServiceLocation = 'Customer Service Location Browser';
    static readonly EditorServiceLocationAdd = 'Customer Service Location Add';
    static readonly EditorServiceLocationEdit = 'Customer Service Location Edit';
    static readonly EditorServiceLocationView = 'Customer Service Location View';
    static readonly EditorServiceLocationWizardStepNavigated = 'Customer Service Location Wizard Step Navigated';
    static readonly EditorServiceLocationWizardStepInitialised = 'Customer Service Location Wizard Step Initialised';

    static readonly ResidentialHouseholdOrdersBrowser = 'Residential Household Orders Browser';

    static readonly ContextTitle = 'Tile';

    static readonly GlobalSearchRequest = 'Global Search Request';
    static readonly GlobalSearchRequestTimer = 'Global Search Request Timer';

    static readonly MyProfileRequest = 'My Profile Request';
    static readonly MyProfileRequestTimer = 'My Profile Request Timer';

    static readonly MunicipalOfferingsSearchRequest = 'Municipal Offerings Search Request';
    static readonly MunicipalOfferingsSearchRequestTimer = 'Municipal Offerings Search Request Timer';

    static readonly CustomerSearchRequest = 'Customer Search Request';
    static readonly CustomerSearchRequestTimer = 'Customer Search Request Timer';

    static readonly SiteSearchRequest = 'Site Search Request';
    static readonly SiteSearchRequestTimer = 'Site Search Request Timer';

    static readonly ContractSearchRequest = 'Contract Search Request';
    static readonly ContractSearchRequestTimer = 'Contract Search Request Timer';

    static openEditorEvent(source: string): string {
        return 'Open ' + source + ' Editor';
    }
}
