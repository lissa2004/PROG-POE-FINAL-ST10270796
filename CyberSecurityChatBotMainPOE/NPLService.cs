using System;

namespace CyberSecurityChatBotMainPOE.Services
{
    public class NLPService
    {
        public enum Intent
        {
            Unknown,
            Quiz,
            Task,
            Log,
            Info,
            Exit
        }

        public Intent DetectIntent(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return Intent.Unknown;

            input = input.ToLower().Trim();

            if (input.Contains("quiz") || input.Contains("play quiz") || input.Contains("start quiz"))
                return Intent.Quiz;

            if (input.Contains("add task") || input.Contains("new task") || input.Contains("create task") ||
                input.Contains("remind me") || input.Contains("set a reminder") || input.Contains("add a reminder") ||
                input.Contains("task") || input.Contains("reminder") || input.Contains("remember"))
                return Intent.Task;

            if (input.Contains("activity log") || input.Contains("what have you done") ||
                input.Contains("show log") || input.Contains("recent actions"))
                return Intent.Log;

            if (input.Contains("bye") || input.Contains("goodbye") || input.Contains("exit") || input.Contains("quit"))
                return Intent.Exit;

            if (input.Contains("password") || input.Contains("phishing") || input.Contains("hacking") ||
                input.Contains("2fa") || input.Contains("security") || input.Contains("safe"))
                return Intent.Info;

            return Intent.Unknown;
        }

        public string ExtractTaskDescription(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            input = input.ToLower().Trim();

            string[] prefixes = { "add task", "new task", "create task", "remind me", "set a reminder", "add a reminder" };

            foreach (string prefix in prefixes)
            {
                if (input.Contains(prefix))
                {
                    int index = input.IndexOf(prefix) + prefix.Length;
                    if (index < input.Length)
                    {
                        string description = input.Substring(index).Trim();
                        if (description.StartsWith("to ")) description = description.Substring(3);
                        if (description.StartsWith("for ")) description = description.Substring(4);
                        return description;
                    }
                }
            }

            return input;
        }
    }
}
