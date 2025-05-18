namespace AuthenticationAPI.Common.Utility
{
    public class ConstantsUtil
    {
        public enum HttpStatusCode
        {
            Ok = 200,
            Success = 204,
            BadRequest = 400,
            InvalidToken = 404,
            InternalServerError = 500
        }
    }
}
