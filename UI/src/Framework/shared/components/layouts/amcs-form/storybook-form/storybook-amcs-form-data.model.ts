import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';

@amcsJsonObject()
export class StorybookAmcsFormModel extends ApiBaseModel {
  @amcsJsonMember('Id')
  id: number;

  @amcsJsonMember('DropdownId')
  dropdownId: number;

  @amcsJsonMember('NumInput')
  numInput: number;

  @amcsJsonMember('SomeText')
  someText: string;

  @amcsJsonMember('Date')
  date: AmcsDate;

  @amcsJsonMember('Table')
  table: { id: number; description: string };

  @amcsJsonMember('TestId')
  testId: { id: number; description: string };

  @amcsJsonMember('TestName')
  testName: { id: number; description: string };

  @amcsJsonMember('TestIdTwo')
  testIdTwo: { id: number; description: string };

}
