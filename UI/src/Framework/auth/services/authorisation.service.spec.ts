// import { HttpClient } from '@angular/common/http';
// import { AuthorisationClaimNames } from '@auth-module/models/authorisation-claim-names.constants';
// import { AuthorisationClaim } from '@auth-module/models/authorisation-claim.model';
// import { AuthorisationService } from '@auth-module/services/authorisation.service';
// import { ApiRequest } from '@coremodels/api/api-request.model';
// import { ErpApiService } from '@coreservices/erp-api.service';
// import { ErpSaveService } from '@coreservices/erp-save.service';
// import { ErrorNotificationService } from '@coreservices/error-notification.service';
// import { Observable } from 'rxjs';

// describe('AuthorisationService', () => {
//     let service: AuthorisationService;

//     beforeEach(() => {
//         const claims: AuthorisationClaim[] = [
//             { description: AuthorisationClaimNames.customerFeature, hasAccess: false },
//             { description: AuthorisationClaimNames.routingFeature, hasAccess: true }
//         ];

//         const mockErpApi = new ErpApiService(<ErpSaveService>{}, <HttpClient>{}, <ErrorNotificationService>{});
//         mockErpApi.getRequestHandleError = <T>(apiRequest: ApiRequest, mapFunction: (result: T) => any, displayError: boolean, handleErrorFunction?: ((error: any) => any)) => {
//             return Observable.create(observer => {
//                 observer.next(claims);
//                 observer.complete();
//             });
//         };

//         service = new AuthorisationService(mockErpApi as ErpApiService);
//     });

//     it('should have no claims list when initialised', () => {
//         expect(service.authorisationClaims == null);
//     });

//     it('should have a populated claims list when setAuthorisationClaims() called', (done: DoneFn) => {
//         service.setAuthorisationClaims();
//         service.authorisationClaims.subscribe(
//             claims => {
//                 expect(claims.length > 0);
//                 done();
//             });
//     });

//     it('should have a access to routing feature but not to customer feature when hasAuthorisation() called ', (done: DoneFn) => {
//         service.setAuthorisationClaims();
//         service.authorisationClaims.subscribe(
//             claims => {
//                 expect(service.hasAuthorisation(AuthorisationClaimNames.routingFeature));
//                 expect(!service.hasAuthorisation(AuthorisationClaimNames.customerFeature));
//                 done();
//             });
//     });
// });
