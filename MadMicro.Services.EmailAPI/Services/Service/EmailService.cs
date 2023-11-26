using MadMicro.Services.EmailAPI.DataContext;
using MadMicro.Services.EmailAPI.Message;
using MadMicro.Services.EmailAPI.Models;
using MadMicro.Services.EmailAPI.Models.DTO;
using MadMicro.Services.EmailAPI.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MadMicro.Services.EmailAPI.Services.Service;

public class EmailService : IEmailService
{
    private readonly DbContextOptions<AppDbContext> _dbContext;

    public EmailService(DbContextOptions<AppDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EmailCartAndLog(CartDTO cartDTO)
    {
       
        StringBuilder message = new StringBuilder();

        message.AppendLine("<br/>Cart Email Requested");
        message.AppendLine($"<br/>Total{cartDTO.CartHeaders.CartTotal}");
        message.Append("<br/>");
        message.Append("<ul>");


        foreach (var item in cartDTO.CartDetails)
        {
            message.Append("<li>");
            message.Append($"{item.Product.Name} x {item.Count}");
            message.Append("</li>");
        }
        message.Append("</ul>");

        await LogAndEmail(message.ToString(), cartDTO.CartHeaders.Email);
    }

    public async Task LogOrderPlaced(OrderConfirmation rewardMessage)
    {

        string message = $"New Order Placed. <br/> Order ID : {rewardMessage.OrderId} ";
        await LogAndEmail(message, "vess1994@gmail.com");
       
    }

    public async Task RegisterUserEmailAndLog(string email)
    {
        string message = $"User Register Success. <br/> Email : {email}";
        await LogAndEmail(message, "vess1994@gmail.com");
    }

    private async Task<bool> LogAndEmail(string message, string email)
    {
        try
        {
            var emailLog = new EmailLogger()
            {
                Email = email,  
                EmailSent = DateTime.Now,
                Message = message
            };

            await using var _db = new AppDbContext(_dbContext);
            await _db.EmailLoggers.AddAsync(emailLog); 
            await _db.SaveChangesAsync();   

            return true;
        }
        catch (Exception)
        {

            return false;
            throw;
        }
    }
}
