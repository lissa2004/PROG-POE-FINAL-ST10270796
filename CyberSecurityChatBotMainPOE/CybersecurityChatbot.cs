using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CyberSecurityChatBotMainPOE
{
    public class CybersecurityChatbot
    {
        private Random random = new Random();
        private string userName = "";
        private string userInterest = "";
        private bool waitingForReminder = false;
        private string lastTaskTitle = "";
        private string lastTopic = "";
        private bool _topicExplained = false;
        private int _conversationCount = 0;
        private bool _taskAddedRecently = false; 

        private Dictionary<string, List<string>> tips = new Dictionary<string, List<string>>()
        {
            {
                "phishing", new List<string>() {
                    "Be cautious of emails asking for personal information.",
                    "Always verify the sender before clicking links.",
                    "Do not open suspicious attachments.",
                    "Look out for urgent or threatening language in emails."
                }
            },
            {
                "password", new List<string>() {
                    "Use at least 12 characters with symbols and numbers.",
                    "Never reuse passwords across accounts.",
                    "Enable two-factor authentication.",
                    "Avoid using personal details in passwords."
                }
            },
            {
                "browsing", new List<string>() {
                    "Only use HTTPS websites for sensitive data.",
                    "Avoid downloading unknown files.",
                    "Keep your browser updated.",
                    "Do not enter personal info on untrusted sites."
                }
            }
        };

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please type something so I can help you.";

            input = input.ToLower();
            _conversationCount++;
            string sentiment = DetectSentiment(input);

            // NAME CAPTURE
            if (input.Contains("my name is"))
            {
                userName = input.Replace("my name is", "").Trim();
                return $"Nice to meet you, {userName}! How can I help you with cybersecurity today?";
            }

            // INTEREST CAPTURE 
            if (input.Contains("i'm interested in") || input.Contains("im interested in"))
            {
                userInterest = input.Replace("i'm interested in", "").Replace("im interested in", "").Trim();
                return $"Great! I'll remember that you're interested in {userInterest}. It's an important part of cybersecurity.";
            }

            // ADD TASK (via chatbot) 
            if (input.Contains("add task"))
            {
                lastTaskTitle = input.Replace("add task", "").Trim();
                if (string.IsNullOrWhiteSpace(lastTaskTitle))
                    return "Please specify what task you'd like to add. Example: 'add task enable 2FA'";

                bool saved = SaveTaskToDatabase(lastTaskTitle);
                if (saved)
                {
                    waitingForReminder = true;
                    _taskAddedRecently = true; 
                    return $"✅ Task added with the description '{lastTaskTitle}'. Would you like a reminder? (Type 'yes' or 'no')";
                }
                else
                {
                    return "❌ Sorry, I couldn't save your task. Please check your database connection.";
                }
            }

            // REMINDER 
            if (input.Contains("yes") && waitingForReminder)
            {
                waitingForReminder = false;
                _taskAddedRecently = true;   
                return "✅ Got it! I'll remind you in 3 days.\n\n📋 You can type 'show activity log' to see your recent actions.\n🧠 Or type 'start quiz' to test your cybersecurity knowledge!";
            }
            if (input.Contains("no") && waitingForReminder)
            {
                waitingForReminder = false;
                _taskAddedRecently = true;
                return "Okay, I won't set a reminder for that task.\n\n📋 You can type 'show activity log' to see your recent actions.\n🧠 Or type 'start quiz' to test your cybersecurity knowledge!";
            }

            //TOPIC: PHISHING
            if (input.Contains("phishing"))
            {
                lastTopic = "phishing";
                _topicExplained = true;
                return BuildPhishingResponse(sentiment);
            }

            //TOPIC: PASSWORD
            if (input.Contains("password"))
            {
                lastTopic = "password";
                _topicExplained = true;
                return BuildPasswordResponse(sentiment);
            }

            //TOPIC: SAFE BROWSING
            if (input.Contains("safe browsing") || input.Contains("browsing"))
            {
                lastTopic = "browsing";
                _topicExplained = true;
                return BuildBrowsingResponse(sentiment);
            }

            //TOPIC: HACKING
            if (input.Contains("hack"))
            {
                lastTopic = "hacking";
                _topicExplained = true;
                return BuildHackingResponse(sentiment);
            }

            //FOLLOW-UP: TELL ME MORE
            if (input.Contains("tell me more") || input.Contains("explain more"))
            {
                _topicExplained = true;
                return BuildMoreInfoResponse();
            }

            //TIP
            if (input.Contains("give me a tip") || input.Contains("tip") || input.Contains("another tip"))
            {
                return BuildTipResponse();
            }

            //ADVICE
            if (input.Contains("advice") || input.Contains("recommend"))
            {
                return BuildAdviceResponse();
            }

            //HELP
            if (input.Contains("help") || input.Contains("what can i ask"))
            {
                return GetHelpMenu();
            }

            //GOODBYE
            if (input.Contains("goodbye") || input.Contains("bye") || input.Contains("exit"))
            {
                return "Goodbye! Stay safe online! 👋";
            }

            //DEFAULT
            return "I didn't quite understand that. Try asking about cybersecurity topics like phishing, passwords, or safe browsing. Type 'help' for a list of commands.";
        }

        //Build Response Methods

        private string BuildPhishingResponse(string sentiment)
        {
            string baseResponse = "Phishing is a cyberattack where scammers trick you into giving sensitive information.";
            return BuildResponse(baseResponse, sentiment) +
                "\n\n💡 Would you like to add a task to review your email security settings? (Type 'add task review email security' or say 'no thanks')";
        }

        private string BuildPasswordResponse(string sentiment)
        {
            string baseResponse = "Strong passwords protect your accounts from attackers.";
            return BuildResponse(baseResponse, sentiment) +
                "\n\n💡 Would you like to add a task to update your passwords? (Type 'add task update passwords' or say 'no thanks')";
        }

        private string BuildBrowsingResponse(string sentiment)
        {
            string baseResponse = "Safe browsing helps protect you from malicious websites.";
            return BuildResponse(baseResponse, sentiment) +
                "\n\n💡 Would you like to add a task to review your browser security settings? (Type 'add task review browser security' or say 'no thanks')";
        }

        private string BuildHackingResponse(string sentiment)
        {
            string baseResponse = "Hacking involves exploiting system vulnerabilities.";
            return BuildResponse(baseResponse, sentiment) +
                "\n\n💡 Would you like to add a task to review your account security? (Type 'add task review account security' or say 'no thanks')";
        }

        private string BuildMoreInfoResponse()
        {
            string response = "";

            if (lastTopic == "phishing")
                response = "Phishing scams often use fake emails, websites, and urgent messages to trick users into revealing passwords or banking information. Always verify the sender before clicking links.";
            else if (lastTopic == "password")
                response = "Weak passwords are easy for hackers to guess. Use at least 12 characters with a mix of uppercase, lowercase, numbers, and symbols. Never reuse passwords across accounts.";
            else if (lastTopic == "browsing")
                response = "Unsafe browsing can expose users to malware, fake websites, and online scams. Always use HTTPS websites and avoid downloading unknown files.";
            else if (lastTopic == "hacking")
                response = "Hackers use various techniques like malware, phishing, and social engineering to gain unauthorized access. Regular software updates and strong passwords help protect against attacks.";
            else
                return "Please ask about a specific topic first (like phishing, passwords, or safe browsing) so I can give you more information.";

            return response +
                "\n\n💡 Would you like to add a task to improve your cybersecurity? (Type 'add task [description]' or say 'no thanks')";
        }

        private string BuildTipResponse()
        {
            if (tips.ContainsKey(lastTopic))
            {
                string tip = GetRandom(tips[lastTopic]);
                return tip +
                    "\n\n💡 Would you like to add a task based on this tip? (Type 'add task [description]' or say 'no thanks')";
            }
            return "Tell me a topic first (like phishing, passwords, or safe browsing) so I can give you a tip.";
        }

        private string BuildAdviceResponse()
        {
            if (!string.IsNullOrEmpty(userInterest))
            {
                return $"Since you're interested in {userInterest}, " +
                       $"you should regularly review your account privacy and security settings to always stay on the safe side." +
                       $"\n\n💡 Would you like to add a task to review your {userInterest} settings? (Type 'add task review {userInterest} settings' or say 'no thanks')";
            }
            return "Tell me what you're interested in first. Say 'I'm interested in [topic]' and I'll give you personalized advice.";
        }

        

        //Helper Methods

        private string GetHelpMenu()
        {
            return @"📋 **Available Commands:**

🔹 **Cybersecurity Topics:**
   - 'what is phishing' - Learn about phishing
   - 'what is password safety' - Learn about passwords
   - 'what is safe browsing' - Learn about browsing safety
   - 'what is hacking' - Learn about hacking

🔹 **Task Management:**
   - 'add task [description]' - Add a new task
   - 'yes' - Confirm reminder (after adding task)
   - 'no' - Decline reminder (after adding task)

🔹 **Quiz & Activity:**
   - 'start quiz' - Test your cybersecurity knowledge
   - 'show activity log' - View your recent actions

🔹 **Additional Features:**
   - 'tell me more' - Get more info about current topic
   - 'give me a tip' - Get a cybersecurity tip
   - 'another tip' - Get another tip
   - 'advice' - Get personalized advice

🔹 **Other:**
   - 'my name is [name]' - Set your name
   - 'I'm interested in [topic]' - Set your interest
   - 'help' - Show this menu
   - 'goodbye' - Exit the chatbot";
        }

        private string GetRandom(List<string> list)
        {
            return list[random.Next(list.Count)];
        }

        private string DetectSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("scared"))
                return "worried";
            if (input.Contains("frustrated") || input.Contains("annoyed"))
                return "frustrated";
            if (input.Contains("curious"))
                return "curious";
            return "";
        }

        private string BuildResponse(string baseResponse, string sentiment)
        {
            string emotionalLayer = "";

            if (sentiment == "worried")
                emotionalLayer = "It's completely understandable to feel that way. ";
            else if (sentiment == "frustrated")
                emotionalLayer = "Don't worry, cybersecurity can be confusing at first. ";
            else if (sentiment == "curious")
                emotionalLayer = "That's a great question! ";

            string tip = "";
            if (tips.ContainsKey(lastTopic))
                tip = "\n\n💡 TIP: " + GetRandom(tips[lastTopic]);

            return emotionalLayer + baseResponse + tip;
        }

        public static bool SaveTaskToDatabase(string title)
        {
            string connectionString =
                "server=localhost;database=CyberSecurityChatBotMainPOE;uid=root;pwd=kamogelomathikge@2004;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query =
                        "INSERT INTO tasks (title, description, reminder_date, status) " +
                        "VALUES (@title, @desc, @date, @status)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@desc", "Added via chatbot");
                    cmd.Parameters.AddWithValue("@date", DateTime.Now.AddDays(3));
                    cmd.Parameters.AddWithValue("@status", "Pending");

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving task: {ex.Message}");
                return false;
            }
        }
    }
}