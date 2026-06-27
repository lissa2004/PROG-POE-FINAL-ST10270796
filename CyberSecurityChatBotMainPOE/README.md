# Cybersecurity Awareness Chatbot

## Student Information

* **Name:** Kamogelo Mathikge
* **Student Number:** ST10270796
* **Course:** Programming 2A - PROG6221- PART 2 - POE
* **Institution:** The Independent Institute of Education Rosebank College


# Project Description

The Cybersecurity Awareness Chatbot is a C# WPF desktop application designed to educate users about online safety and cybersecurity threats.

The chatbot interacts with users through a graphical chat interface and provides helpful information about:

* Phishing attacks
* Password safety
* Safe browsing
* Hacking awareness
* Online privacy and security tips

The application includes interactive chatbot conversations, random cybersecurity tips, typing animations, emotional awareness responses, and memory recall, quiz, Activity Log and Task Assistant(with and SQL intergration) features to create a more engaging user experience.


# Features

* Interactive chatbot GUI using WPF
* Typing animation effect
* Audio greeting on startup
* Random cybersecurity tips
* Conversation flow handling
* Memory and recall functionality
* Sentiment detection
* Personalized chatbot responses
* Error handling and validation
* Enter key support for sending messages
* Scrollable chat interface
* Goodbye exit functionality


# Part 1: Initial Submission

* Complete chatbot application structure
* Implementation of object-oriented programming principles
* Multiple classes for chatbot logic and GUI management
* Functional chatbot interaction system
* Organized project structure
* Cybersecurity educational content integration


# Part 2: GUI Implementation

* Converted console chatbot into a WPF desktop application
* Created modern chat interface with message bubbles
* Added typing animation effect
* Added sound greeting system
* Added user name capturing system
* Implemented chatbot conversation flow
* Improved chatbot user experience

# Part 3: Advanced Features (Final Submission)

## Natural Language Processing (NLP) Simulation

* Keyword detection using `string.Contains()`
* Intent detection
* Flexible command recognition
* Task extraction from user input
* Reduced "I don't understand" responses

## Supported intents:

* Information
* Quiz
* Task
* Reminder
* Activity Log
* Exit

## Cybersecurity Mini-Game (Quiz)

* 14 Multiple Choice and True/False questions
* Immediate feedback after every answer
* Score tracking
* Final quiz results
* Personalized feedback
* One question displayed at a time

## Topics Covered:

* Phishing
* Password Security
* Safe Browsing
* Malware
* Social Engineering
* Public Wi-Fi
* Email Security
* Two-Factor Authentication

## Task Assistant with SQL Integration

Users can:

* Add tasks
* View tasks
* Complete tasks
* Delete tasks
* Set reminder dates
* Track task status
* Store tasks in a MySQL database

## Activity Log

The chatbot automatically records:

* Quiz started
* Quiz completed
* Tasks added
* Tasks completed
* Tasks deleted
* Reminder creation
* NLP interactions
* Activity Log viewed

The latest actions are displayed with timestamps.

# Technologies Used

* C#
* .NET Framework / .NET Core
* WPF (Windows Presentation Foundation)
* XAML
* MySQL Database
* System.Media (SoundPlayer)
* GitHub Actions (CI)
* Visual Studio 2022
* NuGet Package Manager



# Project Structure

```text
CyberSecurityChatBotMainPOE/
│
├── Models/
│   ├── Task.cs
│   ├── ActivityLogEntry.cs
│   └── QuizQuestion.cs
│
├── Services/
│   ├── DatabaseHelper.cs
│   ├── TaskService.cs
│   ├── ActivityLogService.cs
│   └── NLPService.cs
│
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── CybersecurityChatbot.cs
├── QuizManager.cs
├── App.xaml
├── App.xaml.cs
├── App.config
├── packages.config
│
├── audio/
│   └── greeting.wav
│
├── .github/
│   └── workflows/
│       └── ci.yml
│
├── README.md
└── .gitignore                
```

# Database Setup

## Create Database and Tables

```sql
CREATE DATABASE maincybersecuritychatbot;

USE maincybersecuritychatbot;

CREATE TABLE tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    reminder_date DATETIME,
    status VARCHAR(50) DEFAULT 'Pending'
);
```
## Update Connection String

In **MainWindow.xaml.cs**, update the connection string:

```csharp
string connectionString =
"server=localhost;database=maincybersecuritychatbot;uid=YOUR_USERNAME;pwd=YOUR_PASSWORD;";
```

# How to Run the Application

1. Open the project in Visual Studio 2022.
2. Restore NuGet packages.
3. Configure the MySQL connection string.
4. Build the solution.
5. Press **Ctrl + F5** to run the application.
6. Wait for the greeting audio.
7. Enter your name.
8. Start chatting with the chatbot.

# How to Use the Chatbot

* Type your message.
* Click **SEND** or press **Enter**.
* Ask cybersecurity-related questions.
* Request cybersecurity tips.
* Start the quiz.
* Add and manage tasks.
* View the Activity Log.


# Example Questions

Users can ask questions such as:

* Give me a tip
* Another tip
* Tell me more
* Explain more
* Start quiz
* Play quiz
* Add task Enable 2FA
* Remind me to update Windows
* Show activity log
* Show log
* Recent actions
* What have you done for me?


# Example Questions

Users can ask:

* What is phishing?
* Give me a phishing tip.
* Explain password safety.
* Tell me more.
* What is malware?
* What is safe browsing?
* What can I ask?
* Help.
* Start quiz.
* Show activity log.


# Memory and Recall Features

The chatbot remembers:

* User's name
* User interests
* Previous cybersecurity topics discussed

This enables more personalized conversations.


# Conversation Flow Features

The chatbot supports follow-up interactions such as:

* "Tell me more"
* "Explain more"
* "Another tip"

This creates a more natural and engaging conversation experience.


# Random Responses

The chatbot uses lists and dictionaries to randomly select cybersecurity tips and responses, ensuring conversations feel less repetitive and more interactive.


# Error Handling

The chatbot includes:

* Empty input validation
* Invalid command handling
* Missing audio file handling
* Database connection handling
* Quiz validation
* Safe message handling
* Exception handling throughout the application



# Exit the Application

To close the chatbot, type:

```text
goodbye
```
or

```text
bye
```

or

```text
exit
```

The chatbot will display a goodbye message and close the application.


# Changelog

# Changelog

## Version 1.0 (13/04/2026)

* Program structure created
* Chatbot response system implemented
* Audio greeting feature added
* README created

---

## Version 2.0 (29/05/2026)

* Added WPF graphical interface
* Added typing animation
* Added memory and recall
* Added random response system
* Added Task Assistant
* Added MySQL integration

---

## Version 3.0 (03/06/2026)

* Added NLP simulation
* Added Intent Detection
* Added Cybersecurity Quiz
* Added Activity Log
* Added SQL Task Assistant
* Added Reminder System
* Improved conversation flow
* Fixed bugs and improved performance

## Developer Notes

This project was developed for educational purposes to demonstrate:

* Object-Oriented Programming
* GUI development using WPF
* Event-driven programming
* SQL database integration
* Natural Language Processing simulation
* GitHub Actions (CI/CD)
* Software engineering best practices
* Cybersecurity awareness education


# Author

Kamogelo Mathikge
ST10270796
Programming 2A - PROG6221