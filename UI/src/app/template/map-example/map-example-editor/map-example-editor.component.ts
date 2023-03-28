import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { MapExampleMapConfig } from '@app/template/models/map-example/map-example-map-config.model';
import { MapExampleFormService } from '@app/template/services/map-example/map-example-form.service';
import { MapExampleService } from '@app/template/services/map-example/map-example.service';
import { MapExampleEditorData } from '@app/template/models/map-example/map-example-editor-data.model';

@Component({
  selector: 'app-map-example-editor',
  styleUrls: ['./map-example-editor.component.scss'],
  templateUrl: './map-example-editor.component.html',
})
export class MapExampleEditorComponent implements OnInit, OnDestroy {
  @Input() mapPointId: number;
  @Input() config: MapExampleMapConfig = null;
  @Output() onSaveArea = new EventEmitter();
  @Output() onCancelArea = new EventEmitter();

  constructor(public mapExampleFormService: MapExampleFormService, public mapExampleService: MapExampleService) {}

  private subscriptions = new Array<Subscription>();
  ngOnInit() {
    this.subscriptions.push(
      this.config.mapExampleDrawService.onMovePoint.subscribe((latLang) => {
        this.mapExampleFormService.form.latitude.setValue(latLang[0]);
        this.mapExampleFormService.form.longitude.setValue(latLang[1]);
      }),
      this.mapExampleService.mapPointsEditorData$.subscribe((op: MapExampleEditorData) => {
        this.mapExampleFormService.editorData = op;
        this.mapExampleFormService.buildForm();
      })
    );
    if (this.mapPointId > 0) {
      this.mapExampleService.requestEditServicePointEditorData(this.mapPointId);
    } else {
      this.config.mapExampleListService.requestNewMapExampleEditorData();
    }
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }
  onSave() {
    if (this.mapExampleFormService.isValid()) {
      this.onSaveArea.emit();
    }
  }

  onCancel() {
    this.onCancelArea.emit();
  }
}
