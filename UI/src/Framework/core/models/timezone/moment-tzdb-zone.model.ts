import { amcsJsonObject, ApiBaseModel, amcsJsonMember, amcsJsonArrayMember } from '../api/api-base.model';

@amcsJsonObject()
export class MomentTZDBZone extends ApiBaseModel {
    @amcsJsonMember('name')
    name: string;

    @amcsJsonArrayMember('abbrs', String)
    abbrs: string[];

    @amcsJsonArrayMember('untils', Number)
    untils: number[];

    @amcsJsonArrayMember('offsets', Number)
    offsets: number[];

    @amcsJsonMember('canonicalId')
    canonicalId: string;
}
