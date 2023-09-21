namespace MadMicro.Web.Utility
{
    public class StaticDetail
    {
        public static string CouponAPIBase {  get; set; }   
        public static string AuthAPIBase {  get; set; }   
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE,
        }
    }
}
