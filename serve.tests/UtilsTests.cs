using System;
using Xunit;
using serve;

namespace serve.tests
{
    public class UtilsTests
    {
        [Fact]
        public void HttpHeadersVerbsTests()
        {
            var contentType = HttpHeaders.ContentType;
            Assert.Equal("Content-Type", Utils.EnumToString(contentType));
            Assert.Equal("Accept", Utils.EnumToString(HttpHeaders.Accept));
            Assert.Equal("POST", Utils.EnumToString(HttpVerb.Post));
        }
        
        [Fact]
        public void GetResourcePath()
        {
            string getHeader = "GET /images/path23.jpg HTTP/1.1";
            string path = Utils.GetResourcePath(getHeader);
            Assert.Equal("/images/path23.jpg", path);
        }
    }
}