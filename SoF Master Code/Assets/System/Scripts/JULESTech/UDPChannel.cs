using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace UDPHelper
{
    public class UDPChannel
    {
        UdpClient recvClient = null;
        Socket senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint sendEndPoint = null;
        IPEndPoint listenEndPoint = null;
        Queue<byte[]> messageQueue = new Queue<byte[]>();

        public bool IsConnected
        {
            get
            {
                if (sendEndPoint != null)
                    return true;
                return false;
            }
        }

        public void Connect(IPAddress _address, int _sendPort, int _recvPort)
        {
            sendEndPoint = new IPEndPoint(_address, _sendPort);
            listenEndPoint = new IPEndPoint(IPAddress.Any, _recvPort);

            recvClient = new UdpClient(_recvPort);
            Thread recvThread = new Thread(this.ReceiveMessages);
            recvThread.Start();
        }

        public void Disconnect()
        {
            recvClient.Close();
            sendEndPoint = null;
        }

        private void ReceiveMessages()
        {
            while (IsConnected)
            {
                byte[] receivedBytes = null;
                try
                {
                    receivedBytes = recvClient.Receive(ref listenEndPoint);
                }
                catch
                {
                    return;
                }

                if (receivedBytes == null || receivedBytes.Length == 0)
                    continue;

                lock (messageQueue)
                {
                    messageQueue.Enqueue(receivedBytes);
                }
            }
        }

        public void Send(byte[] _dataToSend)
        {
            if (sendEndPoint == null)
                return;

            try
            {
                senderSocket.SendTo(_dataToSend, sendEndPoint);
            }
            catch (Exception sendException)
            {
                sendEndPoint = null;
                throw sendException;
            }
        }

		public void Send(string _message)
		{
			Send(Encoding.ASCII.GetBytes(_message));
		}

        public int MessageCount
        {
            get { return messageQueue.Count; }
        }

        public byte[] GetMessage()
        {
            if (MessageCount == 0)
                throw new InvalidOperationException("UDP Channel message queue is empty");

            byte[] result = null;
            lock (messageQueue)
            {
                result = messageQueue.Dequeue();
            }
            return result;
        }
    }
}
