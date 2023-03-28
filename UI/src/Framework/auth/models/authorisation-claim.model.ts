import { alias } from '@coreconfig/api-dto-alias.function';

export class AuthorisationClaim {
    @alias('Description')
    description: string;

    @alias('HasAccess')
    hasAccess: boolean;
}
