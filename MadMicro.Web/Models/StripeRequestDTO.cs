namespace MadMicro.Web.Models;

public class StripeRequestDTO
{
    public string? StripeSessionURL { get; set; }
    public string? StripeSessionID { get; set; }
    public string ApprovedURL { get; set; }
    public string CancelURL { get; set; }
    public OrderHeaderDTO OrderHeader { get; set; }
}
