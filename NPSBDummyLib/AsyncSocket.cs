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

        public async Task ReceiveAsync()
        {
            try
            {
                using (var stream = Client.GetStream())
                {
                    var bufferSize = 1024;
                    var buffer = new byte[1024];
                    var length = await stream.ReadAsync(buffer, 0, bufferSize);//.ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                //Interlocked.Increment(ref CloseCount);
                Client.Close();
            }
        }

        public async Task WriteAsync(int bufferSize, byte[] buffer)
        {
            try
            {
                using (var stream = Client.GetStream())
                {
                    await stream.WriteAsync(buffer, 0, bufferSize);//.ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Client.Close();
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
