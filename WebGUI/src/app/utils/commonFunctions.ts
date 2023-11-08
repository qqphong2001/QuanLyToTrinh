import { environment } from 'src/environments/environments';
import * as constant from './constants';

export const GetCurrentUserId = () => {
    const userInfo = localStorage.getItem('UserInfo');
    if(!!userInfo) return JSON.parse(userInfo).userId;
    return 0;
}

export const GetCurrentUserInfo = () => {
    const userInfo = localStorage.getItem('UserInfo');
    if(!!userInfo) return JSON.parse(userInfo);
    return 0;
}

export interface RoleInfo {
    admin: boolean;
    specialist: boolean;
    generalSpecialist: boolean;
    approver: boolean;
    generalApprover:boolean;        
}
export const GetRoleInfo = () => {
    const userInfo = GetCurrentUserInfo();
    if(userInfo != 0){
        const isAdmin = userInfo.roles.includes(constant.ROLES.ADMIN);
        const isGeneralApprover = userInfo.roles.includes(constant.ROLES.GENERAL_APPROVER);
        const roleInfo: RoleInfo = {
            admin: isAdmin,
            specialist: userInfo.roles.includes(constant.ROLES.SPECIALIST) || isAdmin,
            generalSpecialist: userInfo.roles.includes(constant.ROLES.GENERAL_SPECIALIST) || isAdmin || isGeneralApprover,
            approver: userInfo.roles.includes(constant.ROLES.APPROVER) || isAdmin,
            generalApprover: isGeneralApprover || isAdmin
        }               
        return roleInfo;
    }
    return {
        admin: false,
        specialist: false,
        generalSpecialist: false,
        approver: false,
        generalApprover: false
    } as RoleInfo;
}

export const GetFullFilePath = (relativePath: string) => {    
    return environment.hostUrl + relativePath;              
}