import { ApiBaseModel, amcsJsonMember, amcsJsonObject } from '@core-module/models/api/api-base.model';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';

@amcsJsonObject()
export class DinnerCourseLookup extends ApiBaseModel implements ILookupItem {
  @amcsJsonMember('DinnerCourseId')
  id: number;

  @amcsJsonMember('Description')
  description: string;
}
