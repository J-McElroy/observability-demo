using Common.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Common.Pipelines
{
    public class RandomErrorPipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<RandomErrorMiddleware>();
        }
    }
}