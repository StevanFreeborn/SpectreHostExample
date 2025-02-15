# Spectre Host Example

This is a small example of how I'm approaching writing up the standard .NET hosting model with Spectre.Console.Cli's dependency injection system. This way I can use the host to configure my app, but then make sure Spectre.Console.Cli uses the host's services collection to resolve dependencies.

