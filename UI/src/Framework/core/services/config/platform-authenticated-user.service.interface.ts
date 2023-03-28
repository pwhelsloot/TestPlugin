
import { Observable } from 'rxjs';

export interface IPlatformAuthenticatedserService {

    initialised$: Observable<void>;

    navigateToHomeAfterCoreAppLogin(): void;
}
