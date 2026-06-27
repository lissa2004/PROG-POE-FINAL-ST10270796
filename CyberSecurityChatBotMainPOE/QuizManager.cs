using System.Collections.Generic;
using System.Windows;

namespace CyberSecurityChatBotMainPOE
{
    public class QuizManager  
    {
        private List<QuizQuestion> quizQuestions;
        private int currentQuestion;
        private int score;
        private bool questionAnswered = false;

        public QuizManager()
        {
            quizQuestions = new List<QuizQuestion>();

            LoadQuizQuestions();
        }

        private void LoadQuizQuestions()
        {
            quizQuestions.Clear();

            quizQuestions.Add(new QuizQuestion
            {
                Question = "What should you do if you receive an email asking for your password?",

                Options = new List<string>
        {
            "A) Reply with your password",
            "B) Delete the email",
            "C) Report it as phishing",
            "D) Ignore it"
        },

                CorrectAnswer = "C",

                Explanation = "Reporting phishing emails helps stop cybercriminals."
            });


            quizQuestions.Add(new QuizQuestion
            {
                Question = "True or False: You should reuse the same password on every website.",
                Options = new List<string> { "True", "False" },
                CorrectAnswer = "False",
                Explanation = "Use a unique password for every account."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "Which password is the strongest?",
                Options = new List<string>
        {
            "A) password123",
            "B) John1999",
            "C) P@ssw0rd!",
            "D) T8#kL!92@mQ"
        },
                CorrectAnswer = "D",
                Explanation = "Long, random passwords are much harder to crack."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "True or False: HTTPS websites are generally safer than HTTP websites.",
                Options = new List<string> { "True", "False" },
                CorrectAnswer = "True",
                Explanation = "HTTPS encrypts communication between your browser and the website."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "What is phishing?",
                Options = new List<string>
        {
            "A) Fishing online",
            "B) Tricking users into revealing sensitive information",
            "C) Updating antivirus",
            "D) Creating passwords"
        },
                CorrectAnswer = "B",
                Explanation = "Phishing tricks people into giving away confidential information."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "True or False: Two-factor authentication improves account security.",
                Options = new List<string> { "True", "False" },
                CorrectAnswer = "True",
                Explanation = "It adds an extra layer of protection."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "Which of these is an example of social engineering?",
                Options = new List<string>
        {
            "A) Firewall",
            "B) Fake phone call pretending to be IT",
            "C) Antivirus",
            "D) VPN"
        },
                CorrectAnswer = "B",
                Explanation = "Social engineering manipulates people rather than computers."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "True or False: You should install software only from trusted sources.",
                Options = new List<string> { "True", "False" },
                CorrectAnswer = "True",
                Explanation = "Trusted sources reduce the risk of malware."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "What should you do if you suspect malware?",
                Options = new List<string>
        {
            "A) Ignore it",
            "B) Run antivirus software",
            "C) Share the file",
            "D) Disable updates"
        },
                CorrectAnswer = "B",
                Explanation = "Scan your device immediately with trusted antivirus software."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "True or False: Public Wi-Fi is always secure.",
                Options = new List<string> { "True", "False" },
                CorrectAnswer = "False",
                Explanation = "Public Wi-Fi can expose your data if not protected."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "What is the safest action when receiving an unknown attachment?",
                Options = new List<string>
        {
            "A) Open it immediately",
            "B) Delete or verify the sender first",
            "C) Forward it",
            "D) Ignore antivirus warnings"
        },
                CorrectAnswer = "B",
                Explanation = "Always verify the sender before opening attachments."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "True or False: Keeping your operating system updated improves security.",
                Options = new List<string> { "True", "False" },
                CorrectAnswer = "True",
                Explanation = "Updates often fix important security vulnerabilities."
            });
        }

        

        public void StartQuiz()
        {
            currentQuestion = 0;
            score = 0;
            questionAnswered = false;
        }

        public bool SubmitAnswer(string answer)
        {
            if (questionAnswered)
                return false;

            questionAnswered = true;

            var question = GetCurrentQuestion();
            if (question == null)
                return false;

            string userAnswer = answer.Trim().ToUpper();
            string correctAnswer = question.CorrectAnswer.Trim().ToUpper();

            bool correct = userAnswer == correctAnswer;

            if (correct)
                score++;

            return correct;
        }


        public bool NextQuestion()
        {
            currentQuestion++;
            questionAnswered = false;

            return currentQuestion < quizQuestions.Count;
        }

        public QuizQuestion GetCurrentQuestion()
        {
            if (currentQuestion >= quizQuestions.Count)
                return null;

            return quizQuestions[currentQuestion];
        }

        public void ResetQuiz()
        {
            currentQuestion = 0;
            score = 0;
            questionAnswered = false;
        }

        public string GetExplanation()
        {
            return quizQuestions[currentQuestion].Explanation;
        }

        public int CurrentQuestionNumber
        {
            get { return currentQuestion + 1; }
        }

        public int TotalQuestions
        {
            get { return quizQuestions.Count; }
        }

        public int Score
        {
            get { return score; }
        }
    }
}
