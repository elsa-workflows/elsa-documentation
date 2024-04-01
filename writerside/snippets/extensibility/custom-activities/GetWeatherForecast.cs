using Elsa.Extensions;
using Elsa.Workflows;

public class GetWeatherForecast : CodeActivity<WeatherForecast>
{
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var apiClient = context.GetRequiredService<IWeatherApi>();
        var forecast = await apiClient.GetWeatherAsync();
        context.SetResult(forecast);
    }
}