import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserProfileService } from '@coreservices/user-profile-service/user-profile.service';
import { MyProfileData } from '@core-module/models/external-dependencies/profile/my-profile-data.model';

@Component({
  selector: 'app-my-profile',
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.scss'],
})
export class MyProfileComponent implements OnInit, OnDestroy {
  profileData: MyProfileData;
  constructor(private profileService: UserProfileService) {}
  private dataSub: Subscription;
  ngOnInit() {
    this.profileService.requestProfileData();
    this.dataSub = this.profileService.myProfile$.subscribe((profileData) => {
      this.profileData = profileData;
    });
  }

  ngOnDestroy(): void {
    this.dataSub.unsubscribe();
  }
}
