import { TemplateRef } from '@angular/core';
import { nameof } from '@core-module/helpers/name-of.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { GridColumnType } from '@shared-module/components/amcs-grid/grid-column-type.enum';

@amcsJsonObject()
export class PersonalAccessToken extends ApiBaseModel implements ILookupItem {

    @amcsJsonMember('SysUserPrivateKeyId')
    id: number;

    @amcsJsonMember('SysUserId')
    sysUserId: number;

    @amcsJsonMember('Description')
    description: string;

    @amcsJsonMember('UserName')
    userName: string;

    @amcsJsonMember('CreationDate')
    creationDate: Date;

    @amcsJsonMember('Expire')
    expire: Date;

    @amcsJsonMember('Expires')
    expires: number;

    @amcsJsonMember('PrivateKey')
    privateKey: string;

    rowHighlighted: boolean;

    static getGridColumns(translations: string[], template: TemplateRef<any>): GridColumnConfig[] {
        return [
            new GridColumnConfig(translations['personalAccessToken.description'], nameof<PersonalAccessToken>('description'), [20, 20]),
            new GridColumnConfig(translations['personalAccessToken.userName'], nameof<PersonalAccessToken>('userName'), [15, 15]),
            new GridColumnConfig(translations['personalAccessToken.creationDate'], nameof<PersonalAccessToken>('creationDate'), [15, 15]),
            new GridColumnConfig(translations['personalAccessToken.expiryDate'], nameof<PersonalAccessToken>('expire'), [15, 15]),
            new GridColumnConfig(translations['personalAccessToken.value'], nameof<PersonalAccessToken>('privateKey'), [20, 20]).withType(GridColumnType.template).withTemplate(template)
        ];
    }
}
