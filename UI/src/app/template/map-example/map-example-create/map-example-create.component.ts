import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { MapExampleMapConfig } from '@app/template/models/map-example/map-example-map-config.model';
import { MapExampleEditorData } from '@app/template/models/map-example/map-example-editor-data.model';
import { MapExampleFormService } from '@app/template/services/map-example/map-example-form.service';
import { MapExampleService } from '@app/template/services/map-example/map-example.service';

/*MapExampleCreateComponent handles the if a new point should be added or an existing point should be edited */
@Component({
  selector: 'app-map-example-create',
  templateUrl: './map-example-create.component.html',
  providers: MapExampleFormService.providers,
})
export class MapExampleCreateComponent implements OnInit, OnDestroy {
  @Input() config: MapExampleMapConfig;
  @Input() mapPointId: number;

  subscriptions = new Array<Subscription>();
  constructor(public mapExampleFormService: MapExampleFormService, public mapExampleService: MapExampleService) {}

  ngOnInit() {
    this.subscriptions.push(
      this.mapExampleService.mapPointsEditorData$.subscribe((editorData: MapExampleEditorData) => {
        if (this.mapPointId > 0) {
          this.config.mapExampleDrawService.onEditPoint.next(editorData.mapExample);
        } else {
          this.config.mapExampleDrawService.onAddPoint.next(editorData.mapExample);
        }
      }),
      this.config.mapExampleListService.newMapExample$.subscribe((data) => {
        this.mapExampleService.requestNewServicePointEditorData(data);
      })
    );
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  onSave() {
    if (this.mapExampleFormService.isValid()) {
      this.mapExampleFormService
        .save()
        .pipe(take(1))
        .subscribe((data) => {
          if (isTruthy(data)) {
            this.config.mapExampleDrawService.onSavePoint.next();
          }
        });
    }
  }

  onCancel() {
    this.config.mapExampleDrawService.onCancelPoint.next();
  }
}
