import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppRouteParameterGuard } from './app-route-parameter-guard.service';
import { CoreAppRoutes } from '@coreconfig/routes/core-app-routes.constants';
import { TemplateModuleAppRoutes } from './template/template-module-app-routes.constants';
import { TemplatePageLayoutComponent } from './template-shared/page-layout/page-layout.component';
import { environment } from '@environments/environment';
import { IndexComponent } from '@shared-module/index/index.component';
import { DifferentAppGuard } from '@core-module/guards/different-app.guard';

const appRoutes = [
  {
    path: CoreAppRoutes.index,
    component: IndexComponent,
    data: { title: 'title.app.index' },
    canActivate: [DifferentAppGuard],
  },
  {
    path: environment.applicationURLPrefix,
    redirectTo: CoreAppRoutes.index,
    pathMatch: 'full',
  },
  {
    path: '',
    component: TemplatePageLayoutComponent,
    data: { title: 'title.app.pageLayout' },
    canActivate: [DifferentAppGuard],
    children: [
      {
        path: TemplateModuleAppRoutes.module,
        loadChildren: () => import('./template/template.module').then((m) => m.TemplateModule),
      },
      {
        path: environment.applicationURLPrefix + '/' + CoreAppRoutes.notFound,
        loadChildren: () => import('./not-found/not-found.module').then((m) => m.NotFoundModule),
        data: { title: 'title.app.notFound' },
      },
      {
        path: CoreAppRoutes.homeModule,
        loadChildren: () => import('./home/home.module').then((m) => m.HomeModule),
      },
    ],
  },
  {
    path: '**',
    redirectTo: environment.applicationURLPrefix + '/' + CoreAppRoutes.notFound,
    data: { title: 'title.app.notFound' },
  },
];

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)],
  exports: [RouterModule],
  providers: [AppRouteParameterGuard],
})
export class AppRoutingModule {}
