﻿using JetBrains.Annotations;
using System.Net;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    public static class NetExtension
    {
        #region WebRequest

        /// <summary>
        ///     A WebRequest extension method that gets the WebRequest response or the WebException response.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The WebRequest response or WebException response.</returns>
        public static WebResponse GetResponseSafe([NotNull] this WebRequest @this)
        {
            try
            {
                return @this.GetResponse();
            }
            catch (WebException e)
            {
                return e.Response;
            }
        }

        /// <summary>
        ///     A WebRequest extension method that gets the WebRequest response or the WebException response.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The WebRequest response or WebException response.</returns>
        public static async Task<WebResponse> GetResponseSafeAsync([NotNull] this WebRequest @this)
        {
            try
            {
                return await @this.GetResponseAsync().ConfigureAwait(false);
            }
            catch (WebException e)
            {
                return e.Response;
            }
        }

        public static byte[] GetResponseBytes([NotNull] this WebRequest @this)
        {
            using var response = @this.GetResponse();
            return response.ReadAllBytes();
        }

        public static async Task<byte[]> GetResponseBytesAsync([NotNull] this WebRequest @this)
        {
            using var response = await @this.GetResponseAsync().ConfigureAwait(false);
            return await response.ReadAllBytesAsync().ConfigureAwait(false);
        }

        public static string GetResponseString([NotNull] this WebRequest @this)
        {
            using var response = @this.GetResponse();
            return response.ReadToEnd();
        }

        public static async Task<string> GetResponseStringAsync([NotNull] this WebRequest @this)
        {
            using var response = await @this.GetResponseAsync().ConfigureAwait(false);
            return await response.ReadToEndAsync().ConfigureAwait(false);
        }

        public static byte[] GetResponseBytesSafe([NotNull] this WebRequest @this)
        {
            using var response = @this.GetResponseSafe();
            return response.ReadAllBytes();
        }

        public static async Task<byte[]> GetResponseBytesSafeAsync([NotNull] this WebRequest @this)
        {
            using var response = await @this.GetResponseSafeAsync().ConfigureAwait(false);
            return await response.ReadAllBytesAsync().ConfigureAwait(false);
        }

        public static string GetResponseStringSafe([NotNull] this WebRequest @this)
        {
            using var response = @this.GetResponseSafe();
            return response.ReadToEnd();
        }

        public static async Task<string> GetResponseStringSafeAsync([NotNull] this WebRequest @this)
        {
            using var response = await @this.GetResponseSafeAsync().ConfigureAwait(false);
            return await response.ReadToEndAsync().ConfigureAwait(false);
        }

        #endregion WebRequest

        #region WebResponse

        /// <summary>
        /// A WebResponse extension method that reads the response stream to byte array.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The response stream as byte array</returns>
        public static byte[] ReadAllBytes([NotNull] this WebResponse @this)
        {
            using var stream = @this.GetResponseStream()!;
            var byteArray = new byte[stream.Length];
            stream.Write(byteArray, 0, byteArray.Length);
            return byteArray;
        }

        /// <summary>
        /// A WebResponse extension method that reads the response stream to byte array.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The response stream as byte array</returns>
        public static async Task<byte[]> ReadAllBytesAsync([NotNull] this WebResponse @this)
        {
            using var stream = @this.GetResponseStream()!;
            var byteArray = new byte[stream.Length];
            await stream.WriteAsync(byteArray, 0, byteArray.Length).ConfigureAwait(false);
            return byteArray;
        }

        /// <summary>
        ///     A WebResponse extension method that reads the response stream to the end.
        /// </summary>
        /// <param name="response">The response to act on.</param>
        /// <returns>The response stream as a string, from the current position to the end.</returns>
        public static string ReadToEnd([NotNull] this WebResponse response)
        {
            using var stream = response.GetResponseStream()!;
            return stream.ReadToEnd(Encoding.UTF8);
        }

        /// <summary>
        ///     A WebResponse extension method that reads the response stream to the end.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The response stream as a string, from the current position to the end.</returns>
        public static async Task<string> ReadToEndAsync([NotNull] this WebResponse @this)
        {
            using var stream = @this.GetResponseStream()!;
            return await stream.ReadToEndAsync().ConfigureAwait(false);
        }

        #endregion WebResponse
    }
}
