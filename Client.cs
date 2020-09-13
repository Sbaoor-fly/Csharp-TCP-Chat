using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Security;

class Program2
    {
        //创建1个客户端套接字和1个负责监听服务端请求的线程  
        static Thread ThreadClient = null;
        static Socket SocketClient = null;
     
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("请输入要连接的聊天室IP");
                string  host = Console.ReadLine();
                int port = 6000; 
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port);
     
                //定义一个套接字监听  
                SocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
     
                try
                {
                    //客户端套接字连接到网络节点上，用的是Connect  
                    SocketClient.Connect(ipe);
                }
                catch (Exception)
                {
                    Console.WriteLine("连接失败！\r\n");
                    Console.ReadLine();
                    return;
                }
     
                ThreadClient = new Thread(Recv);
                ThreadClient.IsBackground = true;
                ThreadClient.Start();
     
                Thread.Sleep(1000);
                Console.WriteLine("请输入内容<按Enter键发送>：\r\n");
                while(true)
                {
                    string sendStr = Console.ReadLine();
                    ClientSendMsg(sendStr);
                }
     
                //int i = 1;
                //while (true)
                //{
                //    Console.Write("请输入内容：");
                //    string sendStr = Console.ReadLine();
     
                //    Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //    clientSocket.Connect(ipe);
                //    //send message
                //    //byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
                //    byte[] sendBytes = Encoding.GetEncoding("utf-8").GetBytes(sendStr);
     
                //    //Thread.Sleep(4000);
     
                //    clientSocket.Send(sendBytes);
     
                //    //receive message
                //    string recStr = ""; 
                //    byte[] recBytes = new byte[4096];
                //    int bytes = clientSocket.Receive(recBytes, recBytes.Length, 0);
                //    //recStr += Encoding.ASCII.GetString(recBytes, 0, bytes);
                //    recStr += Encoding.GetEncoding("utf-8").GetString(recBytes, 0, bytes);
                //    Console.WriteLine(recStr);
     
                //    clientSocket.Close();
                //    if (i >= 100)
                //    {
                //        break;
                //    }
                //    i++;
                //}
                    
                //Console.ReadLine();
                //return;
     
                //string result = String.Empty;
     
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
     
        //接收服务端发来信息的方法    
        public static void Recv()
        {
                int x = 0;
            //持续监听服务端发来的消息 
            while (true)
            {
                try
                {
                    //定义一个1M的内存缓冲区，用于临时性存储接收到的消息  
                    byte[] arrRecvmsg = new byte[1024 * 1024];
     
                    //将客户端套接字接收到的数据存入内存缓冲区，并获取长度  
                    int length = SocketClient.Receive(arrRecvmsg);
     
                    //将套接字获取到的字符数组转换为人可以看懂的字符串  
                    string strRevMsg = Encoding.UTF8.GetString(arrRecvmsg, 0, length);
                    if (x == 1)
                    {
                        Console.WriteLine("\r\n服务器：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\r\n" + strRevMsg+"\r\n");
                            
                    }
                    else
                    {
                        Console.WriteLine(strRevMsg + "\r\n");
                        x = 1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("远程服务器已经中断连接！" + ex.Message + "\r\n");
                    break;
                }
            }
        }
     
        //发送字符信息到服务端的方法  
        public static void ClientSendMsg(string sendMsg)
        {
            //将输入的内容字符串转换为机器可以识别的字节数组     
            byte[] arrClientSendMsg = Encoding.UTF8.GetBytes(sendMsg);
            //调用客户端套接字发送字节数组     
            SocketClient.Send(arrClientSendMsg);
        }    
    }