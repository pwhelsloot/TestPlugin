import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';
import { PreviousRouteService } from '@coreservices/previous-route.service';
import { CoreTranslationsService } from '@coreservices/translation/core-translation.service';
import { UserProfileService } from '@coreservices/user-profile-service/user-profile.service';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.scss'],
})
export class NotFoundComponent implements OnInit {
  constructor(
    public translate: CoreTranslationsService,
    private previousRouteService: PreviousRouteService,
    private router: Router,
    private profileService: UserProfileService
  ) {}

  ngOnInit() {
    this.profileService.requestProfileData();
  }

  previousRoute() {
    // Unusual case here as we never record not-found navigations so getUrlAt(1) really is
    // getting the previous url
    const lastUrl: string = this.previousRouteService.getUrlAt(1);
    if (lastUrl !== null) {
      this.router.navigateByUrl(lastUrl);
    } else {
      this.router.navigateByUrl(CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search);
    }
  }
}
