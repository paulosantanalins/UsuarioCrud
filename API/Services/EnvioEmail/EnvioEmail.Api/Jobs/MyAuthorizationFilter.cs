using Hangfire.Dashboard;

namespace EnvioEmail.Api.Jobs
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return true;
        }
    }
}
