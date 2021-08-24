using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Supabase.Gotrue;
using uptimey.Extensions;

namespace uptimey.Middlewares
{
    public class UserAccesorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<SupabaseSettings> _supabaseSettings;

        public UserAccesorMiddleware(RequestDelegate next, IOptions<SupabaseSettings> supabaseSettings)
        {
            _next = next;
            _supabaseSettings = supabaseSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                await _next(context);
                return;
            }

            var headers = new Dictionary<string, string>
            {
                ["Authorization"] = "Bearer " + token,
                ["apikey"] = _supabaseSettings.Value.Key
            };
            var api = new Api($"{_supabaseSettings.Value.Url}/auth/v1", headers);
            try
            {
                var user = await api.GetUser(token);
                if (user != null)
                {
                    context.Items["User"] = user;
                }
                else
                {
                    await context.ForbidAsync();
                    return;
                }
            }
            catch (Exception)
            {
                await context.ForbidAsync();
                return;
            }

            await _next(context);
        }
    }
}