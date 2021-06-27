/* using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ifttthandler.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate next;

        public AuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration config)
        {
            string extractedApiKey = String.Empty;
            context.Request.EnableBuffering(bufferThreshold: 1024 * 45, bufferLimit: 1024 * 100);

            using (var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: (int?)context.Request.ContentLength ?? 0,
                leaveOpen: true))
            {
                extractedApiKey = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            if (String.IsNullOrEmpty(extractedApiKey)) 
            {
                context.Response.StatusCode = 401;
                return;
            }

            var appSettings = config;

            var apiKey = appSettings["ApiKey"] ?? throw new InvalidOperationException("ApiKey was not found in configuration");
            var extractedApiKeyString = extractedApiKey.ToString();

            if (!apiKey.Equals(extractedApiKeyString))
            {
                context.Response.StatusCode = 401;
                return;
            }

            await next(context);
        }
    }
} */