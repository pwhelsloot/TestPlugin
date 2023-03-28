import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { MomentTZDBZone } from './moment-tzdb-zone.model';

@amcsJsonObject()
export class MomentTZDB extends ApiBaseModel {
    @amcsJsonMember('version')
    version: string;

    @amcsJsonArrayMember('zones', MomentTZDBZone)
    zones: MomentTZDBZone[];

    @amcsJsonArrayMember('links', String)
    links: string[];
}
