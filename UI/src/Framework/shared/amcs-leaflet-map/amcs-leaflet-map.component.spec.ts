import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import { LeafletDrawModule } from '@asymmetrik/ngx-leaflet-draw';
import { LeafletMarkerClusterModule } from '@asymmetrik/ngx-leaflet-markercluster';
import { Subject } from 'rxjs';
import { AmcsLeafletMapComponent } from './amcs-leaflet-map.component';
import { AmcsLeafletMapOptions } from './models/amcs-leaflet-map.options';
import { AmcsLeafletMapEngineService } from './services/amcs-leaflet-map-engine.service';

/*  describe:  always execute all tests in this spec
    fdescribe: ONLY execute all tests in this spec, this will skip any other .spec.ts files! usefull when writing tests
    xdescribe: skip this test, this will disable all tests in this spec,
    use for developing only, do not push these changes to the Develop Branch without notifying Team members,
    only use it when you want to disable a test while developing other Tests)
*/

describe('Component: AmcsLeafletMap', () => {
  let component: AmcsLeafletMapComponent;
  let fixture: ComponentFixture<AmcsLeafletMapComponent>;
  let mockMapEngine: Partial<AmcsLeafletMapEngineService> = {};
  beforeEach(waitForAsync((() => {

    /* Our Mock AmcsLeafletMapEngineService,
      we do not want to use the actual Service that is not the purpose of this file,
    */
    mockMapEngine = {
      options: new AmcsLeafletMapOptions(),
      onMarkerDragged$: new Subject<[number, number] | string>(),
      invalidateSize: () => { },
      initialize: () => { },
      tryFitToBounds: () => { },
      fitToBounds: () => { }
    };

    //Configure Test environment
    TestBed.configureTestingModule({
      // Components we need to execute our Tests
      declarations: [AmcsLeafletMapComponent],
      imports: [
        // The component has direct dependencies(can be types,services, or models etc) that live in these modules
        LeafletModule,
        LeafletDrawModule,
        LeafletMarkerClusterModule,
      ],
      //Here we actually tell Angular to use our mock/dummy service we created above instead of the real one!
      providers: [
        { provide: AmcsLeafletMapEngineService, useValue: mockMapEngine }],
      /*  When a component has references to other Components in its HTML by default Angular will try to initialize those aswell
          But since we are only testing the Map component we dont want this.
          We have 2 options to ignore the initialisation of these components
            1.  Mock these components: Lets say we want to mock the app-amcs-button that is referenced in the map component
                we can create a FakeAmcsButtonComponent that has the same selector as the app-amcs-button and give it no actual implementation
            2.  Use the NO_ERRORS_SCHEMA: This basically tells angular to ignore any errors that come from missing components
       */
      schemas: [NO_ERRORS_SCHEMA]
    });
    /*  The AmcsLeafletMapComponent provides Services without actually using them itself
        (see line AmcsLeafletMapComponent: 27)
        In this Test we are only testing the Component so we need to either mock them or remove them from the Providers list
    */
    TestBed.overrideComponent(AmcsLeafletMapComponent, {
      set: {
        providers: [] //override providers
      }
    });
    // Build Test environment
    TestBed.compileComponents();
  })));

  // This will recreate our AmcsLeafletMapComponent for before executing a actual test
  beforeEach(waitForAsync(() => {
    fixture = TestBed.createComponent(AmcsLeafletMapComponent);
    //Set the component instance to the one created by the Test environment
    component = fixture.componentInstance;
    //Fire Change detection so component is actually created
    fixture.detectChanges();
  }));

  /*
    it:  always execute this test
    fit: only execute this Test, this will skip any other tests(other 'it')
    xit: skip this test
  */
  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('Should update height of viewport', () => {
    //arrange
    component.mapHeight = 999;
    component.mapHeightUnit = 'px';
    spyOn(mockMapEngine, 'invalidateSize');
    //act
    component.setHeightOfMapViewport();

    //assert
    expect(component.mapDiv.nativeElement.style.height).toBe('999px');
    expect(mockMapEngine.invalidateSize).toHaveBeenCalled();
  });

  it('Should ignore viewport height update when map is not loaded', () => {
    //arrange
    component.mapHeight = 999;
    component.mapHeightUnit = 'px';
    component.mapDiv = undefined;
    spyOn(mockMapEngine, 'invalidateSize');

    //act
    component.setHeightOfMapViewport();

    //assert
    expect(mockMapEngine.invalidateSize).not.toHaveBeenCalled();
  });

  it('should call tryFitToBounds', () => {
    //arrange
    spyOn(mockMapEngine, 'tryFitToBounds');

    //act
    component.fitToBounds();

    //assert
    expect(mockMapEngine.tryFitToBounds).toHaveBeenCalled();
  });
});
