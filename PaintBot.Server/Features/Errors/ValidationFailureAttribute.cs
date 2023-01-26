using System;

namespace PaintBot.Server.Features.Errors;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public class ValidationFailureAttribute : Attribute
{
    public ErrorCode ErrorCode { get; }

    public ValidationFailureAttribute(ErrorCode errorCode) =>
        ErrorCode = errorCode;
}
