using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace AllinaHealth.Framework.Utilities
{
    //*****************************************
    //Courtesy of Wellclicks.Utilities project
    //*****************************************

    /// <summary>
    /// Post or Get Data using Web Request/Response
    /// </summary>
    public class PostGetData : IDisposable
    {

        #region Constructor

        ~PostGetData()
        {
            Dispose();
        }

        private bool _disposedValue;

        public virtual void Dispose()
        {
            if (_disposedValue)
            {
                return;
            }

            _disposedValue = true;
            Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion


        /// <summary>
        /// determines what type of post to perform.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public enum ePostTypeEnum
        {
            /// <summary>
            /// The get
            /// </summary>
            Get,

            /// <summary>
            /// The post
            /// </summary>
            Post
        }

        /// <summary>
        /// Post Form Type
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public enum ePostFormTypeEnum
        {
            /// <summary>
            /// The encoded
            /// </summary>
            Encoded,

            /// <summary>
            /// The form
            /// </summary>
            Form
        }

        public HttpStatusCode? LastHttpStatusCode { private set; get; }

        private const bool BKeepAlive = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostGetData"/> class.
        /// </summary>
        public PostGetData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostGetData"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        public PostGetData(string url)
            : this()
        {
            Url = url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostGetData"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="values">The values.</param>
        // ReSharper disable once UnusedMember.Global
        public PostGetData(string url, NameValueCollection values)
            : this(url)
        {
            PostItems = values;
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// RequestTimeOut
        /// </summary>
        public int? RequestTimeOut { get; set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>
        /// The user agent.
        /// </value>
        public string UserAgent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType { get; set; } = string.Empty;

        public string Accept { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the post items.
        /// </summary>
        /// <value>
        /// The post items.
        /// </value>
        public NameValueCollection PostItems { get; set; } = new NameValueCollection();

        /// <summary>
        /// Gets or sets the Header items.
        /// </summary>
        /// <value>
        /// The header items.
        /// </value>
        public NameValueCollection WebHeaders { get; set; } = new NameValueCollection();

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public ePostTypeEnum Type { get; set; } = ePostTypeEnum.Get;

        /// <summary>
        /// Gets or sets the type of the form.
        /// </summary>
        /// <value>
        /// The type of the form.
        /// </value>
        public ePostFormTypeEnum FormType { get; set; } = ePostFormTypeEnum.Encoded;

        /// <summary>
        /// Posts this instance.
        /// </summary>
        /// <returns></returns>
        public string Post()
        {
            var sbParameters = new StringBuilder();
            for (var i = 0; i < PostItems.Count; i++)
            {
                EncodeAndAddItem(ref sbParameters, PostItems.GetKey(i), PostItems[i]);
            }

            var result = PostData(Url, sbParameters.ToString());
            return result;
        }

        /// <summary>
        /// Posts the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public string Post(string url)
        {
            Url = url;
            return Post();
        }

        /// <summary>
        /// Posts the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="postData">The post data.</param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public string Post(string url, string postData)
        {
            Url = url;
            return PostData(Url, postData);
        }

        /// <summary>
        /// Posts the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public string Post(string url, NameValueCollection values)
        {
            PostItems = values;
            return Post(url);
        }

        private string PostData(string url, string postData)
        {
            HttpWebRequest request = null;
            LastHttpStatusCode = null;

            if (Type == ePostTypeEnum.Post)
            {
                switch (FormType)
                {
                    case ePostFormTypeEnum.Encoded:
                    {
                        var uri = new Uri(url);
                        request = (HttpWebRequest)WebRequest.Create(uri);
                        request.Method = "POST";
                        request.ContentType = ContentType; // "application/x-www-form-urlencoded";
                        request.ContentLength = postData.Length;
                        request.Accept = Accept;
                        request.KeepAlive = BKeepAlive;

                        if (!string.IsNullOrEmpty(UserAgent))
                            request.UserAgent = UserAgent;

                        if (WebHeaders != null)
                            if (WebHeaders.HasKeys())
                                for (var i = 0; i < WebHeaders.Count; i++)
                                {
                                    request.Headers.Add(WebHeaders.GetKey(i), WebHeaders[i]);
                                }

                        using (var writeStream = request.GetRequestStream())
                        {
                            var encoding = new UTF8Encoding();
                            var bytes = encoding.GetBytes(postData);
                            writeStream.Write(bytes, 0, bytes.Length);
                            Thread.Sleep(500);
                            writeStream.Close();
                        }

                        break;
                    }
                    case ePostFormTypeEnum.Form:
                    {
                        var uri = new Uri(url);
                        request = (HttpWebRequest)WebRequest.Create(uri);
                        request.Method = "POST";
                        request.ContentType = ContentType; // "multipart/form-data";
                        request.ContentLength = postData.Length;
                        request.Accept = Accept;
                        request.KeepAlive = BKeepAlive;

                        if (!string.IsNullOrEmpty(UserAgent))
                            request.UserAgent = UserAgent;

                        using (var writeStream = request.GetRequestStream())
                        {
                            var encoding = new UTF8Encoding();
                            var bytes = encoding.GetBytes(postData);
                            writeStream.Write(bytes, 0, bytes.Length);
                            Thread.Sleep(500);
                            writeStream.Close();
                        }

                        break;
                    }
                }
            }
            else
            {
                Uri uri;
                if (postData != null && !string.IsNullOrEmpty(postData))
                {
                    uri = new Uri(url + "?" + postData);
                }
                else
                {
                    uri = new Uri(url);
                }

                request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = ContentType; // "multipart/form-data";
                if (WebHeaders != null)
                    if (WebHeaders.HasKeys())
                        for (var i = 0; i < WebHeaders.Count; i++)
                        {
                            request.Headers.Add(WebHeaders.GetKey(i), WebHeaders[i]);
                        }

                request.Method = "GET";
            }

            if (RequestTimeOut.HasValue)
            {
                if (request != null) request.Timeout = RequestTimeOut.Value;
            }

            var result = string.Empty;
            if (request == null) return result;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                LastHttpStatusCode = response.StatusCode;

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null) return result;
                    using (var readStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                    }
                }
            }

            return result;
        }

        private static void EncodeAndAddItem(ref StringBuilder baseRequest, string key, string dataItem)
        {
            if (baseRequest == null)
            {
                baseRequest = new StringBuilder();
            }

            if (baseRequest.Length != 0)
            {
                baseRequest.Append("&");
            }

            baseRequest.Append(key);
            baseRequest.Append("=");
            baseRequest.Append(HttpUtility.UrlEncode(dataItem));
        }
    }
}