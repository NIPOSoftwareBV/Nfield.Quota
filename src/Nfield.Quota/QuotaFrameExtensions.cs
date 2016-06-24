using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Nfield.Quota
{
    /// <summary>
    /// Contains all the extension methods for <see cref="QuotaFrame"/> class
    /// </summary>
    public static class QuotaFrameExtensions
    {
        /// <summary>
        /// Helper method that compares recursively two quota frames. 
        /// </summary>
        public static bool IsEqual(this QuotaFrame frame, QuotaFrame other)
        {
            var thisJObject = JObject.Parse(JsonConvert.SerializeObject(frame));
            var otherJObject = JObject.Parse(JsonConvert.SerializeObject(other));

            return JToken.DeepEquals(thisJObject, otherJObject);
        }
    }
}
