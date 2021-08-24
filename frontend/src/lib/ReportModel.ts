export interface ReportModel {
    dateChecked: Date;
    responseTime: string;
    url: string;
    hasError: boolean;
    errorMessage: string;
}