using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace StockMarketUI.Helpers
{
    public static class HtmlHelper
    {
        public static IHtmlContent RenderForm(JsonObject model)
        {
            var formContent = new HtmlContentBuilder();

            foreach (var property in model)
            {                    
                var propertyName = property.Key;
                var propertyValue = property.Value;
                var propertyType = property.Value?.GetType();

                formContent.AppendHtml("<div class='mb-3'>");
                formContent.AppendHtml($"<label for='{propertyName}' class='form-label'>{propertyName}</label>");

                if (propertyType == typeof(string))
                {
                    formContent.AppendHtml($"<input type='text' class='form-control' id='{propertyName}' name='{propertyName}' value='{propertyValue}' />");
                }
                else if (propertyType == typeof(int) || propertyType == typeof(double) || propertyType == typeof(decimal))
                {
                    formContent.AppendHtml($"<input type='number' class='form-control' id='{propertyName}' name='{propertyName}' value='{propertyValue}' />");
                }
                else if (propertyType == typeof(DateTime))
                {
                    formContent.AppendHtml($"<input type='date' class='form-control' id='{propertyName}' name='{propertyName}' value='{propertyValue}' />");
                }
                else if (propertyType == typeof(bool))
                {
                    formContent.AppendHtml($"<input type='checkbox' class='form-check-input' id='{propertyName}' name='{propertyName}' value='{propertyValue}' />");
                }
                else
                {
                    formContent.AppendHtml($"<input type='text' class='form-control' id='{propertyName}' name='{propertyName}' value='{propertyValue}' />");
                }

                formContent.AppendHtml("</div>");

            }

            return formContent;
        }
    }
}