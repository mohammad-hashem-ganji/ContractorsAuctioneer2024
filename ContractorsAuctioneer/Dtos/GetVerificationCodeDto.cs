namespace ContractorsAuctioneer.Dtos
{
    /// <summary>
    /// داده‌های ورودی برای تأیید کد احراز هویت دو مرحله‌ای.
    /// </summary>
    public class GetVerificationCodeDto
    {
        /// <summary>
        /// کد تأیید ارسال شده به کاربر.
        /// </summary>
        public string VerificationCode { get; set; }

        /// <summary>
        /// کد ملی کاربر.
        /// </summary>
        public string Ncode { get; set; }

        /// <summary>
        /// شماره تلفن کاربر.
        /// </summary>
        public string PhoneNumber { get; set; }
    }

}
