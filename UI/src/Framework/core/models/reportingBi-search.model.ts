import { alias } from '@coreconfig/api-dto-alias.function';
import { ReportParameter } from './ReportParamaters.model';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/245825
 */
export class ReportingBiSearch {

    @alias('Config')
    config: string;

    @alias('ReportFilter')
    reportFilter: string;

    @alias('ReportPath')
    reportPath: string;

    @alias('ExportType')
    exportType: string;

    reportParameters: ReportParameter[];
}
