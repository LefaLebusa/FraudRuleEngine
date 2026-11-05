using FluentValidation;
using FraudRuleEngine.API.Requests;
using FraudRuleEngine.API.Validation;
using FraudRuleEngine.Core.Features.AccountEmails;
using FraudRuleEngine.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FraudRuleEngine.API.Controllers;

// Manages where fraud alerts for an account are sent. Kept separate from transaction
// submission so that only an authenticated caller can change an alert recipient.
[ApiController]
[Route("api/accounts")]
public class AlertsForAccountsController(
    AddEmailCommandHandler addEmail,
    IValidator<AccountIdRequest> accountIdValidator,
    IValidator<AlertEmailRequest> alertEmailValidator,
    ILogger<AlertsForAccountsController> logger) : ControllerBase
{
    /// <summary>Set the email address that receives fraud alerts for an account.</summary>
    [HttpPut("{accountId}/alert-email")]
    public async Task<IActionResult> SetAlertEmail(
        string accountId,
        [FromBody] AlertEmailRequest request,
        CancellationToken cancellationToken)
    {
        var invalidAccount = await this.ValidateAsync(accountIdValidator, new AccountIdRequest(accountId), cancellationToken);
        if (invalidAccount is not null) return invalidAccount;

        var invalidEmail = await this.ValidateAsync(alertEmailValidator, request, cancellationToken);
        if (invalidEmail is not null) return invalidEmail;

        await addEmail.Handle(new AddEmailCommand(AccountEmail.Create(request.Email, accountId)), cancellationToken);

        // The email address itself is not logged, only the account it was set for
        logger.LogInformation("Alert email updated for account {AccountId}", accountId);
        return NoContent();
    }
}
