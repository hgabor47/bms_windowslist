using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsList
{
    class Program
    {
        static string shipUUID = "08440500-6940-4d89-a007-fe598f26e146";  //This Pod is a SHIP
        const bool DEBUG_OnlyWin = false;

        static BabylonMS.BabylonMS bms;
        static BabylonMS.BMSPack outputpack;
        static void Main(string[] args)
        {
            outputpack = new BabylonMS.BMSPack();
            bms = BabylonMS.BabylonMS.ShipDocking(shipUUID, args);
            bms.NewInputFrame += NewInputFrame;
            bms.OpenGate(true);
        }
        static void NewInputFrame(BabylonMS.BMSEventSessionParameter session) //newinput frame or continuous 
        {
            try
            {
                lista();                
                bms.TransferPacket(session.writer,outputpack,false);
                outputpack.Clear();
            }
            catch (Exception ) { };
        }
        
        /// <summary>
        /// Window list
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);
        // This static method is required because Win32 does not support
        // GetWindowLongPtr directly
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            IntPtr a;
            if (IntPtr.Size == 8)
                a = GetWindowLongPtr64(hWnd, nIndex);
            else
                a = GetWindowLongPtr32(hWnd, nIndex);
            return a;
        }
        static IntPtr WindowType(IntPtr handle)
        {
            return GetWindowLongPtr(handle, -16);
        }


        static void lista()
        {
            Process[] processes = Process.GetProcesses();
            //int i = 0;
            IntPtr self = Process.GetCurrentProcess().MainWindowHandle;
            foreach (Process p in processes)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                    long typ = (long)WindowType(p.MainWindowHandle); //https://msdn.microsoft.com/en-us/library/windows/desktop/ms632600(v=vs.85).aspx
                    if (p.MainWindowHandle.ToInt64() != self.ToInt64())
                    {
                        if (false || (
                            //((typ & 0x80000000L)==0) &&
                            ((typ & 0x20000000L) == 0)
                        )
                        )
                        {
                            if (DEBUG_OnlyWin)
                            {
                                if ((p.MainWindowTitle.CompareTo("{C:\\Program Files (x86)\\VideoLAN\\VLC} - Far 3.0.4400 x86") == 0)
                                    || (p.MainWindowTitle.CompareTo("{C:\\Program Files (x86)\\VideoLAN\\VLC} - Far 3.0.4545 x86") == 0)
                                    || (p.MainWindowTitle.CompareTo("Számológép") == 0)
                                    )
                                {
                                    outputpack.AddField("HWND", BabylonMS.BabylonMS.CONST_FT_INT64);
                                    outputpack.GetField(outputpack.FieldsCount()-1 ).Value(p.MainWindowHandle.ToInt64());
                                }
                            }
                            else
                            {
                                if ((!p.MainWindowTitle.StartsWith("Android Monitor")) &&
                                    (!p.MainWindowTitle.StartsWith("Prelimutens")) &&
                                    (!p.MainWindowTitle.StartsWith("VRMainContentExporter")))
                                {
                                    outputpack.AddField("HWND", BabylonMS.BabylonMS.CONST_FT_INT64);
                                    outputpack.GetField(outputpack.FieldsCount() - 1).Value(p.MainWindowHandle.ToInt64());
                                }
                            }
                        }
                    }
                }
            }
        }



    }
}
