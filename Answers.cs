using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace CSDS_Assign_1
{
    public class Answer
    {
        // Properties
        public string Id { get; private set; }
        public string QuestionId { get; private set; }
        public string AnswerText { get; private set; }
        public string IsCorrect { get; private set; }
        public string AnswerIndex { get; private set; }
      
        // Constructor to initialize properties
        public Answer(string id, string questionId, string answerText, string isCorrect, string answerIndex)
        {
            Id = id;
            QuestionId = questionId;
            AnswerText = answerText;
            IsCorrect = isCorrect;
            AnswerIndex = answerIndex;
        }
    }
}
