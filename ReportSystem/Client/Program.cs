using Microsoft.AspNetCore.SignalR.Client;

const string url = "http://localhost:5000/logs";

await using var connection = new HubConnectionBuilder().WithUrl(url).Build();


await connection.StartAsync();

await foreach (var date in connection.StreamAsync<DateTime>("GetDate"))
{
    Console.WriteLine(date);
}
