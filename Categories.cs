namespace CSDS_Assign_1
{
    public class Category(String name, String imageType, byte[] image)
    {
        private readonly String name = name;
        private readonly String imageType = imageType;
        private readonly byte[] image = image;

        public String GetName()
        {
            return name;
        }

        public String GetImageType()
        {
            return imageType;
        }

        public byte[] GetImage()
        {
            return image;
        }
    }
}
