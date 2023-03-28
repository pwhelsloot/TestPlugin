import { ApiBaseModel, amcsJsonMember, amcsJsonObject, amcsApiUrl } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
@amcsApiUrl('api/templates')
export class TemplateFeature extends ApiBaseModel {
  @amcsJsonMember('TemplateName')
  templateName: string;

  @amcsJsonMember('TemplateDate', true)
  templateDate: Date;
}
