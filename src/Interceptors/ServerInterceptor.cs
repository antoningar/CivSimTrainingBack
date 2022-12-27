using Grpc.Core;
using Grpc.Core.Interceptors;

namespace cst_back.Interceptors
{
    public class ServerInterceptor : Interceptor
    {
        private ILogger<ServerInterceptor> _logger;

        public ServerInterceptor(ILogger<ServerInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation($"Starting call. Type: {context.Method}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");

            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error thrown by {context.Method}.");
                throw;
            }
        }
    }
}
