using AspireDemoApp.Shared.Models;

namespace AspireDemoApp.Web;

public class ApiClient(HttpClient httpClient)
{
    public async Task<Person[]> GetPeopleListAsync(CancellationToken cancellationToken = default)
    {
        var people = await httpClient.GetFromJsonAsync<Person[]>("api/people", cancellationToken: cancellationToken);
        return people ?? [];
    }

    public async Task SavePersonAsync(CancellationToken cancellationToken = default)
    {
        await httpClient.PostAsync("api/people",null, cancellationToken: cancellationToken);
    }
}
