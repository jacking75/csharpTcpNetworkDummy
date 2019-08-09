using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public class AsyncSocket
    {
        TcpClient Client = null;
        string LastExceptionMessage;

        public bool IsConnected() { return Client != null && Client.Connected; }

        

        public async Task<(bool,string)> ConnectAsync(string ip, int port)
        {
            try
            {
                Client = null;
                Client = new TcpClient();
                Client.NoDelay = true;

                await Client.ConnectAsync(ip, port);
                DummyManager.DummyConnected();
                return (true, "");
            }
            catch (Exception ex)
            {
                LastExceptionMessage = ex.Message;
                return (false, ex.Message);
            }
        }

        public async Task<(int, string)> ReceiveAsync(int bufferSize, byte[] buffer)
        {
            try
            {
                var stream = Client.GetStream();
                var length = await stream.ReadAsync(buffer, 0, bufferSize);//.ConfigureAwait(false);
                return (length, "");
                //using (var stream = Client.GetStream())
                //{
                //    var length = await stream.ReadAsync(buffer, 0, bufferSize);//.ConfigureAwait(false);
                //    return (length, "");
                //}
            }
            catch (Exception ex)
            {
                LastExceptionMessage = ex.Message;
                Client.Close();
                return (-1, ex.Message);
            }            
        }

        public async Task<string> SendAsync(int bufferSize, byte[] buffer)
        {
            try
            {
                var stream = Client.GetStream();
                await stream.WriteAsync(buffer, 0, bufferSize);//.ConfigureAwait(false);
                return "";
            }
            catch (Exception ex)
            {
                LastExceptionMessage = ex.Message;
                Client.Close();
                return ex.Message;
            }            
        }

        public void Close()
        {
            try
            {
                Client.Close();                
            }
            catch(Exception ex)
            {
                LastExceptionMessage = ex.Message;
            }
            finally
            {
                DummyManager.DummyDisConnected();
            }            
        }
    }
}
