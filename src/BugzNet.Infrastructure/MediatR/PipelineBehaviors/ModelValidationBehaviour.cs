using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BugzNet.Core.Exceptions;
using BugzNet.Infrastructure.MediatR;
using BugzNet.Infrastructure.DataEF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.MediatR.PipelineBehaviors
{
    public class ModelValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : CommandResponse
    {
        private readonly IEnumerable<IValidator> _validators;

        public ModelValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public ModelValidationBehaviour()
        {
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse response = null;

            if (_validators.Any())
            {
                // in case of multiple validators for the same model
                // this pipeline will trust any model that passes atleast one validator 
                // in this case you should explicitly validate the model with the 
                // apropriate validator before passing it to MediatR
                if (_validators.All(v => !v.Validate(request).IsValid))
                {
                    var result = _validators.First().Validate(request);
                    if (!result.IsValid)
                    {
                        var errorMsg = string.Join(Environment.NewLine, result.Errors);

                        response = CreateErrorResponse(errorMsg);
                        if (response != null)
                            return response;
                    }
                }
            }

            try
            {
                response = await next();
            }
            catch (Exception ex) when (ex is DomainException || ex is ArgumentException)
            {
                response = CreateErrorResponse(ex.Message);
                if (response == null)
                    throw;

                return response;
            }

            return response;
        }

        private TResponse CreateErrorResponse(string message)
        {
            if (typeof(TResponse).IsAssignableFrom(typeof(CommandResponse)))
                return CommandResponse.WithError(message) as TResponse;
            else if (typeof(TResponse).IsAssignableFrom(typeof(CommandResponse<int>)))
                return CommandResponse<int>.WithError(message) as TResponse;
            else if (typeof(TResponse).IsAssignableFrom(typeof(CommandResponse<string>)))
                return CommandResponse<string>.WithError(message) as TResponse;
            else if (typeof(TResponse).IsAssignableFrom(typeof(CommandResponse<bool>)))
                return CommandResponse<bool>.WithError(message) as TResponse;
            return null;
        }
    }
}
