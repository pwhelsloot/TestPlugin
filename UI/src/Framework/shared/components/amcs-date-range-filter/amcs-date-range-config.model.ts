import { AmcsDateRangeConfigFilter, AmcsDateRangeConfigFilterType } from './amcs-date-range-config-filter.model';

export class AmcsDateRangeConfig {
  hasCustom = false;

  constructor(public filters: AmcsDateRangeConfigFilter[]) {
    this.hasCustom = this.filters.some(x => x.type === AmcsDateRangeConfigFilterType.custom);
  }

  static getDefaultOperationsConfig(): AmcsDateRangeConfig {
    return new AmcsDateRangeConfig([
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.today),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.today, -1),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.todayRange, -7, 0),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.todayRange, 0, 7),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.todayRange, -30, 0),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.month),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.month, -1),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.custom)
    ]);
  }

  static getDefaultScheduleConfig(): AmcsDateRangeConfig {
    return new AmcsDateRangeConfig([
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.date),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.dateRange, 0, 7),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.dateRange, 0, 30)
    ]);
  }

  static getDefaultCollectionHistoryConfig(): AmcsDateRangeConfig {
    return new AmcsDateRangeConfig([
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.todayRange, -90, 0),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.custom)
    ]);
  }

  static getDefaultDemandPlanConfig(): AmcsDateRangeConfig {
    return new AmcsDateRangeConfig([
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.month, -1),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.month),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.month, +1),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.custom)
    ]);
  }

  static getDefaultSalesOrderConfig(): AmcsDateRangeConfig {
    return new AmcsDateRangeConfig([
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.month, -1),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.todayRange, -7),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.today),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.month),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.custom)
    ]);
  }

  static getDefaultAssociatedRecordsConfig(): AmcsDateRangeConfig {
    return new AmcsDateRangeConfig([
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.today),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.todayRange, -7, 0),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.todayRange, 0, 7),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.custom)
    ]);
  }

  static getDefaultServiceAgreementsConfig(): AmcsDateRangeConfig {
    return new AmcsDateRangeConfig([
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.custom)
    ]);
  }

  static getDefaultApprovalsConfig(): AmcsDateRangeConfig {
    return new AmcsDateRangeConfig([
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.today),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.today, -1),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.todayRange, -7, 0),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.todayRange, 0, 7),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.todayRange, -30, 0),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.month),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.month, -1),
      new AmcsDateRangeConfigFilter(AmcsDateRangeConfigFilterType.custom)
    ]);
  }
}
