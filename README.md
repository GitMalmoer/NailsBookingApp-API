# NailsBookingApp-API
Nails Booking API

## To do very soon:
- Complete code architecture refactor with CQRS and MediatR pattern
- Make code loosely coupled. For test purposes evrtything is now tightly coupled in controllers.
- Refactor ApiResponse - This class is repeated almost in every request. This class is not necessary because im going to base on status codes and middlewares to handle api response.

Frontend: https://nailsbookingapp.netlify.app/

Backend: https://nailsbookingapi.azurewebsites.net/index.html

## Screenshot
![nailsbookingapi](https://github.com/GitMalmoer/NailsBookingApp-API-Public/assets/113827015/cd47243c-7713-4d4d-ab6a-1f5b5035ac58)

## First run

1.First add environmental variables

update this to appsettings.json file:
```csharp
  {
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnectionString": "YOUR_DATA_HERE",
    "StorageAccount": "YOUR_DATA_HERE"
  },
  "ApiSettings": {
    "Secret": "YOUR_DATA_HERE"
  },
  "EmailConfiguration": {
    "From": "YOUR_DATA_HERE",
    "SmtpServer": "smtp.gmail.com",
    "Port": 465,
    "Username": "YOUR_DATA_HERE",
    "Password": "YOUR_DATA_HERE"
  },
  "QuestionRecipent": "YOUR_DATA_HERE"
}
```

2. apply update-database in package manager console
3. run
    
