import { Guid } from "guid-typescript";

export interface DocumentApprovalModel {
    id?: number;
    title?: string;
    docId?: number;
    statusCode?: number;
    userId?: string;
    userName?: string;
    modified?: Date;
    deleted?: boolean;
    modifiedBy?: string;
    createdBy?: string;
    created?: Date;
    comment?: string;
}

export interface DocumentApprovalSummaryModel{
    title: string;
    status: string;
    field: string;
    submittedAt: Date;
    submitter: string;
    deadlineAt: Date;
    endAt: Date;
    approvals: string[];
    declines: string[];
    noResponses: string[];
    maxRows?: number;
    totalVote?: number;
    yayVote?: number;
    nayVote?: number;
}