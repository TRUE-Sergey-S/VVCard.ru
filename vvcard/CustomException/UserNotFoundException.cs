using System;

namespace vvcard.CustomException
{
    public class UserNotFoundException: Exception
    {
        public UserNotFoundException(string userName):base($"User: {userName} not found in database")
        {
        }
    }
}