export interface UserLogInModel{
    userName: string;
    password: string;
}

export interface UserSignUpModel{
    username: string;
    password: string;
    fullName: string;
    email: string;
    phoneNumber: string;
    roleIds: string[];
}
export  interface  GetAllInfoUserModel{
  userId : string;
  userName:string;
  userFullName:string;
  email:string;
  phoneNumber:string;
  createDate:Date;
  lastloginDate:Date;
  isApproved:boolean;
  isLockedout:boolean;
}
export interface  CreateAccount{
  username:string,
  password : string,
  fullName : string,
  email : string,
  phoneNumber : string,
  roleId : []
}
export interface TokenResponseModel{
    userName: string;
    userId: string;
    accessToken: string;
    displayName: string;
    tokenExpiration: Date;
    roles: string[];
}
