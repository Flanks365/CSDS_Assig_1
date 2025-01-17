namespace CSDS_Assign_1
{
    public class Category
    {
        // Properties
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string ImageType { get; private set; }
        public byte[] Image { get; private set; }

        // Constructor to initialize properties
        public Category(string id, string name, string imageType, byte[] image)
        {
            Id = id;
            Name = name;
            ImageType = imageType;
            Image = image;
        }
    }
}
