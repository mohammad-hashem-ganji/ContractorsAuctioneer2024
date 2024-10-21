using System.ComponentModel.DataAnnotations;

namespace ContractorsAuctioneer.Dtos
{
    /// <summary>
    /// داده‌های ورودی برای ورود کاربر.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// کد ملی کاربر. (الزامی)
        /// </summary>
        [Required]
        public string Ncode { get; set; }

        /// <summary>
        /// شماره تلفن کاربر. (الزامی)
        /// </summary>
        [Required]
        public string PhoneNumber { get; set; }
    }

}
