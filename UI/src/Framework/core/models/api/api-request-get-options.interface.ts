export interface IApiRequestGetOptions {
  searchTerms?: string[];
  max?: number;
  includeCount?: boolean;
  includeDeleted?: boolean;
  page?: number;
  suppressErrorModal?: boolean;
  isCoreRequest?: boolean;
}
