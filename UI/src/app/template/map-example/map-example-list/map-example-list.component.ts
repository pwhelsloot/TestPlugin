import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { MapExampleMapConfig } from '@app/template/models/map-example/map-example-map-config.model';
import { MapExample } from '@app/template/models/map-example/map-example.model';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-map-example-list',
  templateUrl: './map-example-list.component.html',
  styleUrls: ['./map-example-list.component.scss'],
})
export class MapExampleListComponent implements OnInit, OnDestroy {
  @Input() config: MapExampleMapConfig;
  subscriptions = new Array<Subscription>();
  mapExamples: MapExample[] = [];
  isCreating = false;
  mapPointId: number;

  ngOnInit() {
    this.subscriptions.push(
      this.config.mapExampleListService.mapExampleList$.subscribe((examples) => {
        this.mapExamples = examples;
      }),
      this.config.mapExampleDrawService.onCancelPoint.subscribe(() => {
        this.isCreating = false;
        this.mapPointId = null;
      }),
      this.config.mapExampleDrawService.onSavePoint.subscribe(() => {
        this.isCreating = false;
        this.mapPointId = null;
      })
    );
  }

  selectItem(mapPointId: number) {
    this.mapPointId = mapPointId;
    this.isCreating = true;
  }

  onDelete(mapPoint: MapExample) {
    this.config.translationService.translations.pipe(take(1)).subscribe((translations) => {
      const dialog = this.config.modalService.createModal({
        template: translations['template.mapExample.deleteConfirmationDialog.message'],
        title: translations['template.mapExample.deleteConfirmationDialog.title'],
        type: 'confirmation',
        largeSize: false,
      });

      dialog
        .afterClosed()
        .pipe(take(1))
        .subscribe((result) => {
          if (result) {
            this.config.mapExampleListService
              .deleteServicePoint(mapPoint.mapExampleId, translations['template.mapExample.deletedMessage'])
              .subscribe((response: boolean) => {
                if (response) {
                  this.config.mapExampleListService.requestMapExamples();
                }
              });
          }
        });
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  toggleCreateMode() {
    this.isCreating = true;
  }
}
