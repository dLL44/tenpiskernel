
// Requirements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sys = Cosmos.System;

namespace tenpis
{
    /*  User System  */
    public class User
    {
            public string Username { get; set; }
            public string Password { get; set; }
        // Add more user-related properties later (password, permissions, etc.)
    }



    /*  File System Classes  */
    public class File
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public class Directory
    {
        public string Name { get; set; }
        public List<File> Files { get; set; }
        public List<Directory> Directories { get; set; }
    }

    public class Kernel : Sys.Kernel
    {
        private List<User> users;
        private Directory rootDirectory;
        private Directory currentDirectory;

        public Kernel()
        {
            users = new List<User>();
            LoadUsersFromFile();
        }
        protected override void BeforeRun() // a before loop funct
        {
            InitializeFileSystem(); // THIS IS HEAVILY REQUIRED. FILE SYSTEM WILL NOT RUN IF NOT CALLED.
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Tenpis libnux\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("tenpis beta booted surprisingly with help from GRUB"); // edit this?
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
        }
        protected override void Run() //essentialy a loop funct
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{currentDirectory.Name}> "); // Print the cwd
            Console.ForegroundColor = ConsoleColor.White;
            var input = Console.ReadLine().ToLower();
            string[] inputArgs = input.Split(' ');

            switch (inputArgs[0]) // switch/case for command
            {
                case "help":
                    if (inputArgs.Length > 1)
                    {
                        GetHelp(inputArgs[1]);
                    } else
                    {
                        GenericHelp();
                    }
                    break;

                case "about":
                    AboutCmd();
                    break;

                case "cls":
                    Console.Clear();
                    break;

                case "mkdir":
                    CreateDirectory(inputArgs[1]);
                    break;

                case "ls":
                    ListContents();
                    break;

                case "touch":
                    if (inputArgs.Length > 1)
                        CreateFile(inputArgs[1]);
                    else
                        Console.WriteLine("Invalid command. Usage: touch <filename>");
                    break;

                case "remove":
                    if (inputArgs.Length > 1)
                        RemoveFile(inputArgs[1]);
                    else
                        Console.WriteLine("Invalid command. Usage: remove <filename>");
                    break;

                case "cat":
                    if (inputArgs.Length > 1)
                        DisplayFileContent(inputArgs[1]);
                    else
                        Console.WriteLine("Invalid command. Usage: cat <filename>");
                    break;

                case "cd":
                    if (inputArgs.Length > 1)
                        ChangeDirectory(inputArgs[1]);
                    else
                        Console.WriteLine("Invalid command. Usage: cd <directory>");
                    break;
                case "serch":
                    if (inputArgs.Length > 1)
                    {
                        string fileToSearch = inputArgs[1];
                        string serchRSLT = SearchFile(currentDirectory, fileToSearch);
                        Console.WriteLine(serchRSLT != string.Empty ? serchRSLT : "instance not found");
                    } else
                    {
                        Console.WriteLine("Invalid command. Usage: serch <file>");
                    }
                    break;
                case "exit":
                case "shutdown":
                    Console.WriteLine("leaving tenpis kernel");
                    Console.WriteLine("bye!");
                    Sleep(2000);
                    Cosmos.System.Power.Shutdown();
                    break;
                case "lgin":
                    Login();
                    break;
                case "mkusr":
                    AddUser(inputArgs[1]);
                    break;
                default:
                    BadCommand();
                    break;
            }
        }
        public int wrongCommandCount = 0;
        private void InitializeFileSystem()
        {
            rootDirectory = new Directory { Name = "root", Files = new List<File>(), Directories = new List<Directory>() };
            currentDirectory = rootDirectory;
        }

        private void CreateDirectory(string directoryName)
        {
            currentDirectory.Directories.Add(new Directory { Name = directoryName, Files = new List<File>(), Directories = new List<Directory>() });
        }

        private void ListContents()
        {
            foreach (var dir in currentDirectory.Directories)
            {
                Console.WriteLine($"Directory: {dir.Name}");
            }

            foreach (var file in currentDirectory.Files)
            {
                Console.WriteLine($"File: {file.Name}");
            }
        }

        private void CreateFile(string fileName)
        {
            Console.WriteLine($"add some content for {fileName}!: (type \"$EXIT\" to finish)");

            StringBuilder content = new StringBuilder();
            string line;
            do
            {
                line = Console.ReadLine();
                if (line != "$EXIT")
                {
                    content.AppendLine(line);
                }
            } while (line != "$EXIT");

            currentDirectory.Files.Add(new File { Name = fileName, Content = content.ToString() });
        }

        private void RemoveFile(string fileName)
        {
            var fileToRemove = currentDirectory.Files.Find(f => f.Name == fileName);
            if (fileToRemove != null)
                currentDirectory.Files.Remove(fileToRemove);
            else
                Console.WriteLine($"File '{fileName}' not found.");
        }

        private void DisplayFileContent(string fileName)
        {
            var fileToDisplay = currentDirectory.Files.Find(f => f.Name == fileName);
            if (fileToDisplay != null)
                Console.WriteLine(fileToDisplay.Content);
            else
                Console.WriteLine($"File '{fileName}' not found.");
        }

        private void ChangeDirectory(string directoryName)
        {
            var directoryToChange = currentDirectory.Directories.Find(d => d.Name == directoryName);
            if (directoryToChange != null)
                currentDirectory = directoryToChange;
            else
                Console.WriteLine($"Directory '{directoryName}' not found.");
        }

