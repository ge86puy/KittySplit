using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpenseManager
{
    public class UserManager
    {
        public void AddNewUser()
        {
            // Domain logic for adding a new user
            Console.WriteLine("Adding a new user...");

            using (var context = new ExpenseContext())
            {
                Console.Write("Enter user name: ");
                string userName = Console.ReadLine();

                var user = new User { Name = userName };

                context.Users.Add(user);
                context.SaveChanges();

                Console.WriteLine("User added successfully.");
            }
        }
    }
}
