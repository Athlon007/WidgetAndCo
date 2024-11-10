using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WidgetAndCo.Core.Docs;

public class ReviewStoreDocFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Paths.Add("http://localhost:7169/api/StoreReview", new OpenApiPathItem
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
    }
}