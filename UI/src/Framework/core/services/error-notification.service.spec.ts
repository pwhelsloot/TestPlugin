import { ErrorNotificationService } from './error-notification.service';

describe('ErrorNotificationService', () => {
  it('has expected defaults', () => {
    const testService = new ErrorNotificationService();
    expect(testService.errors$).toBeDefined();
  });

  it('pushes error', () => {
    const expectedError = 'Something went poof';
    const testService = new ErrorNotificationService();

    testService.errors$.subscribe((errors) => {
      expect(errors).toEqual(expectedError);
    });
    testService.notifyError(expectedError);
  });
});
