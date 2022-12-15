using System.Reflection;
using DeepDive.CustomHttpServer.ServerHost.Interfaces;

namespace DeepDive.CustomHttpServer.ServerHost.Handlers;

public sealed class ControllersHandler : IHandler
{
    private const string ControllerPostfix = "Controller";
    private const string HomePageEndpoint = "Index";

    public ControllersHandler(Assembly controllersAssembly)
    {
        var controllersMethods = controllersAssembly.GetTypes()
            .Where(x => typeof(IController).IsAssignableFrom(x))
            .SelectMany(controller => controller.GetMethods().Select(method => new
            {
                Controller = controller,
                Method = method,
            }))
            .ToDictionary(
                key => GetPath(key.Controller, key.Method),
                value => GetEndpointMethod(value.Controller, value.Method)
            );
    }

    private Func<object?> GetEndpointMethod(Type controller, MethodInfo method)
    {
        return () => method.Invoke(Activator.CreateInstance(controller), Array.Empty<object>());
    }

    private string GetPath(Type controller, MethodInfo method)
    {
        var controllerName = controller.Name;
        if (controllerName.EndsWith(ControllerPostfix, StringComparison.InvariantCultureIgnoreCase))
        {
            controllerName = controllerName.Substring(0, controllerName.Length - ControllerPostfix.Length);
        }

        if (method.Name.Equals(HomePageEndpoint, StringComparison.InvariantCultureIgnoreCase))
        {
            return $"/{controllerName}";
        }

        return $"/{controllerName}/{method.Name}";
    }

    public Task HandleAsync(Stream networkStream, Request request)
    {
        return Task.CompletedTask;
    }
}