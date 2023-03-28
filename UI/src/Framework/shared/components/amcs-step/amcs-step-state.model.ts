export class AmcsStepState {
    active: boolean;
    error: boolean;
    completed: boolean;

    constructor() {
        this.active = false;
        this.error = false;
        this.completed = false;
    }
}
