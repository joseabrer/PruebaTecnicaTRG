// Code Analysis usa este archivo para mantener los atributos SuppressMessage
// que se aplican a este proyecto.
// Las supresiones a nivel de proyecto no tienen destino o se les da
// un destino concreto, limitado a un espacio de nombres, tipo, miembro, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "xUnit requires public test classes and collection definitions (xUnit1000, xUnit1027), and the fixtures and base classes they use must be at least as accessible.", Scope = "namespaceanddescendants", Target = "~N:GtMotive.Estimate.Microservice.InfrastructureTests")]
