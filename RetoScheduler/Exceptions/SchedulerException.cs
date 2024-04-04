using System.Runtime.Serialization;

namespace RetoScheduler.Exceptions
{
    [Serializable]
    public class SchedulerException : Exception
    {
        public SchedulerException()
        {
        }

        public SchedulerException(string? message) : base(message)
        {
        }

        public SchedulerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SchedulerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}