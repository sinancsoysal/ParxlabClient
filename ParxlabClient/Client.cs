using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ParkingSystem;

namespace ParxlabClient
{
    class Client
    {
        /// <summary>
        /// <remarks>TCP Instantiation</remarks>
        /// </summary>
        public static ParkingSystem.ParkingRemoteTCP wapper = new ParkingRemoteTCP();
        /// <summary>
        /// <remarks>tcp Listener</remarks>
        /// </summary>
        private static TcpListener tcpListener = null;
        /// <summary>
        /// <remarks>IP Address</remarks>
        /// </summary>
        public static IPAddress ipAddr = new IPAddress(new byte[] { 169, 254, 59, 39 });
        /// <summary>
        /// <remarks>TCP portNum</remarks>
        /// </summary>
        public static int port = 6000;
        /// <summary>
        /// <remarks>Tcp Client</remarks>
        /// </summary>
        private static TcpClient client = new TcpClient();
        /// <summary>
        /// <remarks>Tcp Thread</remarks>
        /// </summary>
        private Thread m_serverThread;

        public Client(IPAddress ipAddress, int portNum)
        {
            ipAddr = ipAddress;
            port = portNum;
            ParkingOriginalPacket.EvProcessReceivedPacket += sp_ProcessReceivedPacket;
        }

        /// <summary>
        /// Start TCP Listening
        /// </summary>
        public void TCP_StartListen()
        {
            try
            {
                tcpListener = new TcpListener(ipAddr, port);
                tcpListener.Start();
                m_serverThread = new Thread(new ThreadStart(ReceiveAccept));
                m_serverThread.Start();
                m_serverThread.IsBackground = true;
                Console.WriteLine("[{0} INF] Communication thread successfully started.", DateTime.Now);
            }
            catch (SocketException se)
            {
                tcpListener.Stop();
                Console.WriteLine("[ERR] An error occured while trying to start TCP listener on IP: {0} at Port: {1}\nCaused by: {2}", ipAddr, port, se.ToString());
                Environment.Exit(-1);
            }
            catch (Exception e)
            {
                Console.Write("[ERR] An error occured while trying to start communication thread.\nCaused by: {0}", e.ToString());
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// TCP Client Accept
        /// </summary>
        private void ReceiveAccept()
        {
            while (true)
            {
                try
                {
                    client = tcpListener.AcceptTcpClient();
                    Console.WriteLine("[{0} INF] Connected.", DateTime.Now);
                    wapper = new ParkingRemoteTCP(client);
                }
                catch (Exception ex)
                {
                    //eth_Setclose();
                    // MessageBox.Show(ex.Message, "?", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        #region Received Process
        /// <summary>
        /// <remarks>Process Received Packet</remarks>
        /// </summary>
        /// <param name="pk">Received Packet</param>
        private void sp_ProcessReceivedPacket(baseReceivedPacket pk)
        {
            Console.WriteLine(pk.ToString());

            try
            {
                byte revType = Convert.ToByte(pk.type_ver >> 8);
                string wpsdid = "";
                string WDCid = "";
                string RSSI = "";
                byte carState = 0;
                string voltage = "";
                string hardVer = "";
                string softVer = "";
                string hbPeriod = "";

                #region Senser Heart Beat
                if (pk is SensorHBeat)
                {
                    SensorHBeat hb = (SensorHBeat)pk;
                    wpsdid = (hb.WPSD_ID).ToString("X2").PadLeft(8, '0');
                    WDCid = (hb.WDC_ID).ToString("X2").PadLeft(8, '0');
                    softVer = "v" + int.Parse(hb.APP_VER.ToString("X2").Substring(0, 1)).ToString() + "." + int.Parse(hb.APP_VER.ToString("X2").Substring(1, 1)).ToString().PadLeft(2, '0');
                    hardVer = ((int)(hb.HARD_VER) + 10).ToString();
                    hardVer = "v" + hardVer.Substring(0, 1) + "." + hardVer.Substring(1, 1);
                    voltage = (Math.Round((decimal)hb.VOLT / 10, 2)).ToString() + "V"; RSSI = ((Int16)hb.RSSI - 30).ToString();
                    hbPeriod = hb.HB_PERIOD.ToString();
                    carState = hb.CAR_STATE;

                    int wpsd = int.Parse(wpsdid, System.Globalization.NumberStyles.HexNumber);
                    int wdc = int.Parse(WDCid, System.Globalization.NumberStyles.HexNumber);

                    int isParked = carState;

                    Console.WriteLine("[STT] Park: {0}, Device: {1}, Is Parked: {2}", wdc, wpsd, isParked);
                }
                #endregion
                #region Sensor Detect
                else if (pk is SensorDetect)
                {
                    SensorDetect decbeat = (SensorDetect)pk;
                    wpsdid = (decbeat.WPSD_ID).ToString("X2").PadLeft(8, '0');
                    WDCid = (decbeat.WDC_ID).ToString("X2").PadLeft(8, '0');
                    hardVer = ((int)(decbeat.HARD_VER) + 10).ToString();
                    hardVer = "v" + hardVer.Substring(0, 1) + "." + hardVer.Substring(1, 1);
                    carState = decbeat.CAR_STATE;

                    int wpsd = int.Parse(wpsdid, System.Globalization.NumberStyles.HexNumber);
                    int wdc = int.Parse(WDCid, System.Globalization.NumberStyles.HexNumber);

                    int isParked = carState;
                    Console.WriteLine("[STT] Park: {0}, Device: {1}, Is Parked: {2}", wdc, wpsd, isParked);
                }
                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        #endregion Received Process
    }
}
