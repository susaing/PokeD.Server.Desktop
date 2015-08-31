﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using PokeD.Core.Wrappers;

namespace PokeD.Server.Windows.WrapperInstances
{
    public class NetworkTCPClientWrapperInstance : INetworkTCPClient
    {
        public bool Connected { get { return !IsDisposed && Client != null && Client.Connected; } }
        public int DataAvailable { get { return !IsDisposed && Client != null ? Client.Available : 0; } }


        private TcpClient Client { get; set; }
        private NetworkStream WriteStream { get; set; }
        private BufferedStream ReadStream { get; set; }

        private StreamWriter Writer { get; set; }
        private StreamReader Reader { get; set; }

        private bool IsDisposed { get; set; }


        public NetworkTCPClientWrapperInstance() { }

        internal NetworkTCPClientWrapperInstance(TcpClient tcpClient)
        {
            Client = tcpClient;
            WriteStream = new NetworkStream(Client.Client);
            Writer = new StreamWriter(WriteStream) {AutoFlush = true};
            ReadStream = new BufferedStream(WriteStream);
            Reader = new StreamReader(ReadStream);
        }

        public void Connect(string ip, ushort port)
        {
            Client = new TcpClient(ip, port);
            WriteStream = new NetworkStream(Client.Client);
        }
        public void Disconnect()
        {
            if (Connected)
                Client.Client.Disconnect(false);
        }

        public void Send(byte[] bytes, int offset, int count)
        {
            if(!IsDisposed)
                WriteStream.Write(bytes, offset, count);
        }
        public int Receive(byte[] buffer, int offset, int count)
        {
            if (!IsDisposed)
                return ReadStream.Read(buffer, offset, count);
            else
                return -1;
        }

        public void WriteLine(string data)
        {
            try { Writer.WriteLine(data); }
            catch (Exception) { Disconnect(); }
        }

        public string ReadLine()
        {
            try { return Reader.ReadLine(); }
            catch (Exception) { Disconnect(); return ""; }
        }

        public Task<string> ReadLineAsync()
        {
            return Reader.ReadLineAsync();
        }


        public async Task ConnectAsync(string ip, ushort port)
        {
            Client = new TcpClient();
            await Client.ConnectAsync(ip, port);

            WriteStream = new NetworkStream(Client.Client);
        }
        public bool DisconnectAsync()
        {
            if (Connected)
                return Client.Client.DisconnectAsync(new SocketAsyncEventArgs {DisconnectReuseSocket = false});
            else
                return true;
        }

        public Task SendAsync(byte[] bytes, int offset, int count)
        {
            if (!IsDisposed)
                return WriteStream.WriteAsync(bytes, offset, count);
            else
                return null;
        }
        public Task<int> ReceiveAsync(byte[] bytes, int offset, int count)
        {
            if (!IsDisposed)
                return ReadStream.ReadAsync(bytes, offset, count);
            else
                return null;
        }


        public INetworkTCPClient NewInstance()
        {
            return new NetworkTCPClientWrapperInstance();
        }


        public void Dispose()
        {
            IsDisposed = true;

            Thread.Sleep(500);

            if (Client != null)
                Client.Close();

            if (WriteStream != null)
                WriteStream.Dispose();
        }
    }
}
