using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml.XPath;
using static System.Net.Mime.MediaTypeNames;

namespace PaintBot.Server.Features.Errors;

public class ErrorResponseDocumentation : IOperationFilter
{
	private readonly IOptions<JsonOptions> jsonOptions;
	private readonly XPathNavigator xmlNavigator;

	public ErrorResponseDocumentation(IOptions<JsonOptions> jsonOptions, XPathDocument xmlDoc)
	{
		this.jsonOptions = jsonOptions;
		xmlNavigator = xmlDoc.CreateNavigator();
	}

	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		foreach (var errorProducer in context.MethodInfo.GetCustomAttributes<ProducesErrorCodeAttribute>())
		{
			var response = GetResponse(operation.Responses, errorProducer.StatusCode);
			var content = GetContent(response.Content);
			var example = BuildExample(errorProducer);
			content.Examples.Add(errorProducer.ErrorCode.ToString(), example);
		}
	}

	private static OpenApiResponse GetResponse(OpenApiResponses responses, int statusCode)
	{
		if (!responses.TryGetValue(statusCode.ToString(), out var response))
		{
			response = new OpenApiResponse();
			responses.Add(statusCode.ToString(), response);
		}
		return response;
	}

	private static OpenApiMediaType GetContent(IDictionary<string, OpenApiMediaType> contentMap)
	{
		if (!contentMap.TryGetValue(Application.Json, out var content))
		{
			content = new OpenApiMediaType();
			contentMap.Add(Application.Json, content);
		}
		return content;
	}

	private OpenApiExample BuildExample(ProducesErrorCodeAttribute errorProducer) =>
		new()
		{
			Summary = errorProducer.ErrorCode.ToString(),
			Description = GetErrorCodeDescription(errorProducer.ErrorCode),
			Value = new OpenApiString(Serialize(new ErrorResponse(errorProducer.ErrorCode))),
		};

	private string GetErrorCodeDescription(ErrorCode errorCode)
	{
		var errorCodeMember = GetErrorCodeMember(errorCode);
		if (errorCodeMember is null)
		{
			return String.Empty;
		}
		var errorCodeMemberName = XmlCommentsNodeNameHelper.GetMemberNameForFieldOrProperty(errorCodeMember);
		var summaryNode = xmlNavigator.SelectSingleNode($"/doc/members/member[@name='{errorCodeMemberName}']/summary");
		if (summaryNode is null)
		{
			return String.Empty;
		}
		return XmlCommentsTextHelper.Humanize(summaryNode.InnerXml);
	}

	private MemberInfo? GetErrorCodeMember(ErrorCode errorCode)
	{
		if (Enum.GetName(errorCode) is not string name)
		{
			return null;
		}
		return typeof(ErrorCode).GetMember(name).SingleOrDefault();
	}

	private string Serialize(ErrorResponse errorResponse) =>
		JsonSerializer.Serialize(errorResponse, jsonOptions.Value.JsonSerializerOptions);
}
