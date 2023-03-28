import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';

@Injectable()
export class AppRouteParameterGuard implements CanActivate {
  constructor(private readonly router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const id = Number(route.params['id']);
    if (id !== null && id !== undefined && !isNaN(id) && id >= -2147483648 && id <= 2147483647) {
      return true;
    } else {
      this.router.navigate([CoreAppRoutes.notFound]);
      return false;
    }
  }
}
