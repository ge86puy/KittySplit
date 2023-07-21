using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ExpenseManager
{
    public class ExpenseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }


        //Select the data provider  |SQLITE
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //optionsBuilder.UseSqlite("Data Source=C:/D/ProSD2022/2022/#10/ext-linq-ef-master/LINQ_EF/example.db");
            //change this location for your own computer. 
            optionsBuilder.UseSqlite("Filename=D:/GERMANY/TUM/SEMESTER 3/Professional Software Development/Assignments/4/psd/PSD-Assignment4/expense_manager.db", option => {
                option.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.PaidBy)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.PaidById);

            base.OnModelCreating(modelBuilder);
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal MoneyReceived { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }

    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int PaidById { get; set; }
        public User PaidBy { get; set; }
    }
}
