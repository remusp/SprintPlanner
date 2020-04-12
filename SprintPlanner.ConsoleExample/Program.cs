using SprintPlanner.Core;
using System;
using System.Security;

namespace SprintPlanner
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var jh = new JiraWrapper(new SimpleHttpRequester());
            jh.ServerAddress = "https://jira.sdl.com";
            //jh.Url = "https://issues.apache.org/jira";
            bool isLoggedIn = false;
            while (!isLoggedIn)
            {
                Console.Write("Please enter username: ");
                string username = Console.ReadLine();

                Console.Write("Please enter password: ");
                var password = GetPassword();

                isLoggedIn = jh.Login(username, password);
                Console.WriteLine("--------------------");
            }

            var boards = jh.GetBoards();
            foreach (var board in boards)
            {
                Console.WriteLine("Sprint: " + board.Value);
            }
            Console.WriteLine("--------------------");

            var sprints = jh.GetOpenSprints(1137);
            foreach (var sprint in sprints)
            {
                Console.WriteLine("Sprint: " + sprint.Item2);
            }
            Console.WriteLine("--------------------");

            var assignees = jh.GetAllAssigneesInSprint(6182);
            foreach (var item in assignees)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("--------------------");

            var assigneeswithallocation = jh.GetAllAssigneesAndWorkInSprint(6182);
            foreach (var item in assigneeswithallocation)
            {
                decimal hours = item.Item2 / 3600m;
                Console.WriteLine(item.Item1 + " has allocated " + hours);
            }


            Console.ReadLine();
        }

        public static SecureString GetPassword()
        {
            var pwd = new SecureString();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            return pwd;
        }
    }
}
