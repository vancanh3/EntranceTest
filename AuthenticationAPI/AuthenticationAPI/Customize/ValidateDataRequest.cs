using AuthenticationAPI.Common;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;

namespace AuthenticationAPI.Customize
{
    public class ValidateDataRequest : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var objectRequest = context.ActionArguments.FirstOrDefault().Value;
            var email = objectRequest?.GetType()?.GetProperty("Email")?.GetValue(objectRequest, null).ToString();
            var password = objectRequest?.GetType()?.GetProperty("Password")?.GetValue(objectRequest, null).ToString();
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                context.Result = new ObjectResult(new
                {
                    StatusCode = StatusCode.VALIDATION_ERROR_CODE,
                    Message = "Validation Error"
                });
                return;
            } else
            {
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                var validPassWord = password.Length > 0 && password.Length < 20;
                if (!Regex.IsMatch(email, emailPattern) || !validPassWord)
                {
                    context.Result = new ObjectResult(new
                    {
                        StatusCode = StatusCode.VALIDATION_ERROR_CODE,
                        Message = "Validation Error"
                    });
                    return;
                }

            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
