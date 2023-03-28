import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { SysUserDashboardStaffType } from '../models/sysuser-dashboard-staff-type.model';
import { SysUserServiceData } from './sysuser.service.data';

/**
 * @deprecated Move to PlatformUI + ScaleUI (TDM / IMM 'fake' this so will break until they remove reference)
 */
@Injectable({ providedIn: 'root' })
export class SysUserService {

  sysUserStaffType = new ReplaySubject<number>(1);
  sysUserStaffTypes$ = new ReplaySubject<{ primaryStaffTypeId: number; additionalStaffTypeIds: number[] }>(1);
  initalised$ = new ReplaySubject(1);

  constructor(
    private dataService: SysUserServiceData
  ) { }

  getSysUserStaffType(sysUserId: number) {
    this.dataService
      .getSysUserDashboardStaffType(sysUserId)
      .pipe(take(1))
      .subscribe((dashboardStaffType: SysUserDashboardStaffType) => {
        // TODO: PAB: Would like to remove sysUserStaffType in future version. Did not remove to prevent breaking other apps.
        this.sysUserStaffType.next(dashboardStaffType.primaryStaffTypeId);
        this.sysUserStaffTypes$.next({ primaryStaffTypeId: dashboardStaffType.primaryStaffTypeId, additionalStaffTypeIds: dashboardStaffType.additionalStaffTypeIds });
        this.initalised$.next();
      });
  }
}
