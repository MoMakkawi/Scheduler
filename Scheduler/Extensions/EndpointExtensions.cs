using Scheduler.Endpoints;

namespace Scheduler.Extensions;

public static class EndpointExtensions
{
    public static void AddEndpoints(this WebApplication app)
    {
        typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IEndpointDefinition).IsAssignableFrom(t))
            .Select(Activator.CreateInstance)
            .Cast<IEndpointDefinition>()
            .ToList()
            .ForEach(endpointDefinition => endpointDefinition.DefineEndpoints(app));
    }
}
