import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MyProfileFormService } from '@app/home/services/my-profile-form.service';
import { PreviousRouteService } from '@coreservices/previous-route.service';
import { BaseComponent } from '@shared-module/components/base.component';
import { LoggingVerbs } from '@coreservices/logging/loggingVerbs.model';
import { InstrumentationService } from '@coreservices/logging/instrumentationService.service';
import { UserProfileService } from '@coreservices/user-profile-service/user-profile.service';
import { HomeTranslationsService } from '@app/home/services/home-translations.service';
import { AmcsNotificationService } from '@coreservices/amcs-notification.service';
import { take } from 'rxjs/operators';
import { MyProfileData } from '@core-module/models/external-dependencies/profile/my-profile-data.model';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';

@Component({
  selector: 'app-profile-settings',
  templateUrl: './profile-settings.component.html',
  styleUrls: ['./profile-settings.component.scss'],
  providers: [MyProfileFormService],
})
export class ProfileSettingsComponent extends BaseComponent implements OnInit, OnDestroy {
  fileToUpload: File = null;
  imageSrc: string;
  imageSelected: string;
  theFile: any;
  profileData: MyProfileData;
  imageChanged = false;
  imageLoaded = false;

  constructor(
    public formService: MyProfileFormService,
    activatedRoute: ActivatedRoute,
    instrumentationService: InstrumentationService,
    private prevRoute: PreviousRouteService,
    router: Router,
    private route: ActivatedRoute,
    private profileService: UserProfileService,
    private translationService: HomeTranslationsService,
    private notificationService: AmcsNotificationService
  ) {
    super(router, activatedRoute, instrumentationService, LoggingVerbs.PageSearch);
  }

  ngOnInit() {
    this.formService.initialise();
    this.profileService.requestProfileData();
    this.profileService.myProfile$.subscribe((profileData) => {
      this.profileData = profileData;
      this.formService.form.formGroup.controls.autoPrint.setValue(this.profileData.autoPrint);
      this.formService.form.formGroup.controls.printerId.setValue(this.profileData.printerId);
    });
    this.imageLoaded = false;
    this.imageChanged = false;
  }

  onSave() {
    this.formService.save();
    this.profileData.autoPrint = this.formService.form.formGroup.controls.autoPrint.value;
    this.profileData.printerId = this.formService.form.formGroup.controls.printerId.value;
    if (this.imageSrc != null && this.imageChanged) {
      this.profileData.profilePictureBase64 = this.imageSrc;
      this.imageChanged = false;
      this.imageLoaded = false;
    }
    this.profileService.save(this.profileData);
    this.canSave();
  }

  return() {
    this.prevRoute.navigationToPreviousUrl(['../' + CoreAppRoutes.search], { relativeTo: this.route });
  }

  handleFileInput(event: any) {
    if (event.target.files[0] != null && event.target.files[0].size <= 2097152) {
      this.fileToUpload = event.target.files[0];
      this.imageSelected = this.fileToUpload.name;
      this.theFile = event;
      this.imageLoaded = true;
    } else {
      this.translationService.translations.pipe(take(1)).subscribe((translations: string[]) => {
        this.notificationService.showNotification(translations['home.myProfile.image.size']);
      });
    }
  }

  onImageCropped(image: any) {
    this.imageSrc = image;
    if (this.imageSrc !== 'data:,') {
      this.imageChanged = true;
    }
  }

  canSave() {
    if (this.imageChanged) {
      return false;
    } else {
      return this.formService.form.formGroup.pristine;
    }
  }
}
