using CyberSecurityChatBotMainPOE;
using CyberSecurityChatBotMainPOE.Models;
using CyberSecurityChatBotMainPOE.Services;
using System;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TaskModel = CyberSecurityChatBotMainPOE.Models.UserTask;


namespace CyberSecurityChatBotMainPOE
{
    public partial class MainWindow : Window
    {
        // Services
        private readonly TaskService _taskService;
        private readonly ActivityLogService _logService;
        private readonly NLPService _nlpService;
        private readonly QuizManager _quizManager = new QuizManager();
        private readonly CybersecurityChatbot _chatbot = new CybersecurityChatbot();
        private readonly Random _random = new Random();

        // UI State
        private bool _waitingForName = true;
        private string _userName = "";

        public MainWindow()
        {
            InitializeComponent();

            // Initialize services
            string connectionString = "server=localhost;database=CyberSecurityChatBotMainPOE;uid=root;pwd=kamogelomathikge@2004;";
            _taskService = new TaskService(connectionString);
            _logService = new ActivityLogService();
            _nlpService = new NLPService();

            // Hide panels
            TaskAssistantPanel.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility = Visibility.Collapsed;

            // Play greeting
            PlayGreetingAudio();

            // Test connection
            TestConnection();

            // Load tasks
            LoadTasks();

            // Load welcome message
            Loaded += MainWindow_Loaded;
        }


        //Initialization

        private void TestConnection()
        {
            if (_taskService.TestConnection())
            {
                MessageBox.Show("✅ Database Connected!");
                _logService.AddLog("DATABASE", "Database connected successfully");
            }
            else
            {
                MessageBox.Show("❌ Database connection failed!");
                _logService.AddLog("DATABASE", "Database connection failed");
            }
        }

        private void PlayGreetingAudio()
        {
            try
            {
                SoundPlayer player = new SoundPlayer(@"audio\greeting.wav");
                player.Play();
            }
            catch
            {
                // Silent fail
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(300);
            TypeText("Hello, Welcome to Cyber Security Awareness Bot!\nI'm here to help you stay safe online.\nPlease enter your name in the chat below.");
        }

        

        //Task Methods

        private void LoadTasks()
        {
            try
            {
                var tasks = _taskService.GetAllTasks();
                TaskListBox.Items.Clear();

                foreach (var task in tasks)
                {
                    TaskListBox.Items.Add(task.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}");
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskTitleBox.Text))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }
            var task = new TaskModel

            {
                Title = TaskTitleBox.Text,
                Description = TaskDescriptionBox.Text,
                ReminderDate = ReminderDatePicker.SelectedDate
            };

            if (_taskService.AddTask(task))
            {
                TypeText($"Task added with the description \"{task.Description}\".");
                _logService.LogTaskAdded(task.Title);

                if (ReminderCheckBox.IsChecked == true && ReminderDatePicker.SelectedDate.HasValue)
                {
                    TypeText($"Got it! I'll remind you on {ReminderDatePicker.SelectedDate.Value.ToShortDateString()}.");
                    _logService.LogReminderSet(task.Title, ReminderDatePicker.SelectedDate.Value);
                }

                LoadTasks();
                TaskTitleBox.Clear();
                TaskDescriptionBox.Clear();
                ReminderDatePicker.SelectedDate = null;
                ReminderCheckBox.IsChecked = false;
            }
            else
            {
                MessageBox.Show("❌ Failed to add task.");
            }
        }

        private void ViewTasksButton_Click(object sender, RoutedEventArgs e)
        {
            LoadTasks();
            _logService.AddLog("VIEW TASKS", "User viewed all tasks");
        }

        private void CompleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            int id = GetSelectedTaskId();
            if (id == -1)
            {
                MessageBox.Show("Select a task first!");
                return;
            }

