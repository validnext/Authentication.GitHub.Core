using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Authentication.GitHub.Core
{
    internal class GitHubHandler : OAuthHandler<GitHubOptions>
    {
        public GitHubHandler(IOptionsMonitor<GitHubOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(
            ClaimsIdentity identity,
            Microsoft.AspNetCore.Authentication.AuthenticationProperties properties,
            OAuthTokenResponse tokens)
        {
            var address = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, "access_token", tokens.AccessToken);

            var response = await Backchannel.GetAsync(address, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("Произошла ошибка при получении профиля пользователя: удаленный сервер " +
                                "вернул {Status} ответ со следующей информацией: {Headers} {Body}.",
                                response.StatusCode,
                                response.Headers.ToString(),
                                await response.Content.ReadAsStringAsync());

                throw new HttpRequestException("Произошла ошибка при получении профиля пользователя.");
            }

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            var user = (JObject)payload;

            identity.AddOptionalClaim(ClaimTypes.NameIdentifier, user.Value<string>("id"), Options.ClaimsIssuer)
                    .AddOptionalClaim(ClaimTypes.Name, user.Value<string>("login"), Options.ClaimsIssuer)
                    .AddOptionalClaim(ClaimTypes.Uri, user.Value<string>("url"), Options.ClaimsIssuer)
                    .AddOptionalClaim(ClaimTypes.Email, user.Value<string>("email"), Options.ClaimsIssuer);


            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, user);

            context.RunClaimActions();

            await Options.Events.CreatingTicket(context);

            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }
    }
}
