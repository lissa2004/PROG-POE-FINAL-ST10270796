using System.Collections.Generic;

namespace CyberSecurityChatBotMainPOE
{
    public class QuizQuestion
    {
        public string Question { get; set; }

        public List<string> Options { get; set; }

        public string CorrectAnswer { get; set; }

        public string Explanation { get; set; }


        // Default constructor (parameterless)
        public QuizQuestion() { }


        public QuizQuestion(string question, List<string> options, string correctAnswer, string explanation)
        {
            Question = question;
            Options = options;
            CorrectAnswer = correctAnswer;
            Explanation = explanation;
        }
    }
}
