using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Dummys
{
    class ConnectOnly
    {
        AsyncSocket ClientSocket = new AsyncSocket();

        public async Task<string> ProcessAsync(string ip, int port)
        {
            try
            {
                var result = await ClientSocket.ConnectAsync(ip, port);
                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

#pragma warning disable 1998
        public async Task<string> EndAsync()
        {
            ClientSocket.Close();
            return "";
        }
#pragma warning restore 1998
    }
}
