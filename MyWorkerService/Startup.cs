﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", async context =>
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Hola DevOps" }));
            });

            endpoints.MapPost("/sms", async context =>
            {
                var client = app.ApplicationServices.GetRequiredService<IHttpClientFactory>().CreateClient();

                var phone = context.Request.Headers["phone"].ToString();
                var message = context.Request.Headers["message"].ToString();
                var apiKey = context.Request.Headers["x-api-key"].ToString();

                var request = new HttpRequestMessage(HttpMethod.Post, "https://yourwebservice.com/api");
                request.Headers.Add("phone", phone);
                request.Headers.Add("message", message);
                request.Headers.Add("x-api-key", apiKey);

                var response = await client.SendAsync(request);

                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
            });
        });
    }
}