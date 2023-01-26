using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PaintBot.Server.Features.Errors;

public static class ValidationResponseFactory
{
    public static ApiBehaviorOptions AddValidationErrorResponseFilter(this ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = ValidationErrorResponseFilter;
        return options;
    }

    public static IActionResult ValidationErrorResponseFilter(ActionContext context)
    {
        var validationFailure = GetValidationFailureFromContext(context);
        var errorResponse = MakeErrorResponseFromValidationFailure(validationFailure);
        return new JsonResult(errorResponse)
        {
            StatusCode = errorResponse.Code.ToStatusCode(),
        };
    }

    private static ValidationFailureAttribute? GetValidationFailureFromContext(ActionContext context)
    {
        var invalidParamName = context.ModelState.FirstOrDefault(kvp => IsModelStateInvalid(kvp.Value)).Key;
        if (invalidParamName is null)
        {
            return null;
        }
        return GetValidationFailureFromParam(context.ActionDescriptor, invalidParamName) ??
            GetValidationFailureFromPropertiesOnTypes(
                context.ActionDescriptor.Parameters
                    .Select(paramDesc => paramDesc.ParameterType),
                invalidParamName);
    }

    private static bool IsModelStateInvalid(ModelStateEntry? modelState) =>
        modelState?.ValidationState == ModelValidationState.Invalid;

    private static ValidationFailureAttribute? GetValidationFailureFromParam(ActionDescriptor actionDesc, string name)
    {
        var paramDesc = GetParameterDescriptorWithName(actionDesc, name);
        if (paramDesc is ControllerParameterDescriptor controllerParamDesc)
        {
            if (GetValidationFailureFromParameterInfo(controllerParamDesc.ParameterInfo) is ValidationFailureAttribute validationFailure)
            {
                return validationFailure;
            }
        }
        return paramDesc?.ParameterType.GetCustomAttribute<ValidationFailureAttribute>();
    }

    private static ParameterDescriptor? GetParameterDescriptorWithName(ActionDescriptor actionDesc, string name) =>
        actionDesc.Parameters.FirstOrDefault(param => param.Name == name);

    private static ValidationFailureAttribute? GetValidationFailureFromParameterInfo(ParameterInfo paramInfo) =>
        paramInfo.GetCustomAttribute<ValidationFailureAttribute>();

    private static ValidationFailureAttribute? GetValidationFailureFromPropertiesOnTypes(IEnumerable<Type>? types, string name) =>
        types?.Select(type => GetValidationFailureFromPropertiesOnType(type, name))
            .FirstOrDefault(v => v is not null);

    private static ValidationFailureAttribute? GetValidationFailureFromPropertiesOnType(Type? type, string name)
    {
        if (type?.GetProperty(name) is PropertyInfo propInfo)
        {
            return propInfo.GetCustomAttribute<ValidationFailureAttribute>() ??
                GetValidationFailureFromType(propInfo.PropertyType);
        }
        return GetValidationFailureFromPropertiesOnTypes(type?.GetProperties().Select(propInfo => propInfo.PropertyType), name);
    }

    private static ValidationFailureAttribute? GetValidationFailureFromType(Type? type) =>
        type?.GetCustomAttribute<ValidationFailureAttribute>();

    private static ErrorResponse MakeErrorResponseFromValidationFailure(ValidationFailureAttribute? validationFailure)
    {
        if (validationFailure is not null)
        {
            return new ErrorResponse(validationFailure.ErrorCode);
        }
        else
        {
            return new ErrorResponse(ErrorCode.ParametersInvalid);
        }
    }
}
