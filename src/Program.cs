// Build the app
var app = Setup.CreateApp(args);

// Map all the endpoints
app.MapPost("/donate", Endpoints.CreateDonation);
app.MapGet("/donation/{donationId:guid}", Endpoints.GetDonationStatus);

// run the app
app.Run();
