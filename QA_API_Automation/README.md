# QA_API_Automation Test Suite

This project is an automated API test suite for the ENSEK Energy test platform, built with .NET 8 and NUnit.

## Features
- Automated tests for ENSEK API test endpoints (energy, orders)
- Uses NUnit,FlUrl, FluentAssertions, and Allure for reporting


## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Allure Commandline](https://docs.qameta.io/allure/#_installing_a_commandline) (for HTML reports)

### Running Tests

1. Restore dependencies:
  
   dotnet restore

2. Build the project:

   dotnet build

3. Run all tests:
  
   dotnet test
  
4. (Optional) Generate Allure HTML report:
 - Install Allure Commandline if not already installed:
 - Navigate to the allure results directory:

   QA_API_Automation\bin\Debug\net8.0> allure serve

## Project Structure
- `Tests/` - Contains all test classes and helpers
- `Core/` - API client and Flurlhelper classes
- `Models/` - Data models for requests and responses
- `ApiConfig.cs` - API base URL configuration

## Customization
- Update `ApiConfig.cs` to change the API base URL
- Override `Username` and `Password` in test classes for different credentials



