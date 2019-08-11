using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Checkout.PaymentGateway.Api.Tests.Integration.Services
{
    internal class TestOutputHelperLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestOutputHelperLoggerProvider(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void Dispose()
        {

        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestOutputHelperLogger(categoryName, _testOutputHelper);
        }

        private class TestOutputHelperLogger : ILogger
        {
            private readonly string _categoryName;
            private readonly ITestOutputHelper _testOutputHelper;

            public TestOutputHelperLogger(string categoryName, ITestOutputHelper testOutputHelper)
            {
                _categoryName = categoryName;
                _testOutputHelper = testOutputHelper;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _testOutputHelper.WriteLine($"{_categoryName} - {logLevel}: {formatter(state, exception)}. EventId: {eventId}");
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public IDisposable BeginScope<TState>(TState state)
            {
                return new NonDisposable();
            }

            private class NonDisposable : IDisposable
            {
                public void Dispose() { }
            }
        }
    }
}