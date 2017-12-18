namespace Authentication.GitHub.Core
{
    public static class GitHubDefaults
    {
        public const string ClaimsIssuer = "GitHub";
        public const string AuthenticationScheme = "GitHub";
        public static readonly string DisplayName = "GitHub";
        public static readonly string AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        public static readonly string TokenEndpoint = "https://github.com/login/oauth/access_token";
        public static readonly string UserInformationEndpoint = "https://api.github.com/user";
        public static readonly string CallbackPath = "/signin-github";
    }
}
