using System;
using System.Collections.Generic;
using System.Text;

namespace App.Components.Utilities.ErrorHandling
{
    public class InvalidRequestException: Exception
    {
        public InvalidRequestException(): base()
        {
            this.LogAsInfo().MarkAsClientException();
        }
        public InvalidRequestException(string message) :base(message)
        {
            this.LogAsInfo().MarkAsClientException();
        }
    }
}
