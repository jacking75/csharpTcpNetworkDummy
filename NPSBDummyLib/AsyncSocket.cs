using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public class AsyncSocket
    {
        TcpClient Client = new TcpClient();


        public async Task<string> ConnectAsync(string ip, int port)
        {
            try
            {
                await Client.ConnectAsync(ip, port);
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<(int, string)> ReceiveAsync(int bufferSize, byte[] buffer)
        {
            try
            {
                using (var stream = Client.GetStream())
                {
                    var length = await stream.ReadAsync(buffer, 0, bufferSize);//.ConfigureAwait(false);
                    return (length, "");
                }
            }
            catch (Exception ex)
            {
                Client.Close();
                return (-1, ex.Message);
            }            
        }

        public async Task<string> SendAsync(int bufferSize, byte[] buffer)
        {
            try
            {
                using (var stream = Client.GetStream())
                {
                    await stream.WriteAsync(buffer, 0, bufferSize);//.ConfigureAwait(false);
                    return "";
                }
            }
            catch (Exception ex)
            {
                Client.Close();
                return ex.Message;
            }            
        }

        public void Close()
        {
            if (Client.Connected)
            {
                Client.Close();
            }
        }
    }
}
