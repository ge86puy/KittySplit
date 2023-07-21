using System;

namespace ExpenseManager
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instantiate domain objects
            UserManager userManager = new UserManager();
            ExpenseManager expenseManager = new ExpenseManager();

            // Call the functions representing the technical use cases

            //userManager.AddNewUser();  //Adds new user to the database
            //expenseManager.AddExpense();  //Adds expense and the user who paid
            //expenseManager.AddMoneyReceived();  //Adds money received by the person
            //expenseManager.ViewExpenseSheet();
            //expenseManager.CalculateShares();
            expenseManager.SettleExpenses();
            //expenseManager.ResetDatabase();
        }
    }
}
