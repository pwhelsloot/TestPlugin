import { amcsApiUrl, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
@amcsApiUrl(GlossaryLanguage.endpointName)
export class GlossaryLanguage extends ApiBaseModel {
  static readonly endpointName = 'glossary/glossaryLanguages';

  @amcsJsonMember('GlossaryLanguageId')
  id: number;

  @amcsJsonMember('Original')
  original: string;

  @amcsJsonMember('Translated')
  translated: string;
}
