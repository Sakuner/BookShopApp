using BookShopApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookShopApp
{
    internal static class Program
    {
        const string connectionString = "Server=(LocalDb)\\MSSQLLocalDB;Database=BookShopDb;Trusted_Connection=True;";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var rentalService = new RentalService(connectionString);
            var rentalList = rentalService.GetRentals();
            //var bookRentedSuccessfuly = rentalService.RentBook(1, 1, DateTime.Now.AddDays(15));
            rentalService.CalculatePenalties();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ViewForm(connectionString));
        }
    }
}
