namespace NailsBookingApp_API.Exceptions
{
    public class EmailErrorException :Exception
    {
        public EmailErrorException(string message ) : base(message)
        {
            
        }
    }
}