        private void BadCommand()
        {
           
            Console.WriteLine("i have no fucking idea what your talking about.");
            wrongCommandCount++;
            if (wrongCommandCount == 10)
            {
                Console.WriteLine("you said something completely unkown for 10 times, check your typing");
            }
            else  if(wrongCommandCount > 10){
                Console.WriteLine($"This kernel cant understand your bullshit, you are now at {wrongCommandCount} times.");
            } else if(wrongCommandCount > 100) { 
                Console.WriteLine("I had enough, im boot you out of this kernel");
                extKernel();
            } 
        }
        

        private void extKernel()
        {
            Console.WriteLine("Exiting tenpis...");
            Sleep(1000);
            Cosmos.System.Power.Shutdown();
        }
        private void AboutCmd()
        {
            Console.WriteLine("tenpis ver 1.BETA");
        }

        private void GetHelp(string command)
        {
                switch (command)
                {
                case "help":
                    Console.WriteLine("help prints help i can not tell you anything else\nUSAGE: help <command>"); 
                    break;
                case "about":
                    Console.WriteLine("prints information conscerning tenpis");
                    break;
                case "cls":
                    Console.WriteLine("clears your screen");
                    break;
                case "mkdir":
                    Console.WriteLine("make a new directory\nUSAGE: mkdir <directory>");
                    break;
                case "ls":
                    Console.WriteLine("list the files in your current working directory");
                    break;
                case "touch":
                    Console.WriteLine("touch a new file ( wierd command name iknow )\nUSAGE: touch <filename>");
                    break;
                case "remove":
                    Console.WriteLine("removes a file currently, fixing soon.\nUSAGE: remove <file>");
                    break;
                case "cat":
                    Console.WriteLine("conCATonate a file ( just reads it )\nUSAGE: cat <file>");
                    break;
                case "cd":
                    Console.WriteLine("change directory\nUSAGE: cd <directory>");
                    break;
                case "serch":
                    Console.WriteLine("search for a file, crawls from root to every dir and subdir\nUSAGE: serch <file>");
                    break;
                case "exit":
                case "shutdown":
                    Console.WriteLine("shutdown the machine");
                    break;
                case "lgin":
                    Console.Write("login to a diffrent user");
                    break;
                case "mkusr":
                    Console.WriteLine("make a new user\nUSAGE: mkusr <username>");
                    break;
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void GenericHelp()
        {
            Console.WriteLine("help - prints help (no fucking shit)\n" +
"about - give the about about tenpis\n" +
"cls - clear your damn filthy screen\n" +
"mkdir - create a new directory\n" +
"ls - list contents of the current directory\n" +
"touch - create a new file\n" +
"remove - remove a file\n" +
"cat - display file content\n" +
"cd - change directory\n" +
"serch - search for a file\n" +
"exit or shutdown - shutdowns the OS");
        }
        private string SearchFile(Directory directory, string fileName)
        {
            foreach (var file in directory.Files)
            {
                if (file.Name.Equals(fileName))
                {
                    return $"File found: {file.Name} in {directory.Name}";
                }
            }

            foreach (var dir in directory.Directories)
            {
                string result = SearchFile(dir, fileName);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }

            return string.Empty;
        }

        private void AddUser(string username)
        {
            if (users.Any(u => u.Username == username))
            {
                Console.WriteLine($"User '{username}' already exists.");
            }
            else
            {
                Console.WriteLine("Enter a password:");

                StringBuilder password = new StringBuilder();
                ConsoleKeyInfo key;

                do
                {
                    key = Console.ReadKey(true);

                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        password.Append(key.KeyChar);
                        Console.Write("#"); // Display # for each character
                    }
                    else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password.Remove(password.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                } while (key.Key != ConsoleKey.Enter);

                Console.WriteLine(); // Move to the next line

                string passwordString = password.ToString(); // Convert StringBuilder to string

                users.Add(new User { Username = username, Password = passwordString });

                // Saving users to the file 'users.txt'
                using (StreamWriter writer = new StreamWriter("users.txt"))
                {
                    foreach (var usr in users)
                    {
                        writer.WriteLine($"{usr.Username},{usr.Password}");
                    }
                }

                Console.WriteLine($"Successfully saved '{username}' to user data.");
            }
        }

        private void loginToUser(string username, string password)
        {
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                Console.WriteLine($"Logged in as '{username}'.");
                // Perform actions specific to the logged-in user
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
            }
        }

        private void Login()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password.Append(key.KeyChar);
                    Console.Write("#"); // Display # for each character
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine(); // Move to the next line

            string passwordString = password.ToString(); // Convert StringBuilder to string

            loginToUser(username, passwordString);
        }

        private void LoadUsersFromFile()
        {
            string filePath = "users.txt"; // Define the file path where user data is stored

            if (System.IO.File.Exists(filePath))
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        users.Add(new User { Username = parts[0], Password = parts[1] });
                    }
                    
                }

                Console.WriteLine("User data loaded from file.");
            }
            else
            {
                Console.WriteLine("No user data file found.");
            }
        }

        private void Sleep(int milliseconds)
        {
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < milliseconds)
            {
                // This loop does nothing but waits for the specified time
            }
        }
    }
}