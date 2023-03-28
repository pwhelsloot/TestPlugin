import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/245825
 */
@Injectable({ providedIn: 'root' })
export class ReportingBiHelperService {

    constructor(private router: Router) { }

    Redirect(parameter: string[]) {
        this.navigate(parameter);
    }

    navigate(parameter: string[]): any {
        switch (parameter[0].trim()) {
            case 'Customer':
                this.router.navigateByUrl(CoreAppRoutes.customerModule + '/' + parameter[1] + '/' + CoreAppRoutes.dashboard);
                break;
            case 'CustomerCommunication':
                this.router.navigateByUrl(CoreAppRoutes.customerModule + '/' + parameter[1] + '/' + CoreAppRoutes.communications + '/' + parameter[2]);
                break;
            default:
                break;
        }
    }
}
