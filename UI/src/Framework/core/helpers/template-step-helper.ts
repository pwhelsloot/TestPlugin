import { ClassBuilder } from './dto/class-builder';

/**
 * @deprecated Move to PlatformUI
 */
export class TemplateStepHelper {
    static templateStepVersion = 1;

    static buildTemplateStepFromFormModel<S, D>(formModel: any, sourceType: (new () => S), destinationType: (new () => D)): D {
        const value = ClassBuilder.buildFromFormModel<S, D>(formModel, sourceType, destinationType);
        value['templateStepVersion'] = TemplateStepHelper.templateStepVersion;
        return value;
    }
}
