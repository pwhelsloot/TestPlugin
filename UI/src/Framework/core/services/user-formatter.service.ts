import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { UserLookup } from '@coremodels/lookups/user-lookup.model';

/**
 * @deprecated Move to PlatformUI
 */
export class UserFormatter {

    static formatUsers(users: UserLookup[]) {
        if (users != null && users.length > 0) {
            users.forEach(element => {
                element.description = element.forename + ' ' + element.surname;
            });
            users.sort(function(a, b) {
                return (isTruthy(a.description) ? a.description : '').localeCompare(isTruthy(b.description) ? b.description : '', undefined, { sensitivity: 'accent' });
            });
        }
    }
}
