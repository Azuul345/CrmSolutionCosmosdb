using System.Net;
using System.Net.Mail;
using CrmFunction.Models;
using Microsoft.Extensions.Configuration;

namespace CrmFunction.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendSellerNotificationAsync(Customer customer)
    {
        Console.WriteLine("[EmailService] Start SendSellerNotificationAsync");

        var sellerEmail = customer.ResponsibleSeller?.Email;
        if (string.IsNullOrWhiteSpace(sellerEmail))
        {
            Console.WriteLine("[EmailService] Seller email is empty, skipping.");
            return;
        }

        // Read Mailtrap settings from configuration
        var host = _config["MailtrapHost"];
        var portString = _config["MailtrapPort"];
        var user = _config["MailtrapUser"];
        var password = _config["MailtrapPassword"];
        var fromAddress = _config["MailtrapFrom"];

        Console.WriteLine($"[EmailService] Config Host={host}, Port={portString}, User={user}, From={fromAddress}");

        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(portString) ||
            string.IsNullOrWhiteSpace(user) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fromAddress))
        {
            Console.WriteLine("[EmailService] Mailtrap configuration is incomplete.");
            return;
        }

        if (!int.TryParse(portString, out var port))
        {
            Console.WriteLine("[EmailService] MailtrapPort is not a valid number.");
            return;
        }

        var subject = $"You are now responsible for customer {customer.Name}";
        var body =
            $"Hi {customer.ResponsibleSeller?.Name},\n\n" +
            $"You have been assigned as the responsible seller for the following customer:\n\n" +
            $"Name: {customer.Name}\n" +
            $"Title: {customer.Title}\n" +
            $"Phone: {customer.Phone}\n" +
            $"Email: {customer.Email}\n" +
            $"Address: {customer.Address}\n\n" +
            "Please reach out at your earliest convenience.\n";

        using var message = new MailMessage(fromAddress, sellerEmail)
        {
            Subject = subject,
            Body = body
        };

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, password),
            EnableSsl = true
        };

        try
        {
            Console.WriteLine($"[EmailService] Sending email to: {sellerEmail}");
            await client.SendMailAsync(message);
            Console.WriteLine("[EmailService] Email sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EmailService] Error while sending email: {ex.Message}");
        }
    }
}