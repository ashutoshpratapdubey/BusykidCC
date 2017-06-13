using System;

namespace LeapSpring.MJC.Core.Filters
{
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException(string message)
        : base(message)
        {
        }

    }
}