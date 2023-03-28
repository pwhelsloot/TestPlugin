import { isTruthy } from '../../helpers/is-truthy.function';
import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ReportSpecialTypeEnum } from '../lookups/report-special-type-enum.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class ReportSearchResult extends ApiBaseModel {
    @amcsJsonMember('ReportId')
    reportId: number;

    @amcsJsonMember('Name')
    name: string;

    @amcsJsonMember('Description')
    description: string;

    @amcsJsonArrayMember('ReportLinesOfBusiness', String)
    reportLinesOfBusiness: string[];

    @amcsJsonArrayMember('ReportFilters', String)
    reportFilters: string[];

    @amcsJsonArrayMember('ReportKeywords', String)
    reportKeywords: string[];

    @amcsJsonMember('Tree')
    tree: string;

    @amcsJsonMember('AllowDelete')
    allowDelete: boolean;

    @amcsJsonMember('AllowEdit')
    allowEdit: boolean;

    @amcsJsonMember('AllowDuplicate')
    allowDuplicate: boolean;

    @amcsJsonMember('SpecialType')
    specialType: number;

    formattedFilters: string;
    formattedKeywords: string;
    folderName: string;

    init(translations: string[]) {
        if (isTruthy(this.reportFilters)) {
            this.formattedFilters = this.reportFilters.join(', ');
        }
        if (isTruthy(this.reportKeywords)) {
            this.formattedKeywords = this.reportKeywords.join(', ');
        }

        switch (this.specialType as ReportSpecialTypeEnum) {
            case ReportSpecialTypeEnum.Standard:
                this.folderName = translations['reportsearchresult.specialType.standard'];
                break;
            case ReportSpecialTypeEnum.MyReports:
                this.folderName = translations['reportsearchresult.specialType.myReports'];
                break;
            case ReportSpecialTypeEnum.Shared:
                this.folderName = translations['reports.specialType.shared'];
                break;
            case ReportSpecialTypeEnum.SSRS:
                this.folderName = translations['reportsearchresult.specialType.ssrs'];
                break;
            default:
                this.folderName = '';
                break;
        }
    }
}
