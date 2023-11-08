﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domains
{
    public class BaseResponseModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public object Result { get; set; }
        public BaseResponseModel(object result, bool isSuccess = true, int statusCode = 200, string message = "")
        {
            Result = result;
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Message = message;
        }
    }
}
