import { Injectable } from '@angular/core';

@Injectable()
export class ComponentFilterApiOptionService  {
    // This service is used to notify whether the filters are applied to the in-memory data the filter component is holding or api.
    // This is injected as an optional service to the component filter editor service. When injected, this filter is treated as api filtering,
    // if not it does the in-memory filtering.
}
