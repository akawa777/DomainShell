using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DomainShell.Tests.Web.Controllers.Extension
{
    public static class ControllerExtensions
    {
        public static ActionResult JsonCamel(this Controller controller, object data, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            if (controller.Request.RequestType == WebRequestMethods.Http.Get
                && behavior == JsonRequestBehavior.DenyGet)
                throw new Exception("Get is not allowed");
            var serializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var jsonResult = new ContentResult()
            {
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(data, serializationSettings)
            };

            return jsonResult;
        }
    }
}