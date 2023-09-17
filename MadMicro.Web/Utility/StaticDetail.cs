namespace MadMicro.Web.Utility
{
    public class StaticDetail
    {
        public static string CouponAPIBase {  get; set; }   
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE,
        }
    }
}
