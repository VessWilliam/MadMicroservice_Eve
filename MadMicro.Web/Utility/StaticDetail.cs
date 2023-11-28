namespace MadMicro.Web.Utility
{
    public class StaticDetail
    {
        public static string CouponAPIBase {  get; set; }   
        public static string ProductAPIBase { get; set; }
        public static string AuthAPIBase {  get; set; }
        public static string ShopCartAPIBase { get; set; }
        public static string OrderAPIBase { get; set; }


        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_ReadyToPickup = "ReadyToPickup";
        public const string Status_Completed = "Completed";
        public const string Status_Refunded = "Refunded";
        public const string Status_Cancelled = "Cancelled";

        public const string RoleAdmin = "ADMIN";

        public const string RoleCustomer = "CUSTOMER";

        public const string TokenCookie = "JWTtoken";
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE,
        }

    

    }
}
