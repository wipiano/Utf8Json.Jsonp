using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Utf8Json.JsonpTests
{
    public class JsonpSerializerTests
    {
        private readonly Person _object = new Person { Age = 99, Name = "foobar" };
        private readonly string _callback = "callbackFunc";
        private readonly string _expected = "callbackFunc({\"Age\":99,\"Name\":\"foobar\"})";
        
        [Fact]
        public void Serialize()
        {
            GetString(Jsonp.Serialize(_callback, _object)).Is(_expected);
        }

        [Fact]
        public void SerializeToStream()
        {
            using (var stream = new MemoryStream())
            {
                Jsonp.Serialize(stream, _callback, _object);
                GetString(stream.ToArray()).Is(_expected);
            }
        }

        [Fact]
        public async Task SerializeAsync()
        {
            using (var stream = new MemoryStream())
            {
                await Jsonp.SerializeAsync(stream, _callback, _object);
                GetString(stream.ToArray()).Is(_expected);
            }
        }
        
        private static string GetString(byte[] bytes)
            => Encoding.UTF8.GetString(bytes);
    }
}