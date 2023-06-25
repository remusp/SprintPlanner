using SprintPlanner.Core.Extensions;
using System;
using System.IO;
using System.Net;
using System.Security;
using System.Text;

namespace SprintPlanner.Core
{
    public class SimpleHttpRequester : IHttpRequester
    {
        public virtual string HttpGetByWebRequest(string uri, string username, SecureString password)
        {
            //For Basic Authentication
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes($"{username}:{password.ToPlain()}"));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.Accept = "application/json; charset=utf-8";

            request.Headers["Authorization"] = "Basic " + authInfo;

            var response = (HttpWebResponse)request.GetResponse();

            string strResponse = "";
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                strResponse = sr.ReadToEnd();
            }

            return strResponse;
        }

        public virtual byte[] HttpGetBinaryByWebRequest(string uri, string username, SecureString password)
        {
            //For Basic Authentication
            string authInfo = $"{username}:{password.ToPlain()}";
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.Accept = "application/json; charset=utf-8";

            request.Headers["Authorization"] = "Basic " + authInfo;

            var response = (HttpWebResponse)request.GetResponse();

            byte[] byteResponse = null;
            using (Stream rs = response.GetResponseStream())
            {
                byteResponse = ReadFully(rs);
            }

            return byteResponse;
        }

        /// <summary>
        /// Courtesy of Jon Skeet.
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="initialLength">The initial buffer length</param>
        public static byte[] ReadFully(Stream stream, int initialLength = 32768)
        {
            // If we've been passed an unhelpful initial length, just
            // use 32K.
            if (initialLength < 1)
            {
                initialLength = 32768;
            }

            byte[] buffer = new byte[initialLength];
            int read = 0;

            int chunk;
            while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
            {
                read += chunk;

                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    // End of stream? If so, we're done
                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    // Nope. Resize the buffer, put in the byte we've just
                    // read, and continue
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[read];
            Array.Copy(buffer, ret, read);
            return ret;
        }

        public string HttpPut(string uri, string data, string username, SecureString password)
        {
            byte[] response;
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            using (var client = new WebClient())
            {
                string authInfo = $"{username}:{password.ToPlain()}";
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                client.Headers["Authorization"] = "Basic " + authInfo;
                client.Headers["Content-Type"] = "application/json";
                response = client.UploadData(uri, "PUT", dataBytes);
            }

            return Encoding.ASCII.GetString(response);
        }
    }
}
