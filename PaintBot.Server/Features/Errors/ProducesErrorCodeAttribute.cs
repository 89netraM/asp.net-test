using Microsoft.AspNetCore.Mvc;
using System;

namespace PaintBot.Server.Features.Errors;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ProducesErrorCodeAttribute : ProducesResponseTypeAttribute
{
    public ErrorCode ErrorCode { get; }

    public ProducesErrorCodeAttribute(ErrorCode errorCode) : base(typeof(ErrorResponse), errorCode.ToStatusCode()) =>
        ErrorCode = errorCode;
}
