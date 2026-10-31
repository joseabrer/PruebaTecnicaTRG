// Code Analysis usa este archivo para mantener los atributos SuppressMessage
// que se aplican a este proyecto.
// Las supresiones a nivel de proyecto no tienen destino o se les da
// un destino concreto, limitado a un espacio de nombres, tipo, miembro, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Security Hotspot", "S4792:Configuring loggers is security-sensitive", Justification = "Is already been controlled in the code")]
[assembly: SuppressMessage("Security Hotspot", "S4507:Make sure this debug feature is deactivated before delivering the code in production.", Justification = "Is already been controlled in the code")]
[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "For avoid error SA1003", Scope = "member", Target = "~M:GtMotive.Estimate.Microservice.Host.Infrastructure.Swagger.IdentityServerApiSecurityOperationFilter.Apply(Microsoft.OpenApi.Models.OpenApiOperation,Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext)")]
[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "For avoid error SA1003", Scope = "member", Target = "~F:GtMotive.Estimate.Microservice.Host.Infrastructure.Swagger.IdentityServerApiSecurityOperationFilter.OpenApiSecuritySchemesValues")]
