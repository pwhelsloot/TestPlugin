import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class UserLookup extends ApiBaseModel implements ILookupItem {

    @alias('SysUserId')
    @amcsJsonMember('SysUserId')
    id: number;

    @alias('Forename')
    @amcsJsonMember('Forename')
    forename: string;

    @alias('Surname')
    @amcsJsonMember('Surname')
    surname: string;

    @alias('GUID')
    @amcsJsonMember('GUID')
    guid: string;

    description: string;

    getFullName() {
        return `${this.forename} ${this.surname}`;
    }
}
