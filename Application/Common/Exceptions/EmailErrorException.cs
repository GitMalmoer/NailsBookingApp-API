﻿namespace Application.Common.Exceptions
{
    public class EmailErrorException :Exception
    {
        public EmailErrorException(string message ) : base(message)
        {
            
        }
    }
}
