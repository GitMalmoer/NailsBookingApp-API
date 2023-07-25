using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NailsBookingApp.IntegrationTests
{
    public static class HttpContentHelper
    {
        public static HttpContent ChangeModelToHttpContent(this object obj)
        {
            var jsonObj = JsonConvert.SerializeObject(obj);

            StringContent stringContent = new StringContent(jsonObj,Encoding.UTF8,"application/json");
            return stringContent;
        }
    }
}
