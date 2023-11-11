namespace MadMicro.Web.Utility
{
    public class StaticDetail
    {
        public static string CouponAPIBase {  get; set; }   
        public static string ProductAPIBase { get; set; }
        public static string AuthAPIBase {  get; set; }
        public static string ShopCartAPIBase { get; set; }
        public static string OrderAPIBase { get; set; }


        public const string RoleAdmin = "Admin";

        public const string RoleCustomer = "Customer";

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
