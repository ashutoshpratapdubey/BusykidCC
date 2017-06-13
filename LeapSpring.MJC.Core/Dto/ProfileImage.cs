namespace LeapSpring.MJC.Core.Dto
{
    public class ProfileImage
    {
        /// <summary>
        /// Gets or sets the base 64 image url data.
        /// </summary>
        public string Base64ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the family member identifier.
        /// </summary>
        public int FamilyMemberId { get; set; }
    }
}
