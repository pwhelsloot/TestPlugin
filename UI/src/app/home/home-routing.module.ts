import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MyProfileComponent } from '@app/home/my-profile/my-profile.component';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';

const routes: Routes = [
  {
    path: CoreAppRoutes.myProfile,
    component: MyProfileComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class HomeRoutingModule {}
