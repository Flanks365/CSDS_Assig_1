namespace CSDS_Assign_1
{
    public class Question
    {
        // Properties
        public string Id { get; private set; }
        public string Text { get; private set; }
        public string MediaType { get; private set; }
        public byte[] MediaContent { get; private set; }
        public string MediaPreview { get; private set; }
        public string CategoryId { get; private set; }

        // Constructor to initialize properties
        public Question(string id, string text, string mediaType, byte[] mediaContent, string mediaPreview, string categoryId)
        {
            Id = id;
            Text = text;
            MediaType = mediaType;
            MediaContent = mediaContent;
            MediaPreview = mediaPreview;
            CategoryId = categoryId;
        }
    }
}
