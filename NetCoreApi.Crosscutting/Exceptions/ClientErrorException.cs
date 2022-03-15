﻿using System;
using System.Net;

namespace NetCoreApi.Crosscutting.Exceptions
{
    public class ClientErrorException : Exception
    {
        public int StatusCode { get; set; }
        public ClientErrorException(string message, int statusCode = (int)HttpStatusCode.BadRequest) : base(message)
        {
            StatusCode = statusCode; 
        }
    }
}
