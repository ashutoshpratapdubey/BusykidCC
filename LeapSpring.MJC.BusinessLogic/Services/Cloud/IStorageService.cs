using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Cloud
{
    public interface IStorageService
    {
        /// <summary>
        /// Save file.
        /// </summary>
        /// <param name="fileStream">The file bytes.</param>
        /// <param name="filename">The fime name.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The file url.</returns>
        Task<string> SaveFile(byte[] fileStream, string filename, string contentType);

        /// <summary>
        /// Delete the file.
        /// </summary>
        /// <param name="imagePath">The image path</param>
        /// <returns>None.</returns>
        Task DeleteFile(string imagePath);
    }
}
