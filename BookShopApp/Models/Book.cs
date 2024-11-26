using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopApp.Models
{
    public class Book
    {
        public int RentalId { get; set; }
        public string BookTitle { get; set; }
        public string UserName { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal PenaltyFee { get; set; }
    }
}
