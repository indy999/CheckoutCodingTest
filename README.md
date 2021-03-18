# CheckoutCodingTest

This is the repository for building a payment gateway Api as part of the coding challenge for Checkout.com

# Project information

This is an .NET Core 5 API using xUnit and the in built Visual Studio Test runner.

There is no self signed certificate with the API

## Delivered Artifacts

- The Payment Gateway API has two endpoints that can be reached at `https://localhost:44300/api/payment/`
- A Http POST request will make a Payment Request
- A Http GET request retrieve information about historical payments

## Considerations/Assumptions

I did use the Checkout.com API documentation at https://api-reference.checkout.com/ as a reference at times.

There is an assumption that there is just one Merchant calling the payment gateway.  In the real world there would be an identifier of the merchant in the payment request.  In addition this would also be taken into cosideration what saving payment information the data store to save the payments by merchant Id.

There is no encryption of sensitive card information when saving in my in memory repository implementation.  This would obviously be an important consideration in the real implementation.

The bank stub and the gateway is using a relatively simple card details validation mechanism and hiding the contact with 3rd party information operations etc.  The card validation in a real implementation would be more secure and involved.  This could have its own transaction status potentially.  I have just used a simple "Authorized" and "Declined" status

The historical payment endponts are just using a simple ID.  In a real implementation this would have to take in more factors as different merchants and different reporting formats.

## Bonus features

## Application Logging

I used Serilog as a logging implementation writing to a log file

## Docker container

There is a simple Docker file to build and run the api in a container.  If there was more time I would consider how the container could be used to build the API, unit/smoke tests run as part of the application build and deployment pipeline

## Authentication

The authentication mechanism here is used is a simple API key mechanism and would be more secure and is passed in a custom header in this implementation.
