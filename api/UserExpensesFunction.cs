using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MonthlyExpenses.Api.Interfaces;

namespace MonthlyExpenses.Api
{
    public class UserExpensesFunction
    {
        private readonly IRepository repository;
        private readonly IAuthenticator authenticator;

        public UserExpensesFunction(IRepository repository, IAuthenticator authenticator)
        {
            this.repository = repository;
            this.authenticator = authenticator;
        }

        [FunctionName("monthexpenses")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            var principal = await authenticator.GetClaimsPrincipal(req);
            if (!principal.Identity.IsAuthenticated || principal.IsInRole("authenticated"))
            {
                return new UnauthorizedResult();
            }

            // string name = req.Query["name"];

            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // name = name ?? data?.name;

            // string responseMessage = string.IsNullOrEmpty(name)
            //     ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //     : $"Hello, {name}. This HTTP triggered function executed successfully.";

            var data = await repository.GetUserExpenses();
            return new JsonResult(data);
        }
    }
}
