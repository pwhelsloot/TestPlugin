import { Subscription } from 'rxjs';

export class DragAndDropFromGroup {
    uuid: string;
    subscriptions: Subscription[];
    isFormGroupValid: boolean[];
}
