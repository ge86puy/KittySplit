using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpenseManager
{
    public class ExpenseManager
    {
        private List<Expense> expenses = new List<Expense>();

        public void AddExpense()
        {
            // Domain logic for adding an expense
            Console.WriteLine("Adding an expense...");

            Console.Write("Enter description: ");
            string description = Console.ReadLine();

            Console.Write("Enter amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            using (var context = new ExpenseContext())
            {
                Console.Write("Enter user name who paid: ");
                string userName = Console.ReadLine();

                var user = context.Users.FirstOrDefault(u => u.Name == userName);

                if (user == null)
                {
                    Console.WriteLine("Invalid user name.");
                    return;
                }

                var expense = new Expense { Description = description, Amount = amount, PaidBy = user };
                context.Expenses.Add(expense);
                context.SaveChanges();

                expenses.Add(expense);

                Console.WriteLine("Expense added successfully.");
            }
        }
        public void AddMoneyReceived()
        {
            // Domain logic for adding money received
            Console.WriteLine("Adding money received...");

            Console.Write("Enter user name who received money: ");
            string receiverName = Console.ReadLine();

            using (var context = new ExpenseContext())
            {
                var receiver = context.Users.FirstOrDefault(u => u.Name == receiverName);

                if (receiver == null)
                {
                    Console.WriteLine("Invalid user name.");
                    return;
                }

                Console.Write("Enter amount received: ");
                decimal amountReceived = decimal.Parse(Console.ReadLine());

                Console.Write("Enter user name who gave the money: ");
                string giverName = Console.ReadLine();

                var giver = context.Users.FirstOrDefault(u => u.Name == giverName);

                if (giver == null)
                {
                    Console.WriteLine("Invalid user name.");
                    return;
                }

                // Update MoneyReceived property of the receiver
                receiver.MoneyReceived += amountReceived;

                // Add a new expense record to track the money received
                var expense = new Expense
                {
                    Description = $"Money Received from {giver.Name}",
                    Amount = amountReceived,
                    PaidBy = giver
                };

                context.Expenses.Add(expense);
                context.SaveChanges();

                Console.WriteLine("Money received added successfully.");
            }
        }



        public void CalculateShares()
        {
            Console.WriteLine("Calculating shares...");

            using (var context = new ExpenseContext())
            {
                var expenses = context.Expenses.Include(e => e.PaidBy).ToList();

                // Step 1: Calculate the net balance for each user (total expenses - total money received)
                var userBalances = new Dictionary<User, decimal>();
                foreach (var user in context.Users)
                {
                    decimal totalExpenseForUser = expenses.Where(e => e.PaidBy == user).Sum(e => e.Amount);
                    decimal totalReceivedForUser = user.MoneyReceived;
                    userBalances[user] = totalReceivedForUser - totalExpenseForUser;
                }

                // Step 2: Print the balances
                Console.WriteLine("User Balances:");
                foreach (var userBalance in userBalances)
                {
                    Console.WriteLine($"{userBalance.Key.Name}: {userBalance.Value}");
                }
            }
        }


        public void SettleExpenses()
        {
            Console.WriteLine("Settling expenses...");

            using (var context = new ExpenseContext())
            {
                var expenses = context.Expenses.Include(e => e.PaidBy).ToList();

                // Calculate the net balance for each user (total expenses - total money received)
                var userBalances = new Dictionary<User, decimal>();
                foreach (var user in context.Users)
                {
                    decimal totalExpenseForUser = expenses.Where(e => e.PaidBy == user).Sum(e => e.Amount);
                    decimal totalReceivedForUser = user.MoneyReceived;
                    userBalances[user] = totalReceivedForUser - totalExpenseForUser;
                }

                // Sort users based on their net balance
                var sortedUsers = userBalances.OrderBy(kv => kv.Value).ToList();

                // Step 4: Try to settle expenses starting from the most positive balance user
                int i = 0, j = sortedUsers.Count - 1;
                while (i < j)
                {
                    var payer = sortedUsers[i].Key;
                    var receiver = sortedUsers[j].Key;

                    decimal amountToSettle = Math.Min(Math.Abs(sortedUsers[i].Value), Math.Abs(sortedUsers[j].Value));
                    sortedUsers[i] = new KeyValuePair<User, decimal>(payer, sortedUsers[i].Value + amountToSettle);
                    sortedUsers[j] = new KeyValuePair<User, decimal>(receiver, sortedUsers[j].Value - amountToSettle);

                    Console.WriteLine($"{payer.Name} pays {amountToSettle} to {receiver.Name}");

                    // Move to the next pair if balances are close to zero (within a small tolerance)
                    if (Math.Abs(sortedUsers[i].Value) < 0.01m)
                        i++;

                    if (Math.Abs(sortedUsers[j].Value) < 0.01m)
                        j--;
                }
            }
        }

        public void ViewExpenseSheet()
        {
            // Domain logic for viewing the expense sheet
            Console.WriteLine("Viewing expense sheet...");

            using (var context = new ExpenseContext())
            {
                var expenses = context.Expenses.Include(e => e.PaidBy).ToList();

                Console.WriteLine("Expenses:");
                foreach (var expense in expenses)
                {
                    Console.WriteLine($"Description: {expense.Description}, Amount: {expense.Amount}, PaidBy: {expense.PaidBy.Name}");
                }

                Console.WriteLine("\nMoney Received:");
                var users = context.Users.ToList();
                foreach (var user in users)
                {
                    Console.WriteLine($"User: {user.Name}, MoneyReceived: {user.MoneyReceived}");
                }
            }
        }

        public void ResetDatabase()
        {
            Console.WriteLine("Resetting the database...");

            using (var context = new ExpenseContext())
            {
                // Clear all expenses
                var expenses = context.Expenses.ToList();
                context.Expenses.RemoveRange(expenses);

                // Clear all users
                var users = context.Users.ToList();
                context.Users.RemoveRange(users);

                // Save changes to the database
                context.SaveChanges();

                Console.WriteLine("Database reset complete.");
            }
        }


    }
}