            if (_taskService.CompleteTask(id))
            {
                MessageBox.Show("✅ Task completed!");
                string title = GetTaskTitle(id);
                _logService.LogTaskCompleted(id, title);
                LoadTasks();
            }
            else
            {
                MessageBox.Show("❌ Failed to complete task.");
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            int id = GetSelectedTaskId();
            if (id == -1)
            {
                MessageBox.Show("Select a task first!");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this task?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (_taskService.DeleteTask(id))
                {
                    MessageBox.Show("✅ Task deleted!");
                    _logService.LogTaskDeleted(id);
                    LoadTasks();
                }
                else
                {
                    MessageBox.Show("❌ Failed to delete task.");
                }
            }
        }

        private void CloseTaskButton_Click(object sender, RoutedEventArgs e)
        {
            TaskAssistantPanel.Visibility = Visibility.Collapsed;

            string followUp = GetRandomTaskFollowUp();
            TypeText($"Task Assistant closed.\n\n{followUp}\n\n💡 You can:\n• Type 'start quiz' to test your knowledge\n• Type 'show activity log' to see your actions\n• Ask me about cybersecurity topics");

            _logService.LogPanelClosed("Task Assistant");
            UserInput.Focus();
        }

        private int GetSelectedTaskId()
        {
            if (TaskListBox.SelectedItem == null) return -1;
            string item = TaskListBox.SelectedItem.ToString();
            return int.Parse(item.Split('|')[0].Trim());
        }

        private string GetTaskTitle(int id)
        {
            var tasks = _taskService.GetAllTasks();
            return tasks.Find(t => t.Id == id)?.Title ?? "Unknown Task";
        }

        
        //Quiz Methods

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            StartQuiz();
            StartQuizButton.Visibility = Visibility.Collapsed;
        }

