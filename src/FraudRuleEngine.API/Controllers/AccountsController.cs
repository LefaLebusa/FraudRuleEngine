using System.ComponentModel.DataAnnotations;
using FraudRuleEngine.Core.Features.AccountEmails;
using FraudRuleEngine.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FraudRuleEngine.API.Controllers;

// Manages where fraud alerts for an account are sent. Kept separate from transaction
// submission so that only an authenticated caller can change an alert recipient.
[ApiController]
[Route("api/accounts")]
public class AccountsController(AddEmailCommandHandler addEmail, ILogger<AccountsController> logger) : ControllerBase
{
    /// <summary>Set the email address that receives fraud alerts for an account.</summary>
    [HttpPut("{accountId}/alert-email")]
    public async Task<IActionResult> SetAlertEmail(
        [StringLength(50, MinimumLength = 1)] string accountId,
        [FromBody] AlertEmailRequest request,
        CancellationToken cancellationToken)
    {
        await addEmail.Handle(new AddEmailCommand(AccountEmail.Create(request.Email, accountId)), cancellationToken);

        // The email address itself is not logged, only the account it was set for
        logger.LogInformation("Alert email updated for account {AccountId}", accountId);
        return NoContent();
    }
}

public sealed record AlertEmailRequest(
    [property: Required, EmailAddress, StringLength(254)] string Email);
