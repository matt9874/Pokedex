using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pokedex.API.Mappers;
using Pokedex.API.Models;
using Pokedex.Application;
using Pokedex.Application.Configuration;
using Pokedex.Application.Interfaces;
using Pokedex.Application.Pokemon;
using Pokedex.Application.Pokemon.PokeapiDtos;
using Pokedex.Application.Translation;
using Pokedex.Application.Translation.Dtos;
using Pokedex.Domain;
using Pokedex.Infrastructure.WebRequests;

namespace Pokedex.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<HttpClientOptions>(Configuration.GetSection(
                                        nameof(HttpClientOptions)));

            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
            });

            services.AddHttpClient<IReader<string, PokemonSpecies>, PokeapiSpeciesReader>()
                .ConfigurePrimaryHttpMessageHandler(handler =>
                    new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.Brotli
                    });

            services.AddScoped<IMapper<Pokemon, PokemonDto>, PokemonMapper>();
            services.AddScoped<IMapper<PokemonSpecies, Pokemon>, PokemonSpeciesMapper>();
            services.AddScoped<IPokemonService, PokemonService>();

            services.AddScoped<IMapper<Pokemon, TranslatedPokemonDto>, TranslatedPokemonMapper>();
            services.AddScoped<ITranslationService, TranslationService>();
            services.AddScoped<ITranslationTypeDecider, TranslationTypeDecider>();
            
            services.AddHttpClient("funTranslation");
            services.AddScoped<YodaTranslationClient>();
            services.AddScoped<ShakespeareTranslationClient>();

            services.AddScoped<Func<TranslationType, IReader<TranslationRequest, TranslationResult>>>(sp =>
                translationType =>
                {
                    return translationType switch
                    {
                        TranslationType.Yoda => sp.GetRequiredService<YodaTranslationClient>(),
                        TranslationType.Shakespeare => sp.GetRequiredService<ShakespeareTranslationClient>(),
                        _ => throw new NotSupportedException($"TranslationType of {translationType} is not supported")
                    };
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
