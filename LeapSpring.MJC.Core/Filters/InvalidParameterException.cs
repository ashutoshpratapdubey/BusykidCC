using System;

namespace LeapSpring.MJC.Core.Filters
{
    public class InvalidParameterException : Exception
    {
        public InvalidParameterException(string message)
        : base(message)
        {
        }
    }
}