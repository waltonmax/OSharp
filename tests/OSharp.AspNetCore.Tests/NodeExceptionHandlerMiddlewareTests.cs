﻿using System;
using System.Threading.Tasks;

using Shouldly;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Xunit;


namespace OSharp.AspNetCore.Tests
{
    public class NodeExceptionHandlerMiddlewareTests
    {
        private readonly ServiceProvider _provider;

        public NodeExceptionHandlerMiddlewareTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            _provider = services.BuildServiceProvider();
        }

        [Fact()]
        public void NodeExceptionHandlerMiddlewareTest()
        {
            var middleware = ActivatorUtilities.CreateInstance<NodeExceptionHandlerMiddleware>(_provider,
                new RequestDelegate(c => Task.CompletedTask));
            Assert.NotNull(middleware);
        }

        [Fact()]
        public async Task InvokeAsyncTest1()
        {
            HttpContext context = new DefaultHttpContext();
            context.Request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            var middleware = ActivatorUtilities.CreateInstance<NodeExceptionHandlerMiddleware>(_provider,
                new RequestDelegate(c => throw new Exception("a exception")));
            await middleware.InvokeAsync(context);

            Assert.Equal(500, context.Response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", context.Response.ContentType);
        }

        [Fact()]
        public async Task InvokeAsyncTest2()
        {
            HttpContext context = new DefaultHttpContext();
            context.Request.Headers.Add("Content-Type", "application/json; charset=utf-8");

            var middleware = ActivatorUtilities.CreateInstance<NodeExceptionHandlerMiddleware>(_provider,
                new RequestDelegate(c => throw new Exception("a exception")));
            await middleware.InvokeAsync(context);

            context.Response.StatusCode.ShouldBe(500);
            context.Response.ContentType.ShouldBe("application/json; charset=utf-8");
        }

        [Fact()]
        public async Task InvokeAsyncTest3()
        {
            HttpContext context = new DefaultHttpContext();

            var middleware = ActivatorUtilities.CreateInstance<NodeExceptionHandlerMiddleware>(_provider,
                new RequestDelegate(c => throw new Exception("a exception")));

            Exception ex = await Assert.ThrowsAsync<Exception>(() => middleware.InvokeAsync(context));
            ex.Message.ShouldBe("a exception");
        }

    }
}