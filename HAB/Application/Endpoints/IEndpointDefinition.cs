using JetBrains.Annotations;

namespace Application.Endpoints;

/// <summary>
/// An endpoint definition
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IEndpointDefinition
{
    /// <summary>
    /// Defines an endpoint
    /// </summary>
    static abstract void Define(IEndpointRouteBuilder builder);
}