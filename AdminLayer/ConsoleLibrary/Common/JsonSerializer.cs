using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Script.Serialization;

namespace Mediachase.Web.Console.Common
{
    /// <summary>
    /// JSON Serialize helper
    /// </summary>
    public class JsonSerializer
    {
        /// <summary>
        /// Deserializes the specified json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            using (MemoryStream ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(json)))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                return (T)ser.ReadObject(ms);
            }
        }


        /// <summary>
        /// Serializes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            string json = new JavaScriptSerializer().Serialize(obj);
            return json;
            /* BELOW IS A PREFERRED WAY
            if (!IsSerializable(obj))
            {
                throw new Exception("Target object must be serializable.");
            }
            string json = string.Empty;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);
                json = Encoding.Default.GetString(ms.ToArray());
            }
            return json;
             * */
        }

        /// <summary>
        /// Determines whether the specified obj is serializable.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 	<c>true</c> if the specified obj is serializable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSerializable(object obj)
        {
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            try
            {
                ms = new MemoryStream();
                bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                return true;
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                return false;
            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                ms.Close();
                ms.Dispose();
            }
        }
    }
}
