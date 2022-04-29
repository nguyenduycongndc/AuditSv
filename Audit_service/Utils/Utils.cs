using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Audit_service.Models.MigrationsModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NPOI.OpenXmlFormats.Dml.Chart;
using OfficeOpenXml.ConditionalFormatting;

namespace Audit_service.Utils
{
    public static class Utils
    {
        public static int DepthLengthFromRootLevel(Dictionary<int, int?> parentDic, int parent, int depth =1)
        {
            if (parentDic.ContainsKey(parent))
            {
                if (parentDic[parent] != null)
                {
                   depth =  DepthLengthFromRootLevel(parentDic, (int)parentDic[parent], ++ depth);
                }
            }
            return depth;
        }
    }

    public static class JsonSerializerUtil
    {
        /// <summary>
        /// Deserializes string to JSON
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T Deserialize<T>(string str = "") where T : class, new()
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <returns>A JSON string representation of the object.</returns>
        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
