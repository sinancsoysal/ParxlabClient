using ParkingSystem;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ParxlabClient
{
    class Program
    {
        private static TcpListener tcpListener = null;
        private static IPAddress ipAddr;
        private static int port;
        private static Thread commThread;
        public static ParkingRemoteTCP wapper = new();

        public static void Main(string[] args)
        {
            ParkingOriginalPacket.EvProcessReceivedPacket += sp_ProcessReceivedPacket;

            ParseArguments(args);

            StartTcpListener();

            StartCommunicationThread();

            //ReceiveAccept();
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
            //else PrintUsage();
            else ipAddr = new IPAddress(new byte[] { 169, 254, 59, 39 }); port = 6000;
        }
        private static void StartTcpListener()
        {
            tcpListener = new(ipAddr, port);
            try
            {
                tcpListener.Start();
            }
            catch (SocketException se)
            {
                Console.WriteLine("[ERR] An error occured while trying to start TCP listener on IP: {0} at Port: {1}\nCaused by: {2}", ipAddr, port, se.ToString());
                Environment.Exit(-1);
            }
        }
        private static void StartCommunicationThread()
        {
            try
            {
                commThread = new(new ThreadStart(ReceiveAccept));
                commThread.Start();
                commThread.IsBackground = true;
                Console.WriteLine("[{0} INF] Communication thread successfully started.", DateTime.Now);
            }
            catch (Exception e)
            {
                Console.Write("[ERR] An error occured while trying to start communication thread.\nCaused by: {0}", e.ToString());
                Environment.Exit(-1);
            }
        }
        private static void ReceiveAccept()
        {
            while (true)
            {
                tcpListener.Start();
                TcpClient client = tcpListener.AcceptTcpClient();
                Console.WriteLine("[{0} INF] Connected.", DateTime.Now);
                wapper = new ParkingRemoteTCP(client);
            }
        }

        private static void sp_ProcessReceivedPacket(baseReceivedPacket pk)
        {
            Console.WriteLine(pk.ToString());

            //try
            //{
            //    byte revType = Convert.ToByte(pk.type_ver >> 8);
            //    string wpsdid = "";
            //    string WDCid = "";
            //    string RSSI = "";
            //    byte carState = 0;
            //    string voltage = "";
            //    string hardVer = "";
            //    string softVer = "";
            //    string hbPeriod = "";

            //    #region Senser Heart Beat
            //    if (pk is SensorHBeat)
            //    {
            //        SensorHBeat hb = (SensorHBeat)pk;
            //        wpsdid = (hb.WPSD_ID).ToString("X2").PadLeft(8, '0');
            //        WDCid = (hb.WDC_ID).ToString("X2").PadLeft(8, '0');
            //        softVer = "v" + int.Parse(hb.APP_VER.ToString("X2").Substring(0, 1)).ToString() + "." + int.Parse(hb.APP_VER.ToString("X2").Substring(1, 1)).ToString().PadLeft(2, '0');
            //        hardVer = ((int)(hb.HARD_VER) + 10).ToString();
            //        hardVer = "v" + hardVer.Substring(0, 1) + "." + hardVer.Substring(1, 1);
            //        voltage = (Math.Round((decimal)hb.VOLT / 10, 2)).ToString() + "V"; RSSI = ((Int16)hb.RSSI - 30).ToString();
            //        hbPeriod = hb.HB_PERIOD.ToString();
            //        carState = hb.CAR_STATE;

            //        int wpsd = int.Parse(wpsdid, System.Globalization.NumberStyles.HexNumber);
            //        int wdc = int.Parse(WDCid, System.Globalization.NumberStyles.HexNumber);

            //        int isParked = carState;

            //        Console.WriteLine("[STT] Park: {0}, Device: {1}, Is Parked: {2}", wdc, wpsd, isParked);
            //    }
            //    #endregion
            //    #region Sensor Detect
            //    else if (pk is SensorDetect)
            //    {
            //        SensorDetect decbeat = (SensorDetect)pk;
            //        wpsdid = (decbeat.WPSD_ID).ToString("X2").PadLeft(8, '0');
            //        WDCid = (decbeat.WDC_ID).ToString("X2").PadLeft(8, '0');
            //        hardVer = ((int)(decbeat.HARD_VER) + 10).ToString();
            //        hardVer = "v" + hardVer.Substring(0, 1) + "." + hardVer.Substring(1, 1);
            //        carState = decbeat.CAR_STATE;

            //        int wpsd = int.Parse(wpsdid, System.Globalization.NumberStyles.HexNumber);
            //        int wdc = int.Parse(WDCid, System.Globalization.NumberStyles.HexNumber);

            //        int isParked = carState;
            //        Console.WriteLine("[STT] Park: {0}, Device: {1}, Is Parked: {2}", wdc, wpsd, isParked);
            //    }
            //    #endregion
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
        }

        private static void PrintUsage()
        {
            Console.WriteLine("\nUsage:\nParxlabClient.exe <IP> <Port>\n\tIP: Local IP address that you want to connect\n" +
                "\t\tFor example: 169.254.69.96\n\tPort: Port number that you want to communicate on\n" +
                "\t\tFor example: 6900");
        }
    }
}