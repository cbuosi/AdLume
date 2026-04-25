using Serilog;

namespace AdLumeClient.Classes;

public class InternetCheck
{
    public static async Task<bool> TemInternetAsync()
    {
        try
        {

            //Log.Information("TemInternetAsync");

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(1);

            var response = await client.GetAsync("https://www.google.com");

            return response.IsSuccessStatusCode;

        }
        catch
        {
            return false;
        }
    }
}
