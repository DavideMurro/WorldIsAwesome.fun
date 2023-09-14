using System;

namespace www.worldisawesome.fun.ExceptionModels
{
    public class MyNotFoundException : Exception
    {
        public MyNotFoundException()
        {
        }

        public MyNotFoundException(string message)
            : base(message)
        {
        }

        public MyNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
