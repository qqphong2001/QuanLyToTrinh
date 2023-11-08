import { Guid } from "guid-typescript";
import { CommentModel } from "./comment.model";
import { FieldModel } from "./field.model";
import { DocumentFileModel } from "./documentFile.model";
import { DocumentApprovalModel } from "./documentApproval.model";

export interface DocumentModel {
    id: number;
    title ?:string;
    note ?:string;
    fieldId ?: number;
    dateEndApproval ?:Date;
    statusCode ?:number;
    modified ?:Date;
    deleted ?:boolean;
    modifiedBy ?:string;
    createdBy ?:string;
    created ?:Date;
    field ?: FieldModel

    fieldName?: string;
    authorName?: string;
    comments ?: CommentModel[];
    approvals?: DocumentApprovalModel[];
    documentFiles?: DocumentFileModel[];

    approverAction?: string;
    actionClass?: string;
}