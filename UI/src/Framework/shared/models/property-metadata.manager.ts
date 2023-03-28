import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { IPropertiesMetadata } from '@shared-module/models/properties-metadata.interface';

export class PropertyMetadataManager {
    constructor(item: IPropertiesMetadata) {
        if (isTruthy((item as any).__typedJsonJsonObjectMetadataInformation__)) {
            this.buildDictionaryForTypedJSON(item);
        } else {
            this.buildDictionaryForLegacy(item);
        }
    }

    private propertyMetadataDictionary: {
        [key: string]: { isEditable: boolean; isMandatory: boolean; isDisplay: boolean; position: number };
    } = {};
    private ignoreMetadata = false;

    // RDM - Sometimes we don't have any metadata, for example if the data model was created on
    // angular side. So we want to use the manager (for times when populated via api) but sometimes want it to return
    // a default
    setMetadataDisabled(ignoreMetadata: boolean): PropertyMetadataManager {
        this.ignoreMetadata = ignoreMetadata;
        return this;
    }

    isPropertyEditable(propertyName: string): boolean {
        if (this.ignoreMetadata) {
            return true;
        }
        if (isTruthy(this.propertyMetadataDictionary[propertyName])) {
            return this.propertyMetadataDictionary[propertyName].isEditable;
        } else {
            return false;
        }
    }

    isPropertyMandatory(propertyName: string): boolean {
        if (this.ignoreMetadata) {
            return false;
        }
        if (isTruthy(this.propertyMetadataDictionary[propertyName])) {
            return this.propertyMetadataDictionary[propertyName].isMandatory;
        } else {
            return false;
        }
    }

    isPropertyDisplayed(propertyName: string): boolean {
        if (this.ignoreMetadata) {
            return true;
        }
        if (isTruthy(this.propertyMetadataDictionary[propertyName])) {
            return this.propertyMetadataDictionary[propertyName].isDisplay;
        } else {
            return false;
        }
    }

    getPropertyPosition(propertyName: string): number {
        if (this.ignoreMetadata) {
            return 0;
        }
        if (isTruthy(this.propertyMetadataDictionary[propertyName])) {
            return this.propertyMetadataDictionary[propertyName].position;
        } else {
            return 0;
        }
    }

    private buildDictionaryForTypedJSON(item: IPropertiesMetadata) {
        const typedJsonMappingFields: string[] = this.buildTypedJsonMappingFields((item as any).__typedJsonJsonObjectMetadataInformation__);
        const dictionary: { [propertyName: string]: { isEditable: boolean; isMandatory: boolean; isDisplay: boolean; position: number } } =
            {};
        item.propertiesMetadata.forEach((x) => {
            const key = typedJsonMappingFields[x.name];
            dictionary[key] = { isEditable: x.isEditable, isMandatory: x.IsMandatory, isDisplay: x.isDisplay, position: x.position };
        });
        this.propertyMetadataDictionary = dictionary;
    }

    private buildDictionaryForLegacy(item: IPropertiesMetadata) {
        const mappingFields: { name: string; isDate: boolean }[] = (item as any).constructor._alias;
        const dictionary: { [propertyName: string]: { isEditable: boolean; isMandatory: boolean; isDisplay: boolean; position: number } } =
            {};
        item.propertiesMetadata.forEach((x) => {
            const key = this.getKeyFromAliasName(mappingFields, x.name);
            dictionary[key] = { isEditable: x.isEditable, isMandatory: x.IsMandatory, isDisplay: x.isDisplay, position: x.position };
        });
        this.propertyMetadataDictionary = dictionary;
    }

    private getKeyFromAliasName(mappingFields: { name: string; isDate: boolean }[], name: string): string {
        let propertyName: string = null;
        for (const field in mappingFields) {
            if (mappingFields[field]) {
                const aliasField = mappingFields[field];
                if (aliasField.name === name) {
                    propertyName = field;
                    break;
                }
            }
        }
        return isTruthy(propertyName) ? propertyName : name;
    }

    private buildTypedJsonMappingFields(metadata: any): string[] {
        const mappingFields: string[] = [];
        metadata.dataMembers.forEach((_metadata) => {
            mappingFields[_metadata.name] = _metadata.key;
        });
        return mappingFields;
    }
}
