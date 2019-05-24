﻿using SprintPlanner.Core;
using System;

namespace SprintPlanner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var jh = new JiraWrapper(new SimpleHttpRequester());
            jh.Url = "https://jira.sdl.com";
            //jh.Url = "https://issues.apache.org/jira";
            bool isLoggedIn = false;
            while (!isLoggedIn)
            {
                Console.Write("Please enter username: ");
                string username = Console.ReadLine();

                Console.Write("Please enter password: ");
                string password = Console.ReadLine();

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

            var assignees = jh.GetAllAssigneesInSprint(1137, 6182);
            foreach (var item in assignees)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("--------------------");

            var assigneeswithallocation = jh.GetAllAssigneesAndWorkInSprint(1137, 6182);
            foreach (var item in assigneeswithallocation)
            {
                decimal hours = item.Item2 / 3600m;
                Console.WriteLine(item.Item1 + " has allocated " + hours);
            }


            Console.ReadLine();
        }
    }
}