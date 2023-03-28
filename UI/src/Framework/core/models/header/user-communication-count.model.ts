import { alias } from '../../config/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
export class UserCommunicationCount {
    @alias('CommunicationsCount')
    communicationsCount: number;
}
