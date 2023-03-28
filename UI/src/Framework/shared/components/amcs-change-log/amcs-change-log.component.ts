import { Component, OnInit } from '@angular/core';
import { ChangeLog } from '@core-module/models/change-log/change-log.model';
import { HistoryEntryTypeEnum } from '@core-module/models/history-entry-type.enum';
import { AmcsChangeLogService } from '@core-module/services/amcs-change-log.service';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { AmcsModalChildComponent } from '../amcs-modal/amcs-modal-child-component.interface';

@Component({
  selector: 'app-amcs-change-log',
  templateUrl: './amcs-change-log.component.html',
  styleUrls: ['./amcs-change-log.component.scss'],
  providers: [AmcsChangeLogService]
})
export class AmcsChangeLogComponent implements OnInit, AmcsModalChildComponent {

  extraData: any;
  loading = new BehaviorSubject<boolean>(true);
  changeLog: ChangeLog = null;
  changeLogCount = 0;
  HistoryEntryTypeEnum = HistoryEntryTypeEnum;

  constructor(private changeLogService: AmcsChangeLogService) { }

  ngOnInit() {
    this.getChangeLogDetails(0);
  }

  onPageChange(page: number) {
    this.loading.next(true);
    this.getChangeLogDetails(page);
  }

  getChangeLogDetails(page: number) {
    this.changeLogService.getChangeLogDetails(this.extraData[0],
      this.extraData[1],
      this.extraData[2],
      page
    ) .pipe(take(1))
      .subscribe((data) => {
        this.changeLog = data.results[0];
        this.changeLogCount = data.count;
        this.loading.next(false);
      });
  }

}
