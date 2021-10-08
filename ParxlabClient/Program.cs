using ParkingSystem;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ParxlabClient
{
    class Program
    {
        ///// <summary>
        ///// <remarks>TCP Instantiation</remarks>
        ///// </summary>
        //public static ParkingSystem.ParkingRemoteTCP wapper = new ParkingRemoteTCP();
        ///// <summary>
        ///// <remarks>tcp Listener</remarks>
        ///// </summary>
        //private static TcpListener tcpListener = null;
        /// <summary>
        /// <remarks>IP Address</remarks>
        /// </summary>
        private static IPAddress ipAddr = new IPAddress(new byte[] { 169, 254, 59, 39 });
        /// <summary>
        /// <remarks>TCP portNum</remarks>
        /// </summary>
        private static int port = 6000;
        ///// <summary>
        ///// <remarks>Tcp Client</remarks>
        ///// </summary>
        //private static TcpClient client = new TcpClient();
        ///// <summary>
        ///// <remarks>Tcp Thread</remarks>
        ///// </summary>
        //private Thread m_serverThread;

        static void Main(string[] args)
        {
            ParseArguments(args);

            Client client = new(ipAddr, port);
            client.TCP_StartListen();

            Console.ReadKey();
        }

        private static void ParseArguments(string[] args)
        {
            if (args.Length == 2)
            {
                try
                {
                    ipAddr = IPAddress.Parse(args[0]);
                }
                catch (FormatException fe)
                {
                    Console.WriteLine("[ERR] Provided argument is not an IP address.\nCaused by: {0}", fe.ToString());
                    PrintUsage();
                    Environment.Exit(-1);
                }

                try
                {
                    port = int.Parse(args[1]);
                }
                catch (FormatException fe)
                {
                    Console.WriteLine("[ERR] Provided argument is not a port number.\nCaused by: {0}", fe.ToString());
                    PrintUsage();
                    Environment.Exit(-1);
                }
            }
            else Console.WriteLine("[{0} INF] No arguments provided; Using default parameters.", DateTime.Now);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("\nUsage:\nParxlabClient.exe <IP> <Port>\n\tIP: Local IP address that you want to connect\n" +
                "\t\tFor example: 169.254.69.96\n\tPort: Port number that you want to communicate on\n" +
                "\t\tFor example: 6900");
        }

        ///// <summary>
        ///// Start TCP Listening
        ///// </summary>
        //public void TCP_StartListen()
        //{
        //    try
        //    {
        //        tcpListener = new TcpListener(ipAddr, port);
        //        tcpListener.Start();
        //        m_serverThread = new Thread(new ThreadStart(ReceiveAccept));
        //        m_serverThread.Start();
        //        m_serverThread.IsBackground = true;
        //        Console.WriteLine("[{0} INF] Communication thread successfully started.", DateTime.Now);
        //    }
        //    catch (SocketException se)
        //    {
        //        tcpListener.Stop();
        //        Console.WriteLine("[ERR] An error occured while trying to start TCP listener on IP: {0} at Port: {1}\nCaused by: {2}", ipAddr, port, se.ToString());
        //        Environment.Exit(-1);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Write("[ERR] An error occured while trying to start communication thread.\nCaused by: {0}", e.ToString());
        //        Environment.Exit(-1);
        //    }
        //}

        ///// <summary>
        ///// TCP Client Accept
        ///// </summary>
        //private void ReceiveAccept()
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            client = tcpListener.AcceptTcpClient();
        //            wapper = new ParkingRemoteTCP(client);
        //        }
        //        catch (Exception ex)
        //        {
        //            //eth_Setclose();
        //            // MessageBox.Show(ex.Message, "?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }
        //    }
        //}

        ///// <summary>
        ///// <remarks>Process Received Packet</remarks>
        ///// </summary>
        ///// <param name="pk">Received Packet</param>
        //private static void sp_ProcessReceivedPacket(baseReceivedPacket pk)
        //{
        //    Console.WriteLine(pk.ToString());

        //    try
        //    {
        //        byte revType = Convert.ToByte(pk.type_ver >> 8);
        //        string wpsdid = "";
        //        string WDCid = "";
        //        string RSSI = "";
        //        byte carState = 0;
        //        string voltage = "";
        //        string hardVer = "";
        //        string softVer = "";
        //        string hbPeriod = "";

        //        #region Senser Heart Beat
        //        if (pk is SensorHBeat)
        //        {
        //            SensorHBeat hb = (SensorHBeat)pk;
        //            wpsdid = (hb.WPSD_ID).ToString("X2").PadLeft(8, '0');
        //            WDCid = (hb.WDC_ID).ToString("X2").PadLeft(8, '0');
        //            softVer = "v" + int.Parse(hb.APP_VER.ToString("X2").Substring(0, 1)).ToString() + "." + int.Parse(hb.APP_VER.ToString("X2").Substring(1, 1)).ToString().PadLeft(2, '0');
        //            hardVer = ((int)(hb.HARD_VER) + 10).ToString();
        //            hardVer = "v" + hardVer.Substring(0, 1) + "." + hardVer.Substring(1, 1);
        //            voltage = (Math.Round((decimal)hb.VOLT / 10, 2)).ToString() + "V"; RSSI = ((Int16)hb.RSSI - 30).ToString();
        //            hbPeriod = hb.HB_PERIOD.ToString();
        //            carState = hb.CAR_STATE;

        //            int wpsd = int.Parse(wpsdid, System.Globalization.NumberStyles.HexNumber);
        //            int wdc = int.Parse(WDCid, System.Globalization.NumberStyles.HexNumber);

        //            int isParked = carState;

        //            Console.WriteLine("[STT] Park: {0}, Device: {1}, Is Parked: {2}", wdc, wpsd, isParked);
        //        }
        //        #endregion
        //        #region Sensor Detect
        //        else if (pk is SensorDetect)
        //        {
        //            SensorDetect decbeat = (SensorDetect)pk;
        //            wpsdid = (decbeat.WPSD_ID).ToString("X2").PadLeft(8, '0');
        //            WDCid = (decbeat.WDC_ID).ToString("X2").PadLeft(8, '0');
        //            hardVer = ((int)(decbeat.HARD_VER) + 10).ToString();
        //            hardVer = "v" + hardVer.Substring(0, 1) + "." + hardVer.Substring(1, 1);
        //            carState = decbeat.CAR_STATE;

        //            int wpsd = int.Parse(wpsdid, System.Globalization.NumberStyles.HexNumber);
        //            int wdc = int.Parse(WDCid, System.Globalization.NumberStyles.HexNumber);

        //            int isParked = carState;
        //            Console.WriteLine("[STT] Park: {0}, Device: {1}, Is Parked: {2}", wdc, wpsd, isParked);
        //        }
        //        #endregion
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}

        //    private static void StartTcpListener()
        //    {
        //        tcpListener = new(ipAddr, port);
        //        try
        //        {
        //            tcpListener.Start();
        //        }
        //        catch (SocketException se)
        //        {
        //            Console.WriteLine("[ERR] An error occured while trying to start TCP listener on IP: {0} at Port: {1}\nCaused by: {2}", ipAddr, port, se.ToString());
        //            Environment.Exit(-1);
        //        }
        //    }
        //    private static void StartCommunicationThread()
        //    {
        //        try
        //        {
        //            tcpListener.Start();
        //            m_serverThread = new(new ThreadStart(ReceiveAccept));
        //            m_serverThread.Start();
        //            m_serverThread.IsBackground = true;
        //            Console.WriteLine("[{0} INF] Communication thread successfully started.", DateTime.Now);
        //        }
        //        catch (SocketException se)
        //        {
        //            Console.WriteLine("[ERR] An error occured while trying to start TCP listener on IP: {0} at Port: {1}\nCaused by: {2}", ipAddr, port, se.ToString());
        //            Environment.Exit(-1);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.Write("[ERR] An error occured while trying to start communication thread.\nCaused by: {0}", e.ToString());
        //            Environment.Exit(-1);
        //        }
        //    }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using ParkingSystem;

//namespace ParxlabClient
//{
//    class Program
//    {
//        private static TcpListener tcpListener = null;
//        public static ParkingSystem.ParkingRemoteTCP wapper = new ParkingRemoteTCP();


//        private static void ReceiveAccept()
//        {
//            TcpClient client = new TcpClient();
//            while (true)
//            {
//                try
//                {
//                    client = tcpListener.AcceptTcpClient();
//                    Console.Write("Bağlandı\n");
//                    wapper = new ParkingRemoteTCP(client);
//                }
//                catch (Exception ex)
//                {
//                    //eth_Setclose();
//                    // MessageBox.Show(ex.Message, "?", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                }
//            }
//        }

//        /// <summary>
//        /// <remarks>Process Received Packet</remarks>
//        /// </summary>
//        /// <param name="pk">Received Packet</param>
//        private static void sp_ProcessReceivedPacket(baseReceivedPacket pk)
//        {
//            Console.Write(pk.ToString());
//            Console.Write("\n");
//            //Model1 db = new Model1();
//            try
//            {
//                byte revType = Convert.ToByte(pk.type_ver >> 8);
//                string wpsdid = "";
//                string WDCid = "";
//                string RSSI = "";
//                byte carState = 0;
//                string voltage = "";
//                string hardVer = "";
//                string softVer = "";
//                string deviceName = "";
//                string hbPeriod = "";
//                //anatablo kayit;

//                #region Senser Heart Beat
//                if (pk is SensorHBeat)
//                {
//                    SensorHBeat hb = (SensorHBeat)pk;
//                    wpsdid = (hb.WPSD_ID).ToString("X2").PadLeft(8, '0');
//                    WDCid = (hb.WDC_ID).ToString("X2").PadLeft(8, '0');
//                    softVer = "v" + int.Parse(hb.APP_VER.ToString("X2").Substring(0, 1)).ToString() + "." + int.Parse(hb.APP_VER.ToString("X2").Substring(1, 1)).ToString().PadLeft(2, '0');
//                    hardVer = ((int)(hb.HARD_VER) + 10).ToString();
//                    hardVer = "v" + hardVer.Substring(0, 1) + "." + hardVer.Substring(1, 1);
//                    voltage = (Math.Round((decimal)hb.VOLT / 10, 2)).ToString() + "V"; RSSI = ((Int16)hb.RSSI - 30).ToString();
//                    hbPeriod = hb.HB_PERIOD.ToString();
//                    carState = hb.CAR_STATE;

//                    int wpsd = int.Parse(wpsdid, System.Globalization.NumberStyles.HexNumber);
//                    int wdc = int.Parse(WDCid, System.Globalization.NumberStyles.HexNumber);
//                    //cihaz cihaz = (from veri in db.cihaz where veri.wpsd == wpsd select veri).FirstOrDefault();
//                    //parkyeri park = (from veri in db.parkyeri where veri.wdc == wdc select veri).FirstOrDefault();
//                    int dolu = carState;

//                    Console.Write("park: {0}, cihaz:{1}, dolu mu: {2}\n", wdc, wpsd, dolu);
//                    //cihaz.songuncelleme = hb.rxtime;

//                    //kayit = (from veri in db.anatablo where veri.cihazid == cihaz.cihazid && veri.cikiszamani == null select veri).FirstOrDefault();    /*  Bu cihazın son kaydını bul  */
//                    //if (kayit != null)
//                    //{
//                    //    kayit.pildurumu = hb.VOLT;  /*  Güncelleme yalnızca üstünde araç varsa yapılıyor. Eğer zamana bağlı değişimi incelenecekse çıkış zamanına göre sıralanmalı  */
//                    //}

//                    //if (dolu == cihaz.cihazdurumu)   /*  Cihaz durumunda bir değişiklik yok  */
//                    //{
//                    //    return;
//                    //}

//                    //if (dolu == 0)
//                    //{
//                    //    cihaz.cihazdurumu = 0;


//                    //    if (kayit != null)
//                    //    {
//                    //        kayit.cikiszamani = hb.rxtime;
//                    //    }

//                    //}
//                    //else
//                    //{
//                    //    cihaz.cihazdurumu = 1;
//                    //    kayit = new anatablo();
//                    //    kayit.cihazid = cihaz.cihazid;
//                    //    kayit.parkid = park.parkid;
//                    //    kayit.giriszamani = hb.rxtime;
//                    //    kayit.cikiszamani = null;
//                    //    db.anatablo.Add(kayit);

//                    //}

//                }
//                #endregion
//                #region Senser Detect
//                else if (pk is SensorDetect)
//                {
//                    SensorDetect decbeat = (SensorDetect)pk;
//                    wpsdid = (decbeat.WPSD_ID).ToString("X2").PadLeft(8, '0');
//                    WDCid = (decbeat.WDC_ID).ToString("X2").PadLeft(8, '0');
//                    hardVer = ((int)(decbeat.HARD_VER) + 10).ToString();
//                    hardVer = "v" + hardVer.Substring(0, 1) + "." + hardVer.Substring(1, 1);
//                    carState = decbeat.CAR_STATE;

//                    int wpsd = int.Parse(wpsdid, System.Globalization.NumberStyles.HexNumber);
//                    int wdc = int.Parse(WDCid, System.Globalization.NumberStyles.HexNumber);
//                    //cihaz cihaz = (from veri in db.cihaz where veri.wpsd == wpsd select veri).FirstOrDefault();
//                    //parkyeri park = (from veri in db.parkyeri where veri.wdc == wdc select veri).FirstOrDefault();
//                    int dolu = carState;

//                    Console.Write("park: {0}, cihaz:{1}, dolu mu: {2}\n", wdc, wpsd, dolu);
//                    //cihaz.songuncelleme = decbeat.rxtime;

//                    //if (dolu == cihaz.cihazdurumu)   /*  Cihaz durumunda bir değişiklik yok  */
//                    //{
//                    //    return;
//                    //}

//                    //if (dolu == 0)
//                    //{
//                    //    cihaz.cihazdurumu = 0;
//                    //    kayit = (from veri in db.anatablo where veri.cihazid == cihaz.cihazid && veri.cikiszamani == null select veri).FirstOrDefault();
//                    //    if (kayit != null)
//                    //    {
//                    //        kayit.cikiszamani = decbeat.rxtime;
//                    //    }

//                    //}
//                    //else
//                    //{
//                    //    cihaz.cihazdurumu = 1;
//                    //    kayit = new anatablo();
//                    //    kayit.cihazid = cihaz.cihazid;
//                    //    kayit.parkid = park.parkid;
//                    //    kayit.giriszamani = decbeat.rxtime;
//                    //    kayit.cikiszamani = null;
//                    //    db.anatablo.Add(kayit);

//                    //}


//                }
//                #endregion
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex);
//            }
//            //db.SaveChanges();

