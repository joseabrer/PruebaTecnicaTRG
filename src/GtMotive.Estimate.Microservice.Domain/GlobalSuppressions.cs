// Code Analysis usa este archivo para mantener los atributos SuppressMessage
// que se aplican a este proyecto.
// Las supresiones a nivel de proyecto no tienen destino o se les da
// un destino concreto, limitado a un espacio de nombres, tipo, miembro, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Security Hotspot", "S4834:Controlling permissions is security-sensitive", Justification = "Needed for Microsoft.AspNetCore.Authorization.IAuthorizationService", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.Domain.Interfaces.IAuthorizationService")]
[assembly: SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "T is necessary for dependency injection.", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.Domain.Interfaces.IAppLogger`1")]
[assembly: SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Domain exceptions are always raised with a business message; parameterless and inner-exception constructors would allow them to be raised without one.", Scope = "namespaceanddescendants", Target = "~N:GtMotive.Estimate.Microservice.Domain")]
