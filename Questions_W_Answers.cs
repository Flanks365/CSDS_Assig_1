namespace CSDS_Assign_1
{
    public class Questions_W_Answers
    {
        public string QuestId { get; set; }  // Make setters public
        public string Corr { get; set; }
        public string Dec1 { get; set; }
        public string Dec2 { get; set; }
        public string Dec3 { get; set; }
        public string MediaTyp { get; set; }
        public string MediaPrev { get; set; }
        public string Question { get; set; }

        // Constructor remains the same
        public Questions_W_Answers(string questId, string corr, string dec1, string dec2, string dec3, string mediaTyp,
            string mediaPrev, string question) =>
            (QuestId, Corr, Dec1, Dec2, Dec3, MediaTyp, MediaPrev, Question) =
            (questId, corr, dec1, dec2, dec3, mediaTyp, mediaPrev, question);
    }


}