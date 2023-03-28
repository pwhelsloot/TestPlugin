import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { MapExample } from '@app/template/models/map-example/map-example.model';

/* This sets up all the subjects so that it can make use of the markers. */
@Injectable()
export class MapExampleDrawService {
  onAddPoint: Subject<MapExample> = new Subject<MapExample>();
  onEditPoint: Subject<MapExample> = new Subject<MapExample>();
  onMovePoint: Subject<[number, number]> = new Subject<[number, number]>();
  onSavePoint: Subject<MapExample> = new Subject<MapExample>();
  onCancelPoint: Subject<MapExample> = new Subject<MapExample>();
}
