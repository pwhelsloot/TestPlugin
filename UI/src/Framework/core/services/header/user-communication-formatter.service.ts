import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { UserCommunication } from '@coremodels/header/user-communication.model';

/**
 * @deprecated Move to PlatformUI
 */
export class UserCommunicationFormatter {
    static formatUserCommunication(userCommunications: UserCommunication[]) {
        if (userCommunications !== null && userCommunications.length > 0) {
            userCommunications.forEach(data => {
                if (data.reviewDate !== null) {
                    const reviewDate = data.reviewDate;
                    reviewDate.setHours(0, 0, 0, 0);

                    const today = AmcsDate.create();
                    today.setHours(0, 0, 0, 0);

                    data.overdue = reviewDate < today;
                }

                data.fullName = `${data.forename} ${data.surname}`;

                if (data.notes !== null && data.notes.length > 100) {
                    data.notes = `${data.notes.substring(0, 99)} ...`;
                }
            });
        }
    }
}
