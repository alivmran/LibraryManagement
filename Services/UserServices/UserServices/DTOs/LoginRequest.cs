namespace Library.UserServices.DTOs
{
    /// <summary>
    /// Payload for user login requests.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// The user's email (used as username).
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The user's plaintext password.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
