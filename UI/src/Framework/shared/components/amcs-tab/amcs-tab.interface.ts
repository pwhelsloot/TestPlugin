import { IAmcsTabService } from './amcs-tab.interface.service';

export interface IAmcsTab {
    // Must be unique over all tabs in tab control, used to track linked tabs
    id: number;

    linkedIds?: number[];

    heading: string;

    iconClass?: string;

    formService?: IAmcsTabService;

}
