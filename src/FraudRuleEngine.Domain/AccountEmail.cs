namespace FraudRuleEngine.Domain
{
    public sealed record AccountEmail
    {
        public string Email { get; private set; } = default!;
        public string AccountId { get; private set; } = default!;

        private AccountEmail() { } // Private constructor to enforce the use of the factory method

        public static AccountEmail Create(string email, string accountId)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            if (string.IsNullOrWhiteSpace(accountId)) throw new ArgumentException("AccountId cannot be null or empty.", nameof(accountId));

            return new AccountEmail
            {
                Email = email,
                AccountId = accountId
            };
        }
    }
}
