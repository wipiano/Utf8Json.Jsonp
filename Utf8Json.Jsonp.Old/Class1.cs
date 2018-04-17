using System;
using System.IO;
using System.Text;

namespace Utf8Json
{
    /// <summary>
    /// Additional High-Level API for JSONP
    /// </summary>
    public static class Jsonp
    {
        private static IJsonFormatterResolver DefaultResolver => JsonSerializer.DefaultResolver;
        
        /// <summary>
        /// Serialize to JSONP binary with default resolver.
        /// </summary>
        public static byte[] Serialize<T>(string callback, T obj)
            => Serialize(callback, obj, DefaultResolver);

        /// <summary>
        /// Serialize to binary with specified resolver.
        /// </summary>
        public static byte[] Serialize<T>(string callback, T value, IJsonFormatterResolver resolver)
        {
            var writer = new JsonWriter(MemoryPool.GetBuffer());

            Serialize<T>(ref writer, callback, value, resolver);
            
            return writer.ToUtf8ByteArray();
        }

        public static void Serialize<T>(ref JsonWriter writer, string callback, T value)
            => Serialize(ref writer, callback, value, DefaultResolver);

        public static void Serialize<T>(ref JsonWriter writer, string callback, T value, IJsonFormatterResolver resolver)
        {
            writer.WriteRaw(Encoding.UTF8.GetBytes(callback));      // write callback name
            writer.WriteRaw((byte) '(');
            
            Utf8Json.JsonSerializer.Serialize(ref writer, value, resolver);

            writer.WriteRaw((byte) ')');
        }

        /// <summary>
        /// serialize to stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="callback"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public static void Serialize<T>(Stream stream, string callback, T value)
            => Serialize(stream, callback, value, DefaultResolver);
        
        /// <summary>
        /// serialize to stream with specified resolver.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="callback"></param>
        /// <param name="value"></param>
        /// <param name="resolver"></param>
        /// <typeparam name="T"></typeparam>
        public static void Serialize<T>(Stream stream, string callback, T value, IJsonFormatterResolver resolver)
        {
            var buffer = SerializeUnsafe(callback, value, resolver);
            stream.Write(buffer.Array, buffer.Offset, buffer.Count);
        }

        /// <summary>
        /// Serialize to stream (write async).
        /// </summary>
        public static System.Threading.Tasks.Task SerializeAsync<T>(Stream stream, string callback, T value)
            => SerializeAsync(stream, callback, value, DefaultResolver);
        
        /// <summary>
        /// Serialize to stream (write async) with specified resolver.
        /// </summary>
        public static async System.Threading.Tasks.Task SerializeAsync<T>(Stream stream, string callback, T value,
            IJsonFormatterResolver resolver)
        {
            var buf = BufferPool.Rent();
            try
            {
                var writer = new JsonWriter(buf);
                Serialize(ref writer, callback, value, resolver);
                var buffer = writer.GetBuffer();
                await stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count).ConfigureAwait(false);
            }
            finally
            {
                BufferPool.Return(buf);
            }
        }
        
        public static ArraySegment<byte> SerializeUnsafe<T>(string callback, T obj)
            => SerializeUnsafe(callback, obj, DefaultResolver);
        
        public static ArraySegment<byte> SerializeUnsafe<T>(string callback, T obj, IJsonFormatterResolver resolver)
        {
            var writer = new JsonWriter(MemoryPool.GetBuffer());
            Serialize(ref writer, callback, obj, resolver);
            return writer.GetBuffer();
        }
        
        // ref: Utf8Json.JsonSerializer
        private static class MemoryPool
        {
            [ThreadStatic] 
            private static byte[] s_buffer = null;

            public static byte[] GetBuffer()
            {
                return s_buffer ?? (s_buffer = new byte[65536]);
            }
        }

        // ref: Utf8Json.Internal.ArrayPool
        private static class BufferPool
        {
            private static readonly object s_gate = new object();
            private static int s_index = 0;
            private static byte[][] s_buffers = new byte[4][];

            public static byte[] Rent()
            {
                lock (s_gate)
                {
                    if (s_index >= s_buffers.Length)
                    {
                        Array.Resize(ref s_buffers, s_buffers.Length * 2);
                    }

                    var buffer = s_buffers[s_index] ?? new byte[65536];
                    s_buffers[s_index] = null;
                    s_index++;

                    return buffer;
                }
            }

            public static void Return(byte[] array)
            {
                lock (s_gate)
                {
                    s_buffers[--s_index] = array;
                }
            }
        }
    }
}