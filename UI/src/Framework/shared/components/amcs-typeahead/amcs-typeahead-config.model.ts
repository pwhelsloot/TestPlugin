export class AmcsTypeaheadConfig {
    // The typeahead items, can be list of strings or complex object list.
    typeaheadItems: any;

    // Makes typeahead async, good for large datasets
    typeaheadAsync?: boolean;

    // Sets minimum characters typed before typeahead kicks in.
    typeaheadMinLength?: number;

    // Sets maximum options limit.
    typeaheadOptionsLimit?: number;

    // For complex items, set what property typehead matches on.
    typeaheadOptionField?: string;

    // For complex items, group options by another property.
    typeaheadGroupField?: string;

    typeaheadSingleWords?: boolean;
}
