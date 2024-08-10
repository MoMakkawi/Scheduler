namespace Scheduler.Endpoints;

public interface IEndpointDefinition
{
    void DefineEndpoints(IEndpointRouteBuilder app);
}
