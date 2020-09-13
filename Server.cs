
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
class Program
    {
        //创建一个和客户端通信的套接字
        static Socket SocketWatch = null;
        //定义一个集合，存储客户端信息
        static Dictionary<string, Socket> ClientConnectionItems = new Dictionary<string, Socket> { };
     
        static void Main(string[] args)
        {
            //端口号（用来监听的）
            int port = 6000;
     
            //string host = "127.0.0.1";
            //IPAddress ip = IPAddress.Parse(host);
            IPAddress ip = IPAddress.Any;
     
            //将IP地址和端口号绑定到网络节点point上  
            IPEndPoint ipe = new IPEndPoint(ip, port);
     
            //定义一个套接字用于监听客户端发来的消息，包含三个参数（IP4寻址协议，流式连接，Tcp协议）  
            SocketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //监听绑定的网络节点  
            SocketWatch.Bind(ipe);
            //将套接字的监听队列长度限制为20  
            SocketWatch.Listen(20);
     
     
            //负责监听客户端的线程:创建一个监听线程  
            Thread threadwatch = new Thread(WatchConnecting);
            //将窗体线程设置为与后台同步，随着主线程结束而结束  
            threadwatch.IsBackground = true;
            //启动线程     
            threadwatch.Start();
     
            Console.WriteLine("开启监听......");
            Console.WriteLine("点击输入任意数据回车退出程序......");
            Console.ReadKey();
     
            SocketWatch.Close();
     
            //Socket serverSocket = null;
     
            //int i=1;
            //while (true)
            //{
            //    //receive message
            //    serverSocket = SocketWatch.Accept();
            //    Console.WriteLine("连接已经建立！");
            //    string recStr = "";
            //    byte[] recByte = new byte[4096];
            //    int bytes = serverSocket.Receive(recByte, recByte.Length, 0);
            //    //recStr += Encoding.ASCII.GetString(recByte, 0, bytes);
            //    recStr += Encoding.GetEncoding("utf-8").GetString(recByte, 0, bytes);
     
            //    //send message
            //    Console.WriteLine(recStr);
     
            //    Console.Write("请输入内容：");
            //    string sendStr = Console.ReadLine();
     
            //    //byte[] sendByte = Encoding.ASCII.GetBytes(sendStr);
            //    byte[] sendByte = Encoding.GetEncoding("utf-8").GetBytes(sendStr);
     
            //    //Thread.Sleep(4000);
     
            //    serverSocket.Send(sendByte, sendByte.Length, 0);
            //    serverSocket.Close();
            //    if (i >= 100)
            //    {
            //        break;
            //    }
            //    i++;
            //}
                
            //sSocket.Close();
            //Console.WriteLine("连接关闭！");
     
     
            //Console.ReadLine();
        }
     
        //监听客户端发来的请求  
        static void WatchConnecting()
        {
            Socket connection = null;
     
            //持续不断监听客户端发来的请求     
            while (true)
            {
                try
                {
                    connection = SocketWatch.Accept();
                }
                catch (Exception ex)
                {
                    //提示套接字监听异常     
                    Console.WriteLine(ex.Message);
                    break;
                }
     
                //客户端网络结点号  
                string remoteEndPoint = connection.RemoteEndPoint.ToString();
                //添加客户端信息  
                ClientConnectionItems.Add(remoteEndPoint, connection);
                //显示与客户端连接情况
                Console.WriteLine("\r\n[客户端\"" + remoteEndPoint + "\"建立连接成功！ 客户端数量：" + ClientConnectionItems .Count+ "]");
     
                //获取客户端的IP和端口号  
                IPAddress clientIP = (connection.RemoteEndPoint as IPEndPoint).Address;
                int clientPort = (connection.RemoteEndPoint as IPEndPoint).Port;
     
                //让客户显示"连接成功的"的信息  
                string sendmsg = "[" + "本地IP：" + clientIP + " 本地端口：" + clientPort.ToString() + " 连接服务端成功！]";
                byte[] arrSendMsg = Encoding.UTF8.GetBytes(sendmsg);
                connection.Send(arrSendMsg);
     
                //创建一个通信线程      
                Thread thread = new Thread(recv);
                //设置为后台线程，随着主线程退出而退出 
                thread.IsBackground = true;
                //启动线程     
                thread.Start(connection);
            }
        }
     
        /// <summary>
        /// 接收客户端发来的信息，客户端套接字对象
        /// </summary>
        /// <param name="socketclientpara"></param>    
        static void recv(object socketclientpara)
        {
            Socket socketServer = socketclientpara as Socket;
     
            while (true)
            {
                //创建一个内存缓冲区，其大小为1024*1024字节  即1M     
                byte[] arrServerRecMsg = new byte[1024 * 1024];
                //将接收到的信息存入到内存缓冲区，并返回其字节数组的长度    
                try
                {
                    int length = socketServer.Receive(arrServerRecMsg);
     
                    //将机器接受到的字节数组转换为人可以读懂的字符串     
                    string strSRecMsg = Encoding.UTF8.GetString(arrServerRecMsg, 0, length);
     
                    //将发送的字符串信息附加到文本框txtMsg上     
                    Console.WriteLine("\r\n[客户端：" + socketServer.RemoteEndPoint + " 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")+ "]\r\n" + strSRecMsg);
     
                    //Thread.Sleep(3000);
                    //socketServer.Send(Encoding.UTF8.GetBytes("[" + socketServer.RemoteEndPoint + "]："+strSRecMsg));
                    //发送客户端数据
                    if (ClientConnectionItems.Count > 0)
                    {
                        foreach (var socketTemp in ClientConnectionItems)
                        {
                            socketTemp.Value.Send(Encoding.UTF8.GetBytes("[" + socketServer.RemoteEndPoint + "]：" + strSRecMsg));
                        }
                    }
                }
                catch (Exception)
                {
                    ClientConnectionItems.Remove(socketServer.RemoteEndPoint.ToString());
                    //提示套接字监听异常  
                    Console.WriteLine("\r\n[客户端\"" + socketServer.RemoteEndPoint + "\"已经中断连接！ 客户端数量：" + ClientConnectionItems.Count+"]");
                    //关闭之前accept出来的和客户端进行通信的套接字 
                    socketServer.Close();
                    break;
                }
            }
        }
    }