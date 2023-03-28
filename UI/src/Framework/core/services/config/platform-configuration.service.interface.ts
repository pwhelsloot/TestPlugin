import { PlatformConfiguration } from '@core-module/models/config/platform-configuration';
import { Observable } from 'rxjs';

export interface IPlatformConfigurationService {

    configuration$: Observable<PlatformConfiguration>;

    initialised$: Observable<void>;
}
