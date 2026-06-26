using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;


namespace CyberSecurityChatBotMainPOE
{
    // Main chatbot logic class
    class CybersecurityChatbot
    {

        // Random generator for random responses
        private static Random random = new Random();
        private static string userName = "";
        private static string userInterest = "";
        private static bool waitingForReminder = false;
        private static string lastTaskTitle = "";
        private static string lastTopic = "";


        //Dictionary storing cybersecurity tips
        private static Dictionary<string, List<string>> tips = new Dictionary<string, List<string>>(){
            {
                // Phishing tips array 
                "phishing", new List<string>() {
                    "Be cautious of emails asking for personal information.",
                    "Always verify the sender before clicking links.",
                    "Do not open suspicious attachments.",
                    "Look out for urgent or threatening language in emails."
                }
            },
            {
                // Password safety tips array
                "password", new List<string>() {
                    "Use at least 12 characters with symbols and numbers.",
                    "Never reuse passwords across accounts.",
                    "Enable two-factor authentication.",
                    "Avoid using personal details in passwords."
                }
            },
            {
                // Safe browsing tips array
                "browsing", new List<string>() {
                    "Only use HTTPS websites for sensitive data.",
                    "Avoid downloading unknown files.",
                    "Keep your browser updated.",
                    "Do not enter personal info on untrusted sites."
                }
            }
        };

        // Method returns a random tip from a list
        private static string GetRandom(List<string> list)
        {
            return list[random.Next(list.Count)];
        }

        // Method detects user emotion/sentiment
        private static string DetectSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("scared"))
                return "worried";

            if (input.Contains("frustrated") || input.Contains("annoyed"))
                return "frustrated";

            if (input.Contains("curious"))
                return "curious";

            return "";
        }

        //Part 3 POE: Task 1 Saving Task in the database method

        public static bool SaveTaskToDatabase(string title) {
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

        // Main chatbot response method for Part 3 and Part 2 
        public static string GetResponse(string input)
        {
            {
                if (string.IsNullOrWhiteSpace(input))
                    return "Please type something so I can help you.";

                input = input.ToLower();

                string sentiment = DetectSentiment(input);

                if (input.Contains("add task"))
                {

                    lastTaskTitle = input.Replace("add task", "").Trim();

                    SaveTaskToDatabase(lastTaskTitle);

                    waitingForReminder = true;

                    return $"Task added with the description '{lastTaskTitle}'. Would you like a reminder?";
                }



                if (input.Contains("yes") && waitingForReminder)
                {

                    waitingForReminder = false;

                    return "Got it! I'll remind you in 3 days.";
                }

                if (input.Contains("my name is"))
                {
                    userName = input.Replace("my name is", "").Trim();

                    return $"Nice to meet you, {userName}! How can I help you with cybersecurity today?";
                }


                if (input.Contains("i'm interested in"))
                {

                    userInterest = input.Replace("i'm interested in", "").Trim();

                    return $"Great! I'll remember that you're interested in {userInterest}. " +
                           $"It's an important part of cybersecurity.";
                }


                if (input.Contains("phishing"))
                {
                    lastTopic = "phishing";
                    return BuildResponse("Phishing is a cyberattack where scammers trick you into giving sensitive information.", sentiment);
                }


                if (input.Contains("password"))
                {
                    lastTopic = "password";
                    return BuildResponse("Strong passwords protect your accounts from attackers.", sentiment);
                }


                if (input.Contains("safe browsing"))
                {
                    lastTopic = "browsing";
                    return BuildResponse("Safe browsing helps protect you from malicious websites.", sentiment);
                }


                if (input.Contains("hack"))
                {
                    lastTopic = "hacking";
                    return BuildResponse("Hacking involves exploiting system vulnerabilities.", sentiment);
                }

                // Follow-up conversation flow
                if (input.Contains("tell me more") || input.Contains("explain more"))
                {

                    // Continue phishing conversation
                    if (lastTopic == "phishing")
                    {

                        return "Phishing scams often use fake emails, websites, and urgent messages to trick users into revealing passwords or banking information.";
                    }

                    // Continue password conversation
                    if (lastTopic == "password")
                    {

                        return "Weak passwords are easy for hackers to guess. Strong passwords reduce the risk of cyberattacks.";
                    }

                    // Continue browsing conversation
                    if (lastTopic == "browsing")
                    {

                        return "Unsafe browsing can expose users to malware, fake websites, and online scams.";
                    }

                    return "Please ask about the following cybersecurity Topics e.g. Phishing, password and browsing";
                }

                if (input.Contains("give me a tip") || input.Contains("tip") || input.Contains("another tip"))
                {
                    if (tips.ContainsKey(lastTopic))
                        return GetRandom(tips[lastTopic]);

                    return "Tell me a topic first (like phishing or password safety) so I can give you a tip.";
                }

                // Personalised recall feature
                if (input.Contains("advice") || input.Contains("recommend"))
                {

                    if (!string.IsNullOrEmpty(userInterest))
                    {

                        return $"Since you're interested in {userInterest}, " +
                               $"you should regularly review your account privacy and security settings, to allows be on the safe side";
                    }
                }

                // Help menu response
                if (input.Contains("help") || input.Contains("what can i ask"))
                {
                    return @"You can ask me about:

- Phishing
- Password safety
- Safe browsing
- Hacking

Then ask:
- Give me a tip
- Another tip
- Tell me more
- Explain more";
                }


                // Default chatbot response
                return "Please repeat the question again I don't think I understand, also try rephrasing or ask about cybersecurity topics like phishing or passwords?";
            }
        }


        // Builds chatbot responses with emotion + tips
        private static string BuildResponse(string baseResponse, string sentiment)
        {
            string emotionalLayer = "";

            if (sentiment == "worried")
            {
                emotionalLayer = "It's completely understandable to feel that way. ";
            }
            else if (sentiment == "frustrated")
            {
                emotionalLayer = "Don't worry, cybersecurity can be confusing at first. ";
            }
            else if (sentiment == "curious")
            {
                emotionalLayer = "That's a great question! ";
            }

            string tip = "";

            // Check if tips exist for current topic
            if (tips.ContainsKey(lastTopic))
            {
                tip = "\n\n NOTE THAT: " + GetRandom(tips[lastTopic]);
            }

            return emotionalLayer + baseResponse + tip;
        }
    }

}
