// ReSharper disable AccessToDisposedClosure
using NBomber.CSharp;
using NBomber.Http.CSharp;

using var httpClient = new HttpClient();

var url = "https://localhost:7294";

var scenario = Scenario.Create("default", async _ =>
    {
        var request = Http.CreateRequest(HttpMethod.Post.Method, $"{url}/entity/");
        // to ensure load gets distributed over all instances
        request.Headers.Connection.Add("close");
        return await Http.Send(httpClient, request);
    })
    .WithoutWarmUp()
    .WithLoadSimulations(Simulation.Inject(50, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30)));
    
NBomberRunner
    .RegisterScenarios(scenario)
    .Run();