//        }

//        static void Main(string[] args)
//        {

//            /*
//            ParkingSerialPort sp;

//            try
//            {
//                sp = new ParkingSerialPort("COM1", 6000);
//                sp.Open();
//            }
//            catch (System.IO.IOException ipexp)
//            {
//                Console.WriteLine(ipexp.Message);
//            }
//            */
//            ParkingOriginalPacket.EvProcessReceivedPacket += sp_ProcessReceivedPacket;
//            Console.Write("Local ip: ");
//            string ip = Console.ReadLine();
//            Console.Write("Port: ");
//            string port = Console.ReadLine();
//            int portNum = int.Parse(port);
//            Thread m_serverThread;
//            IPAddress localIP;

//            localIP = IPAddress.Parse(ip);
//            tcpListener = new TcpListener(localIP, portNum);


//            try
//            {
//                tcpListener.Start();
//                m_serverThread = new Thread(new ThreadStart(ReceiveAccept));
//                m_serverThread.Start();
//                m_serverThread.IsBackground = true;

//            }
//            catch (SocketException ex)
//            {
//                tcpListener.Stop();
//                Console.Write("TCP Server Listen Error:" + ex.Message);
//            }
//            catch (Exception err)
//            {
//                Console.Write("TCP Server Listen Error:" + err.Message);
//            }



//            Console.ReadKey();

//        }


//    }

//}
