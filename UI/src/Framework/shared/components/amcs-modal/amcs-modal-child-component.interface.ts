import { BehaviorSubject, Subject } from 'rxjs';

export interface AmcsModalChildComponent {
    extraData: any;
    loading: BehaviorSubject<boolean>;
    externalClose?: Subject<any>;
    buttonText?: string;
}
