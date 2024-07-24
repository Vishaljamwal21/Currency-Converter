# Final-Test
## Currency Converter with Real-Time Exchange Rates
## Project Overview
- The Currency Converter application is designed to convert amounts between different currencies using real-time exchange rates. Built with .NET 8.0 using the MVC framework, the application integrates with a currency exchange API to provide accurate and up-to-date conversion rates. Users can select base and target currencies, input amounts for conversion, and view historical exchange rates. Additionally, registered users can save their favorite currency pairs for quick access.

* Project Setup
* Prerequisites
- .NET 8.0 SDK
- SQL Server
- Access to a free currency exchange API (e.g., ExchangeRate-API, Open Exchange Rates)
## Installation
* Clone the Repository:
- git clone https://github.com/Vishaljamwal21/Final-Test.git
## Set Up the Database:
* Apply database migrations
- update-database
* Run the Application.

## Usage Instructions
* User Authentication:
1. Registration and Login: 
Users can register and log in using the built-in ASP.NET Identity system. This is required to save favorite currency pairs.
* Currency Conversion:
1. Select Currencies: Use the dropdown menus to choose the base and target currencies.
2. Input Amount: Enter the amount in the base currency to be converted.
3. View Conversion: The converted amount will be displayed immediately.
* Historical Exchange Rates: Access historical conversion rates with live updates.
*  Favorite Currency Pairs:
1. Save Favorite Pairs: Logged-in users can save their favorite currency pairs for quick future access.
2. Manage Favorites: View and manage saved currency pairs in the user profile.
## Error Handling
* API Failures: Handle errors gracefully with user-friendly messages if the currency exchange API fails or returns invalid data.
* User Inputs: Validate user inputs and display appropriate error messages for incorrect or missing data.
* Data Parsing: Ensure robust error handling for data parsing issues from the API.
## Assumptions
* API Access: It is assumed that the API credentials and access are correctly set up in appsettings.json.
* Database Setup: The SQL Server instance is properly configured and accessible.
* Network: The application requires an internet connection to fetch real-time exchange rates and historical data.
## Tools and Technologies
* .NET 8.0: Framework used for application development.
* MVC: Model-View-Controller pattern for building the front end.
* Entity Framework Core: ORM for database access and management.
* SQL Server: Database for storing user data and conversion history.
* Currency Exchange API: Provides real-time and historical exchange rates (ExchangeRate-API or Open Exchange Rates).
* ASP.NET Identity: For implementing user authentication and authorization.