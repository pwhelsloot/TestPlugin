import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated To be deleted
 */
export class FollowupActionLookup {
    @alias('CommunicationFollowupActionId')
    id: number;

    @alias('Description')
    description: string;
}
