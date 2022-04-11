using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EasyFtp.Test.UnitTests
{
    public class FtpClientUnitTest
    {
        [Theory]
        [InlineData("ftp://192.168.10.20","testuser","testpassword")]
        public void InitFtpClient_Success(string baseUri,string userName,string password)
        {
            var client = new FtpClient(baseUri, userName, password);
            Assert.NotNull(client);
        }
    }
}
