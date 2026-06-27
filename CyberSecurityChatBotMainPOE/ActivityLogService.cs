using System;
using System.Collections.Generic;
using CyberSecurityChatBotMainPOE.Models;

namespace CyberSecurityChatBotMainPOE.Services
{
    public class ActivityLogService
    {
        private readonly List<ActivityLogEntry> _logs = new List<ActivityLogEntry>();
        private const int MaxLogEntries = 20;

        public void AddLog(string action, string description)
        {
            _logs.Add(new ActivityLogEntry(action, description));
            if (_logs.Count > MaxLogEntries)
                _logs.RemoveAt(0);
        }

        public List<ActivityLogEntry> GetRecentLogs(int count = 10)
        {
            if (_logs.Count == 0) return new List<ActivityLogEntry>();
            int start = Math.Max(0, _logs.Count - count);
            return _logs.GetRange(start, _logs.Count - start);
        }

        public string GetLogSummary()
        {
            if (_logs.Count == 0) return "No recent activity to show.";

            string log = "📋 Here's a summary of recent actions:\n\n";
            int count = 1;
            int start = Math.Max(0, _logs.Count - 10);

            for (int i = start; i < _logs.Count; i++)
            {
                log += $"{count}. {_logs[i]}\n";
                count++;
            }

            return log;
        }

        public void LogTaskAdded(string title) => AddLog("TASK ADDED", $"Task added: '{title}'");
        public void LogTaskCompleted(int id, string title) => AddLog("TASK COMPLETED", $"Task completed: '{title}' (ID: {id})");
        public void LogTaskDeleted(int id) => AddLog("TASK DELETED", $"Task deleted: ID {id}");
        public void LogReminderSet(string title, DateTime date) => AddLog("REMINDER SET", $"Reminder set for '{title}' on {date.ToShortDateString()}");
        public void LogQuizStarted() => AddLog("QUIZ STARTED", "Cybersecurity quiz started");
        public void LogQuizCompleted(int score, int total) => AddLog("QUIZ COMPLETED", $"Quiz completed - Score: {score}/{total}");
        public void LogNLPInteraction(string input, string intent) => AddLog("NLP INTERACTION", $"Intent: '{intent}' | Command: '{input}'");
        public void LogTaskExtraction(string input, string extracted) => AddLog("NLP TASK EXTRACTION", $"Extracted: '{extracted}' from '{input}'");
        public void LogLogView() => AddLog("LOG VIEWED", "User viewed activity log");
        public void LogPanelClosed(string panelName) => AddLog("PANEL CLOSED", $"{panelName} panel closed");
    }
}