using System;
using System.Net.Sockets;
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

        public (int, string) Receive(int bufferSize, byte[] buffer)
        {
            try
            {
                var stream = Client.GetStream();
                var length = stream.Read(buffer, 0, bufferSize);
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


        public async Task<(int, string)> ReceiveAsync(int bufferSize, byte[] buffer)
        {
            try
            {
                var stream = Client.GetStream();
                var length = await stream.ReadAsync(buffer, 0, bufferSize);
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

        public async Task<(int, string)> ReceiveAsyncWithTimeOut(int bufferSize, byte[] buffer, Int64 timeOutSec)
        {
            try
            {
                var stream = Client.GetStream();
                stream.ReadTimeout = (int)timeOutSec * 1000;
                Task<int> readTask = stream.ReadAsync(buffer, 0, bufferSize);//.ConfigureAwait(false);

                Task delayTask = Task.Delay(stream.ReadTimeout);
                Task task = await Task.WhenAny(readTask, delayTask);

                if (task != readTask)
                {
                    return (-1, "");
                }

                return (await readTask, "");

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

        public Int64 Close()
        {
            Int64 currentCount = 0;

            try
            {
                Client.Client.Shutdown(SocketShutdown.Both);
            }
            catch(Exception ex)
            {
                LastExceptionMessage = ex.Message;
            }
            finally
            {
                currentCount = DummyManager.DummyDisConnected();
            }

            return currentCount;
        }
    }
}
