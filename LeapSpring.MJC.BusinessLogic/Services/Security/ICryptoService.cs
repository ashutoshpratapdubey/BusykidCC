namespace LeapSpring.MJC.BusinessLogic.Services.Security
{
    public interface ICryptoService
    {
        /// <summary>
        /// Generate salt for encryption.
        /// </summary>
        /// <returns>The password salt.</returns>
        string GenerateSalt();

        /// <summary>
        /// Encrypts the master password
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <param name="salt">The password salt.</param>
        /// <returns>The encrypted password.</returns>
        string EncryptPassword(string email, string password, string passwordSalt);
    }
}
