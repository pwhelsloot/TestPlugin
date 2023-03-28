import { Observable } from 'rxjs';

export interface IPlatformInitialisationService {

    initialised$: Observable<void>;
}
