import { IAmcsStepService } from './amcs-step.interface.service';

export interface IAmcsStep {
    // Must be unique over all steps in stepper, used to track linked steps
    id: number;

    linkedIds?: number[];

    heading?: string;

    subHeading?: string;

    formService?: IAmcsStepService;
}
