import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
export class UserCommunication {
    @alias('OwnerSysUserId')
    ownerSysUserId: number;

    @alias('CommunicationOutcomeId')
    communicationOutcomeId: number;

    @alias('CommunicationId')
    communicationId: number;

    @alias('CustomerId')
    customerId: number;

    @alias('CustomerName')
    customerName: string;

    @alias('Surname')
    surname: string;

    @alias('CommunicationTypeId')
    communicationTypeId: number;

    @alias('CommunicationType')
    communicationType: string;

    @alias('Forename')
    forename: string;

    @alias('ReviewDate', true)
    reviewDate: Date;

    @alias('Notes')
    notes: string;

    @alias('IsGroup')
    isgroup: boolean;

    @alias('GroupName')
    groupName: string;

    overdue: boolean;

    fullName: string;
}
