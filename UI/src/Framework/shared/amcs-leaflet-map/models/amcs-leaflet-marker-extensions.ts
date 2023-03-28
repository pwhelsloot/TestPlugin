import { Content, DomEvent, DomUtil, Icon, LatLngExpression, Layer, marker, Marker, Popup, PopupOptions } from 'leaflet';
import { take } from 'rxjs/operators';
import { AmcsLeafletMarkerOptions } from './amcs-leaflet-marker-options.model';

export function amcsMarker(latlng: LatLngExpression, options?: AmcsLeafletMarkerOptions): AmcsMarker {
  const newMarker = marker(latlng, options) as AmcsMarker;
  const amcsNewMarker = Object.assign(new AmcsMarker(latlng, options), newMarker);
  amcsNewMarker.buildPopup();
  return amcsNewMarker;
}

interface AmcsMarkerMouseEvent {
  originalEvent: {
    toElement: any;
    relatedTarget: any;
    fromElement: any;
  };
}

export class AmcsMarker extends Marker {
  declare _popup: any;
  declare _icon: Icon;
  options: AmcsLeafletMarkerOptions;

  private isMouseOut = true;

  bindPopup(content: ((layer: Layer) => Content) | Content | Popup, options?: PopupOptions) {
    super.bindPopup.apply(this, [content, options]);
    this.off('click');
    this.on('mouseover', this.markerMouseOver.bind(this), this);
    this.on('mouseout', this.markerMouseOut.bind(this), this);
    return this;
  }

  /**
   * Build the Popup if InfoText or InfoTextFunction is provided
   */
  buildPopup() {
    if (this.options.infoTextFunction) {
      this.buildPopupFromFn();
    } else if (this.options.infoText) {
      this.buildPopupFromText();
    }
  }

  /**
   * Open popup with delayDuration
   * @param latlng
   * @returns
   */
  openPopup(latlng?: LatLngExpression): this {
    setTimeout(() => {
      this.openPopupFn(latlng);
    }, this.options.delayDuration);
    return this;
  }

  private buildPopupFromText() {
    this.bindPopup(this.options.infoText);
  }

  private buildPopupFromFn() {
    this.bindPopup('');
  }

  /**
   * Marker MouseOut event handler
   * @param e
   * @returns True if we are still hovering over the Marker
   */
  private markerMouseOut(e: AmcsMarkerMouseEvent): boolean {
    this.setHoveringOutsideMarker();

    let target = this.getMouseOutTarget(e);

    if (this.isHoveringOverPopup(target)) {
      DomEvent.on(this._popup._container, 'mouseout', this.popupMouseOut, this);
      return true;
    }

    this.closePopup();
    return false;
  }

  /**
   * Marker MouseOver event handler
   * @param e
   * @returns
   */
  private markerMouseOver(e: AmcsMarkerMouseEvent): boolean {
    this.isMouseOut = false;

    let target = getMouseOverTarget();
    let parent = this.getParentFromTarget(target, 'leaflet-popup');

    if (this.isHoveringOverCurrentMarkerPopup(parent)) {
      return true;
    }

    this.openPopup();
    return false;

    function getMouseOverTarget() {
      return e.originalEvent.fromElement || e.originalEvent.relatedTarget;
    }
  }

  private getMouseOutTarget(e: AmcsMarkerMouseEvent) {
    return e.originalEvent.toElement || e.originalEvent.relatedTarget;
  }

  private setHoveringOutsideMarker() {
    this.isMouseOut = true;
  }

  /**
   * Check if we are currently hovering over the current marker popup
   * @param target
   * @returns
   */
  private isHoveringOverCurrentMarkerPopup(target) {
    return target === this._popup._container;
  }

  /**
   * Main popup function, either uses provided infoTextFunction or instantly open it
   * @param latlng Position to open the popup
   */
  private openPopupFn(latlng: LatLngExpression) {
    if (this.options.infoTextFunction) {
      // take 2, first is default (loading) second has data filled in.
      this.options
        .infoTextFunction(this.options.infoTextPayload)
        .pipe(take(2))
        .subscribe((infoText) => {
          this.setPopupContent(infoText);
          this.callInnerPopupIfCurrentlyHovering(latlng);
        });
    } else {
      this.callInnerPopupIfCurrentlyHovering(latlng);
    }
  }

  /**
   * OpenPopup if we are still hovering over it
   * @param latlng Position to open the popup
   */
  private callInnerPopupIfCurrentlyHovering(latlng?: LatLngExpression) {
    if (!this.isMouseOut) {
      super.openPopup.apply(this, latlng);
    }
  }

  /**
   * Event handler for popupMouseOut
   * @param e
   * @returns
   */
  private popupMouseOut(e) {
    DomEvent.off(this._popup, 'mouseout', this.popupMouseOut, this);

    let target = this.getMouseOverTarget(e);

    if (this.isHoveringOverPopup(target)) {
      return true;
    }

    if (this.isHoveringOverIcon(target)) {
      return true;
    }

    this.closePopup();
    return false;
  }

  /**
   * Get the current mouseover target from given event
   * @param e Event
   * @returns Target if any
   */
  private getMouseOverTarget(e) {
    return e.toElement || e.relatedTarget;
  }

  /**
   * Check if we are hovering over the current Marker icon
   * @param target
   * @returns True if we are hovering over the Marker icon
   */
  private isHoveringOverIcon(target): boolean {
    return target === this._icon;
  }

  private isHoveringOverPopup(target): boolean | HTMLElement {
    return this.getParentFromTarget(target, 'leaflet-popup');
  }

  /**
   * Traverse nodes upwards to try and get element with given className
   * @param element
   * @param className
   * @returns
   */
  private getParentFromTarget(element, className: string): HTMLElement | boolean {
    let parent = element?.parentNode;

    while (parent != null) {
      if (parent.className && DomUtil.hasClass(parent, className)) {
        return parent;
      }
      parent = parent.parentNode;
    }

    return false;
  }
}
