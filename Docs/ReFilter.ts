export interface PagedBase {
    pageSize: number;
    pageIndex: number;
}

export enum SortDirection {
    ASC = 0,
    DESC = 1
}

export enum OperatorComparer {
    Contains,
    StartsWith,
    EndsWith,
    Equals = 13,
    GreaterThan = 15,
    GreaterThanOrEqual = 16,
    LessThan = 20,
    LessThanOrEqual = 21,
    NotEqual = 35,
    Not = 34,
    CustomFilter = 99
}

export interface PropertyFilterConfig {
    propertyName: string;
    operatorComparer?: OperatorComparer | null;
    sortDirection?: SortDirection | null;
    value?: any;
}

export interface BasePagedRequest<T> extends PagedBase {
    /**
     * Where object for 1:1 mapping to entity to be filtered.
     * Only requirenment is that property names are same
     */
    where?: T;

    /**
     * Defines rules for sorting and filtering
     * Can be left empty and in such way, the default values are used.
     * Default values are no sort and Equals comparer
     */
    propertyFilterConfigs?: PropertyFilterConfig[];

    /**String SearchQuery meant for searching ANY of the tagged property */
    searchQuery?: string;
}

export interface PagedResultBase extends PagedBase {
    pageCount: number;
    rowCount: number;
    currentPage: number;
    firstRowOnPage: number;
    lastRowOnPage: number;
}

export interface PagedResult<V extends IBaseViewModel<number | string>> {
    results: V[];
    rowCount: number;
}
