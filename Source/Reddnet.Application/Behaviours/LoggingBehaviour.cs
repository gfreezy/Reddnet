using MediatR;
using Microsoft.Extensions.Logging;
using Reddnet.Application.Validation;

namespace Reddnet.Application.Behaviours;

class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private ILogger<LoggingBehaviour<TRequest, TResponse>> logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        => this.logger = logger;

    public Task<TResponse> Handle(TRequest request,  RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        this.logger.LogInformation($"Request: {request}");
        var response = next().ContinueWith(
            response =>
            {
                if (response.Result is Result { IsError: true })
                {
                    this.logger.LogError($"Response: {response}");
                }
                else
                {
                    this.logger.LogInformation($"Response: {response}");
                }

                return response.Result;

            }, cancellationToken);
        return response;
    }

}
