import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated To be deleted
 */
export class FollowupStatusLookup {
    @alias('CommunicationFollowupStatusId')
    id: number;

    @alias('Description')
    description: string;
}
