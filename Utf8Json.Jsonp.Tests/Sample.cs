using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit;

namespace Utf8Json.JsonpTests
{
    public class Sample
    {
        [Fact]
        public void Sample1()
        {
            var p = new Person { Age = 99, Name = "foobar" };
            var callback = "callbackFunc";

            var expected = "callbackFunc({\"Age\":99,\"Name\":\"foobar\"})";
            
            // object -> byte[] (UTF8)
            byte[] bytes = Jsonp.Serialize(callback, p);
            Encoding.UTF8.GetString(bytes).Is(expected);
            
            // write to stream
            using (var stream = new MemoryStream())
            {
                Jsonp.Serialize(stream, callback, p);
                Encoding.UTF8.GetString(stream.ToArray()).Is(expected);
            }
        }
    }
}