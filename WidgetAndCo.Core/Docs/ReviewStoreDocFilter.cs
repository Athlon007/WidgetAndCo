using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json;

namespace WidgetAndCo.Core.Docs;

public class ReviewStoreDocFilter(IConfiguration configuration) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var launchSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(System.IO.File.ReadAllText("Properties/launchSettings.json"));
        var localUrl = launchSettings["profiles"]["http"]["applicationUrl"];

        var reviewFunctionsUrl = configuration["ReviewProcessingFunctionUrl"];
        var orderFunctionsUrl = configuration["OrderProcessingFunctionUrl"];

        swaggerDoc.Servers = new List<OpenApiServer>
        {
            // Also keep current server
            new() { Url = localUrl, Description = "Local" },
            new() { Url = reviewFunctionsUrl, Description = "Review Azure Function" },
            new() { Url = orderFunctionsUrl, Description = "Order Azure Function" }
        };

        #region Reviews
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

        #endregion

        #region Orders

        swaggerDoc.Paths.Add("/CreateOrder", new OpenApiPathItem
        {
            Operations =
            {
                [OperationType.Post] = new OpenApiOperation
                {
                    Summary = "Creates a new order. Must be authenticated",
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
                                        ["productsIDs"] = new OpenApiSchema
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema
                                            {
                                                Type = "string"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });

        swaggerDoc.Paths.Add("/GetOrder/{orderId}", new OpenApiPathItem
        {
            Operations =
            {
                [OperationType.Get] = new OpenApiOperation
                {
                    Summary = "Gets an order by ID. Must be authenticated",
                    Parameters = new List<OpenApiParameter>
                    {
                        new()
                        {
                            Name = "orderId",
                            In = ParameterLocation.Path,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" }
                        }
                    },
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
                                        // Array of GUIDs
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Type = "string"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });

        swaggerDoc.Paths.Add("/GetOrders", new OpenApiPathItem
        {
            Operations =
            {
                [OperationType.Get] = new OpenApiOperation
                {
                    Summary = "Gets all orders. Must be authenticated",
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
                                        // Array of GUIDs
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Type = "string"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });

        #endregion
    }
}