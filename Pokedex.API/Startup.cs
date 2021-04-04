using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pokedex.API.Mappers;
using Pokedex.API.Models;
using Pokedex.Application;
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

            services.AddHttpClient<IReader<TranslationRequest, TranslationResult>, FunTranslationClient>();

            services.AddScoped<IMapper<Pokemon, PokemonDto>, PokemonMapper>();
            services.AddScoped<IMapper<PokemonSpecies, Pokemon>, PokemonSpeciesMapper>();
            services.AddScoped<IPokemonService, PokemonService>();

            services.AddScoped<IMapper<Pokemon, TranslatedPokemonDto>, TranslatedPokemonMapper>();
            services.AddScoped<ITranslationService, TranslationService>();
            services.AddScoped<ITranslatorFactory, TranslatorFactory>();
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
