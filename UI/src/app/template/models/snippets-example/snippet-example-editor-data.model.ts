import { amcsJsonObject, ApiBaseModel, amcsJsonMember, amcsApiUrl } from '@core-module/models/api/api-base.model';
import { SnippetExample } from './snippet-example.model';

// I'm an editor data-model created via 'editor-data' snippet.
@amcsJsonObject()
@amcsApiUrl('template/SnippetExampleEditorDatas')
export class SnippetExampleEditorData extends ApiBaseModel {
  @amcsJsonMember('DataModel')
  dataModel: SnippetExample;
}
