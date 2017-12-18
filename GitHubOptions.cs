using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Authentication.GitHub.Core
{
    public class GitHubOptions : OAuthOptions
    {
        public GitHubOptions()
        {
            ClaimsIssuer = GitHubDefaults.ClaimsIssuer;
            CallbackPath = new PathString(GitHubDefaults.CallbackPath);
            AuthorizationEndpoint = GitHubDefaults.AuthorizationEndpoint;
            TokenEndpoint = GitHubDefaults.TokenEndpoint;
            UserInformationEndpoint = GitHubDefaults.UserInformationEndpoint;

            Scope.Add("user");
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(ClientId))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Отсутствует {0}", nameof(ClientId)), nameof(ClientId));
            }

            if (string.IsNullOrEmpty(ClientSecret))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Отсутствует {0}", nameof(ClientSecret)), nameof(ClientSecret));
            }

            base.Validate();
        }
    }
}
