import { FieldDefinition } from './field-definition.model';

/**
 * @deprecated Move to PlatformUI
 */
export interface IFieldDefinitionGroup {
    definitions: FieldDefinition[];
    buildDefinitions(translations?: string[]): IFieldDefinitionGroup;
}
