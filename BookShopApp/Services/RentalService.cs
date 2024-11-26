using BookShopApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;


namespace BookShopApp.Services
{
    public class RentalService
    {
        private readonly string _connectionString;
        private readonly DatabaseHelper _dbHelper;
        public RentalService(string connectionString)
        {
            _connectionString = connectionString;
            _dbHelper = new DatabaseHelper(connectionString);
        }

        public List<Rental> GetRentals()
        {
            var rentals = new List<Rental>();
            var dataTable = _dbHelper.ExecuteStoredProcedure("GetRentalList");

            foreach (DataRow row in dataTable.Rows)
            {
                rentals.Add(new Rental
                {
                    RentalId = Convert.ToInt32(row["RentalId"]),
                    BookTitle = Convert.ToString(row["BookTitle"]),
                    UserName = Convert.ToString(row["ClientName"]),
                    RentalDate = Convert.ToDateTime(row["RentalDate"]),
                    DueDate = Convert.ToDateTime(row["DueDate"]),
                    ReturnDate = row["ReturnDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ReturnDate"]),
                    PenaltyFee = Convert.ToDecimal(row["PenaltyFee"])
                });
            }

            return rentals;
        }

        public bool RentBook(int bookId, int clientId, DateTime dueDate)
        {
            if (IsUserBlocked(clientId))
            {
                Console.WriteLine("client is blocked");
                return false;
            }
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string checkBookQuery = @"
                    SELECT (Stock - COUNT(RentalId)) AS AvailableCopies 
                    FROM Books LEFT JOIN Rentals ON Books.BookId = Rentals.BookId 
                    AND Rentals.ReturnDate IS NULL WHERE Books.BookId = @BookId 
                    GROUP BY Books.BookId, Books.Stock";
                using (SqlCommand checkBookCommand = new SqlCommand(checkBookQuery, connection))
                {
                    checkBookCommand.Parameters.AddWithValue("@BookId", bookId);

                    int availableCopies = (int)checkBookCommand.ExecuteScalar();
                    if (availableCopies <= 0)
                    {
                        Console.WriteLine("no copy of the book is available for rent");
                        return false;
                    }
                }

                string rentBookQuery = @"
                    INSERT INTO Rentals (ClientId, BookId, RentalDate, DueDate)
                    VALUES (@ClientId, @BookId, @RentalDate, @DueDate)";
                using (SqlCommand rentBookCommand = new SqlCommand(rentBookQuery, connection))
                {
                    rentBookCommand.Parameters.AddWithValue("@ClientId", clientId);
                    rentBookCommand.Parameters.AddWithValue("@BookId", bookId);
                    rentBookCommand.Parameters.AddWithValue("@RentalDate", DateTime.Now);
                    rentBookCommand.Parameters.AddWithValue("@DueDate", dueDate);

                    int rowsAffected = rentBookCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("book rented successfully");
                        return true;
                    }
                }
                return false;
            }
        }

        private bool IsUserBlocked(int clientId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string checkUserQuery = @"
                    SELECT IsBlocked 
                    FROM Clients 
                    WHERE ClientId = @ClientId";
                using (SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection))
                {
                    checkUserCommand.Parameters.AddWithValue("@ClientId", clientId);

                    return (bool)checkUserCommand.ExecuteScalar();
                }
            }
        }

        public void CalculatePenalties()
        {
            _dbHelper.ExecuteStoredProcedure("CalculatePenalties");
        }
    }
}
