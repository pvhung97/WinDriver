﻿namespace UIADriver.dto.response
{
    public class Response
    {
        public object? value { get; set; }

        public Response(object? value)
        {
            this.value = value;
        }
    }
}
