using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json;

namespace WidgetAndCo.Core.Docs;

public class ReviewStoreDocFilter(IConfiguration configuration) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        //var localUrl = "https://localhost:5173";
        // Get from launchSettings.json
        // Load it
        var launchSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(System.IO.File.ReadAllText("Properties/launchSettings.json"));
        var localUrl = launchSettings["profiles"]["http"]["applicationUrl"];

        var functionsUrl = configuration["ReviewProcessingFunctionUrl"];

        swaggerDoc.Servers = new List<OpenApiServer>
        {
            // Also keep current server
            new OpenApiServer { Url = localUrl, Description = "Local" },
            new OpenApiServer { Url = functionsUrl, Description = "Azure Functions" }
        };

        swaggerDoc.Paths.Add("/StoreReview", new OpenApiPathItem
        {
            Operations =
            {
                [OperationType.Post] = new OpenApiOperation
                {
                    Summary = "Stores user Review",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse { Description = "Success" }
                    },
                    RequestBody = new OpenApiRequestBody
                    {
                        Content =
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Properties =
                                    {
                                        ["product_id"] = new OpenApiSchema { Type = "string" },
                                        ["review_id"] = new OpenApiSchema { Type = "string" },
                                        ["user_id"] = new OpenApiSchema { Type = "string" },
                                        ["title"] = new OpenApiSchema { Type = "string" },
                                        ["description"] = new OpenApiSchema { Type = "string" },
                                        ["rating"] = new OpenApiSchema { Type = "integer" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });

        swaggerDoc.Paths.Add("/GetReviews/{productId}", new OpenApiPathItem
        {
            Operations =
            {
                [OperationType.Get] = new OpenApiOperation
                {
                    Parameters = new List<OpenApiParameter>
                    {
                        new OpenApiParameter
                        {
                            Name = "productId",
                            In = ParameterLocation.Path,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" }
                        }
                    },
                    Summary = "Retrieves all reviews for a product",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "Success",
                            Content =
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        // array of review responses
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Type = "object",
                                            Properties =
                                            {
                                                ["product_id"] = new OpenApiSchema { Type = "string" },
                                                ["review_id"] = new OpenApiSchema { Type = "string" },
                                                ["user_id"] = new OpenApiSchema { Type = "string" },
                                                ["title"] = new OpenApiSchema { Type = "string" },
                                                ["description"] = new OpenApiSchema { Type = "string" },
                                                ["rating"] = new OpenApiSchema { Type = "integer" }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                }
            }
        });


        swaggerDoc.Paths.Add("/GetReview/{productId}/{reviewId}", new OpenApiPathItem
        {
            Operations =
            {
                [OperationType.Get] = new OpenApiOperation
                {
                    Parameters = new List<OpenApiParameter>
                    {
                        new OpenApiParameter
                        {
                            Name = "productId",
                            In = ParameterLocation.Path,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" }
                        },
                        new OpenApiParameter
                        {
                            Name = "reviewId",
                            In = ParameterLocation.Path,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" }
                        }
                    },
                    Summary = "Retrieves a specific review for a product",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "Success",
                            Content =
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "object",
                                        Properties =
                                        {
                                            ["product_id"] = new OpenApiSchema { Type = "string" },
                                            ["review_id"] = new OpenApiSchema { Type = "string" },
                                            ["user_id"] = new OpenApiSchema { Type = "string" },
                                            ["title"] = new OpenApiSchema { Type = "string" },
                                            ["description"] = new OpenApiSchema { Type = "string" },
                                            ["rating"] = new OpenApiSchema { Type = "integer" }
                                        }
                                    }
                                }
                            }
                        }
                    },
                }
            }
        });
    }
}