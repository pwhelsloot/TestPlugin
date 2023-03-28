import { amcsJsonObject, ApiBaseModel, amcsJsonMember, amcsJsonArrayMember } from '@core-module/models/api/api-base.model';

// I'm a data-model created via 'data-model' snippet.
@amcsJsonObject()
export class SnippetExample extends ApiBaseModel {
  @amcsJsonMember('SnippetExampleId')
  snippetExampleId: number;

  // I'm a data-model property created via 'd-prop' snippet.
  @amcsJsonMember('SnippetExampleProperty')
  snippetExampleProperty: number;

  // I'm a data-model array property created via 'd-aprop' snippet.
  @amcsJsonArrayMember('SnippetExampleArrayProperties', Number)
  snippetExampleArrayProperties: Number[];

  // I'm a data-model date property created via 'd-dprop' snippet.
  @amcsJsonMember('SnippetExampleDateProperty', true)
  snippetExampleDateProperty: Date;
}
