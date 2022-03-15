using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreApi.Crosscutting.Exceptions;
using NetCoreApp.Infrastructure.Data;
using System;
using System.Net;

namespace NetCoreApp.API.Configuration
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddExceptionModule(this IServiceCollection @this, IHostEnvironment environment)
        {
            @this
               .AddProblemDetails(setup =>
               {
                   setup.IncludeExceptionDetails = (_context, _exception) => !environment.IsDevelopment();

                   setup.GetTraceId = (context) => Guid.NewGuid().ToString();

                   setup.OnBeforeWriteDetails = (context, problemDetail) =>
                   {
                       problemDetail.Instance = context.Request.Path;
                   };

                   setup.Map<ClientErrorException>(e => new Microsoft.AspNetCore.Mvc.ProblemDetails
                   {
                       Title = e.Message,
                       Detail = e.Thrower(),
                       Type = e.GetType().FullName.Replace('.', '-'),
                       Status = e.StatusCode
                   });

                   setup.Map<Exception>(e => new Microsoft.AspNetCore.Mvc.ProblemDetails
                   {
                       Title = e.Message,
                       Detail = e.Thrower(),
                       Type = e.GetType().FullName.Replace('.', '-'),
                       Status = (int)HttpStatusCode.InternalServerError
                   });
               });

            return @this;
        }

        public static IApplicationBuilder UserDatabaseMigration(this IApplicationBuilder @this, IServiceProvider serviceProvider, IWebHostEnvironment environment)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (environment.IsDevelopment()) context.Database.Migrate();
            }

            return @this;
        }
    }
}
