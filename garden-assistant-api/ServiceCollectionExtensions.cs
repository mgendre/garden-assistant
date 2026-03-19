using GardenAssistant.Data.Seeders;
using GardenAssistant.Services;
using GardenAssistant.Services.Interfaces;

namespace GardenAssistant;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISeeder, PlantSeeder>();
        services.AddScoped<ISeeder, AssociationSeeder>();
        services.AddScoped<ISeeder, GuildSeeder>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPlantService, PlantService>();
        services.AddScoped<IPlantAssociationService, PlantAssociationService>();
        services.AddScoped<IGuildService, GuildService>();
        services.AddScoped<IUserPlantService, UserPlantService>();

        return services;
    }
}
