export class CoreUserPreferenceKeys {
    static uiLanguage = 'uiLanguage';
    static gridColumnPreferences = 'gridColumnPreferences';
    static calendarDefaultView = 'calendarDefaultView';

    // The below syntax is only needed for CoreUserPreferenceKeys as used in oreUserPreferencesService.buildKey to decide
    // whether the key is from Core or from an Application.
    uiLanguage: 'uiLanguage';
    gridColumnPreferences = 'gridColumnPreferences';
    calendarDefaultView = 'calendarDefaultView';
}