        private void StartQuiz()
        {
            _quizManager.StartQuiz();
            QuizPanel.Visibility = Visibility.Visible;
            TaskAssistantPanel.Visibility = Visibility.Collapsed;
            StartQuizButton.Visibility = Visibility.Collapsed;
            LoadCurrentQuestion();
            _logService.LogQuizStarted();
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            string buttonText = clickedButton.Content.ToString();

            // Determine the answer format
            string selectedAnswer;

            // Check if it's a True/False question
            if (buttonText == "True" || buttonText == "False")
            {
                // True/False: use the FULL word
                selectedAnswer = buttonText;
            }
            else
            {
                // Multiple Choice: use the first character (A, B, C, D)
                selectedAnswer = buttonText.Substring(0, 1);
            }

            bool correct = _quizManager.SubmitAnswer(selectedAnswer);

            FeedbackLabel.Text = (correct ? "✅ Correct!\n\n" : "❌ Incorrect!\n\n") + _quizManager.GetExplanation();
            ScoreLabel.Text = $"Score: {_quizManager.Score}/{_quizManager.TotalQuestions}";
            NextQuestionButton.Visibility = Visibility.Visible;

            AnswerAButton.IsEnabled = false;
            AnswerBButton.IsEnabled = false;
            AnswerCButton.IsEnabled = false;
            AnswerDButton.IsEnabled = false;
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_quizManager.NextQuestion())
            {
                LoadCurrentQuestion();
                AnswerAButton.IsEnabled = true;
                AnswerBButton.IsEnabled = true;
                AnswerCButton.IsEnabled = true;
                AnswerDButton.IsEnabled = true;
            }
            else
            {
                FinishQuiz();
            }
        }

        private void CloseQuizButton_Click(object sender, RoutedEventArgs e)
        {
            QuizPanel.Visibility = Visibility.Collapsed;
            TaskAssistantPanel.Visibility = Visibility.Collapsed;

            string followUp = GetRandomFollowUpQuestion();
            TypeText($"Quiz closed.\n\n{followUp}\n\n💡 You can:\n• Type 'start quiz' to play again\n• Type 'add task' to manage tasks\n• Type 'show activity log' to see your actions");

            _logService.LogPanelClosed("Quiz");
            UserInput.Focus();
        }


        private void LoadCurrentQuestion()
        {
            var question = _quizManager.GetCurrentQuestion();
            if (question == null) return;

            QuestionNumberLabel.Text = $"Question {_quizManager.CurrentQuestionNumber} of {_quizManager.TotalQuestions}";
            QuestionLabel.Text = question.Question;

            AnswerAButton.Content = question.Options[0];
            AnswerBButton.Content = question.Options[1];

            if (question.Options.Count > 2)
            {
                AnswerCButton.Content = question.Options[2];
                AnswerCButton.Visibility = Visibility.Visible;
                AnswerDButton.Content = question.Options[3];
                AnswerDButton.Visibility = Visibility.Visible;
            }
            else
            {
                AnswerCButton.Visibility = Visibility.Collapsed;
                AnswerDButton.Visibility = Visibility.Collapsed;
            }

            ScoreLabel.Text = $"Score: {_quizManager.Score}/{_quizManager.TotalQuestions}";
            FeedbackLabel.Text = "";
            NextQuestionButton.Visibility = Visibility.Collapsed;

            AnswerAButton.IsEnabled = true;
            AnswerBButton.IsEnabled = true;
            AnswerCButton.IsEnabled = true;
            AnswerDButton.IsEnabled = true;
        }

        private void FinishQuiz()
        {
            string message = _quizManager.Score >= 10 ? "🏆 Great job! You're a cybersecurity pro!" :
                            _quizManager.Score >= 7 ? "👍 Good effort! Keep learning to stay safe online!" :
                            "📚 Keep practicing your cybersecurity knowledge!";

            MessageBox.Show($"Quiz Complete!\n\nFinal Score: {_quizManager.Score}/{_quizManager.TotalQuestions}\n\n{message}", "Quiz Results");

            _logService.LogQuizCompleted(_quizManager.Score, _quizManager.TotalQuestions);

            QuizPanel.Visibility = Visibility.Visible;
            TaskAssistantPanel.Visibility = Visibility.Collapsed;
            
        }

     

        //Chat Methods

        //Part3: POE 
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                AddBotMessage("Please type something.");
                return;
            }

            AddUserMessage(input);

            // First ask for user's name - WITH ENHANCED INTRODUCTION
            if (_waitingForName)
            {
                _userName = input;
                _waitingForName = false;

                string introMessage = $"Nice to meet you, {_userName}! 👋\n\n" +
                                      "I'm your Cybersecurity Awareness Bot. I'm here to help you stay safe online! 💻🔒\n\n" +
                                      "📚 **Here are some topics you can ask me about:**\n" +
                                      "• 🔹 **Phishing** - How to spot and avoid email scams\n" +
                                      "• 🔹 **Passwords** - How to create and manage strong passwords\n" +
                                      "• 🔹 **Safe Browsing** - How to browse the internet safely\n" +
                                      "• 🔹 **Hacking** - Understanding cyber threats\n" +
                                      "💡 **You can also:**\n"+
                                      "🆘 Type 'help' anytime to see all commands!\n\n" +
                                      "What would you like to learn about today? 🤔";

                TypeText(introMessage);
                UserInput.Clear();
                return;
            }

            // Detect intent using NLP service
            var intent = _nlpService.DetectIntent(input);
            _logService.LogNLPInteraction(input, intent.ToString());

            switch (intent)
            {
                case NLPService.Intent.Quiz:
                    AddBotMessage("Starting the Cybersecurity Quiz...");
                    StartQuiz();
                    break;

                case NLPService.Intent.Task:
                    string taskDescription = _nlpService.ExtractTaskDescription(input);
                    if (!string.IsNullOrWhiteSpace(taskDescription) && taskDescription != input)
                    {
                        TaskDescriptionBox.Text = taskDescription;
                        TypeText($"I'll help you add a task: \"{taskDescription}\". Please fill in the details on the TASK ASSISTANT.");
                        _logService.LogTaskExtraction(input, taskDescription);
                    }
                    else
                    {
                        TypeText("Opening Task Assistant...");
                        _logService.AddLog("TASK ASSISTANT", "Task Assistant opened");
                    }
                    TaskAssistantPanel.Visibility = Visibility.Visible;
                    QuizPanel.Visibility = Visibility.Collapsed;
                    break;

                case NLPService.Intent.Log:
                    ShowActivityLog();
                    _logService.LogLogView();
                    break;

                case NLPService.Intent.Exit:
                    TypeText("Stay safe online. Goodbye!");
                    _logService.AddLog("APP CLOSED", "Application closed");
                    Close();
                    return;

                case NLPService.Intent.Info:
                    string response = _chatbot.GetResponse(input);
                    TypeText(response);
                    _logService.AddLog("INFO", $"User asked about: '{input}'");
                    break;

                default:
                    TypeText("I didn't quite understand that. Try asking about passwords, phishing, tasks or quizzes.");
                    break;
            }

            UserInput.Clear();
        }
       

        private Random random = new Random();

        private string GetRandomFollowUpQuestion()
        {
            string[] questions = {
        "What would you like to learn about next? 🤔",
        "Is there a specific cybersecurity topic you're interested in? 💻",
        "Would you like me to explain a security concept? 🔒",
        "Do you have any questions about staying safe online? 🛡️",
        "Would you like to test your knowledge with another quiz? 🧠",
        "Can I help you with anything else today? 😊"
    };
            return questions[random.Next(questions.Length)];
        }

        private string GetRandomTaskFollowUp()
        {
            string[] questions = {
        "Would you like to learn more about cybersecurity topics? 🔒",
        "Do you want to test your knowledge with a quiz? 🧠",
        "Is there anything else I can help you with? 😊",
        "Would you like to see your recent activity? 📋",
        "Do you have any security questions I can answer? 💻",
        "Would you like to add another task? 📝"
    };
            return questions[random.Next(questions.Length)];
        }

        private string GetRandomQuizFollowUp(int score)
        {
            if (score >= 10)
            {
                string[] questions = {
            "Would you like to try a different cybersecurity topic? 🔒",
            "Do you have any questions about what you learned? 💻",
            "Would you like to learn about phishing or passwords next? 📚",
            "Ready for another challenge? 🧠"
        };
                return questions[random.Next(questions.Length)];
            }
            else
            {
                string[] questions = {
            "Would you like to review the topics and try again? 📖",
            "Do you want me to explain any cybersecurity concepts? 💻",
            "Would you like to learn more to improve your score? 🔒",
            "Keep practicing and you'll get better! 💪"
        };
                return questions[random.Next(questions.Length)];
            }
        }

        // Activity Log

        private void ShowActivityLog()
        {
            string logSummary = _logService.GetLogSummary();
            TypeText(logSummary);
        }

       

        //UI Helper Methods

        private async void TypeText(string message)
        {
            Border bubble = new Border
            {
                Background = Brushes.Cyan,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 350
            };

            TextBlock textBlock = new TextBlock
            {
                FontSize = 16,
                Foreground = Brushes.Black,
                TextWrapping = TextWrapping.Wrap
            };

            bubble.Child = textBlock;
            ChatPanel.Children.Add(bubble);

            string currentText = "";
            foreach (char c in message)
            {
                currentText += c;
                textBlock.Text = currentText;
                await Task.Delay(40);
                ChatScrollViewer.ScrollToEnd();
            }
        }

        private void AddBotMessage(string message)
        {
            Border bubble = new Border
            {
                Background = Brushes.LightBlue,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8),
                Margin = new Thickness(8),
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = new TextBlock
                {
                    Text = message,
                    FontSize = 16,
                    Width = 300,
                    TextWrapping = TextWrapping.Wrap
                }
            };

            ChatPanel.Children.Add(bubble);
            ChatScrollViewer.ScrollToEnd();
        }

        private void AddUserMessage(string message)
        {
            Border bubble = new Border
            {
                Background = Brushes.LightPink,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8),
                Margin = new Thickness(8),
                HorizontalAlignment = HorizontalAlignment.Right,
                Child = new TextBlock
                {
                    Text = message,
                    FontSize = 16
                }
            };

            ChatPanel.Children.Add(bubble);
            ChatScrollViewer.ScrollToEnd();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendButton_Click(sender, e);
        }

    
    }
}
