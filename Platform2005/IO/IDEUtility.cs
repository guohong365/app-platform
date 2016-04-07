namespace Platform.IO
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public sealed class IDEUtility
    {
        private const uint DFP_GET_VERSION = 0x74080;
        private const uint DFP_RECEIVE_DRIVE_DATA = 0x7c088;
        private const uint FILE_SHARE_READ = 1;
        private const uint FILE_SHARE_WRITE = 2;
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const int IDE_ATA_IDENTIFY = 0xec;
        private const int IDE_ATAPI_IDENTIFY = 0xa1;
        private const int IDENTIFY_BUFFER_SIZE = 0x200;
        private const uint INVALID_HANDLE_VALUE = uint.MaxValue;
        private const uint OPEN_EXISTING = 3;

        [DllImport("kernel32.dll")]
        private static extern int CloseHandle(uint hObject);
        [DllImport("kernel32.dll")]
        private static extern uint CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, int lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, int hTemplateFile);
        [DllImport("kernel32.dll")]
        private static extern int DeviceIoControl(uint hDevice, uint dwIoControlCode, ref SENDCMDINPARAMS lpInBuffer, int nInBufferSize, ref SENDCMDOUTPARAMS lpOutBuffer, int nOutBufferSize, ref uint lpbytesReturned, int lpOverlapped);
        [DllImport("kernel32.dll")]
        private static extern int DeviceIoControl(uint hDevice, uint dwIoControlCode, int lpInBuffer, int nInBufferSize, ref GETVERSIONOUTPARAMS lpOutBuffer, int nOutBufferSize, ref uint lpbytesReturned, int lpOverlapped);
        public static string Read(byte drive)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                throw new NotSupportedException("½öÖ§³ÖWindowsNT/2000/XP");
            }
            uint hDevice = CreateFile(@"\\.\PhysicalDrive" + drive.ToString(), 0xc0000000, 3, 0, 3, 0, 0);
            if (hDevice != uint.MaxValue)
            {
                GETVERSIONOUTPARAMS lpOutBuffer = new GETVERSIONOUTPARAMS();
                uint lpbytesReturned = 0;
                if ((DeviceIoControl(hDevice, 0x74080, 0, 0, ref lpOutBuffer, Marshal.SizeOf(lpOutBuffer), ref lpbytesReturned, 0) != 0) && (lpOutBuffer.bIDEDeviceMap > 0))
                {
                    byte num3 = (((lpOutBuffer.bIDEDeviceMap >> drive) & 0x10) != 0) ? ((byte) 0xa1) : ((byte) 0xec);
                    SENDCMDINPARAMS lpInBuffer = new SENDCMDINPARAMS();
                    SENDCMDOUTPARAMS sendcmdoutparams = new SENDCMDOUTPARAMS();
                    lpInBuffer.cBufferSize = 0x200;
                    lpInBuffer.irDriveRegs.bFeaturesReg = 0;
                    lpInBuffer.irDriveRegs.bSectorCountReg = 1;
                    lpInBuffer.irDriveRegs.bCylLowReg = 0;
                    lpInBuffer.irDriveRegs.bCylHighReg = 0;
                    lpInBuffer.irDriveRegs.bDriveHeadReg = (byte) (160 | ((drive & 1) << 4));
                    lpInBuffer.irDriveRegs.bCommandReg = num3;
                    lpInBuffer.bDriveNumber = drive;
                    if (DeviceIoControl(hDevice, 0x7c088, ref lpInBuffer, Marshal.SizeOf(lpInBuffer), ref sendcmdoutparams, Marshal.SizeOf(sendcmdoutparams), ref lpbytesReturned, 0) != 0)
                    {
                        StringBuilder builder = new StringBuilder();
                        for (int i = 20; i < 40; i += 2)
                        {
                            builder.Append((char) sendcmdoutparams.bBuffer[i + 1]);
                            builder.Append((char) sendcmdoutparams.bBuffer[i]);
                        }
                        CloseHandle(hDevice);
                        return builder.ToString().Trim();
                    }
                }
                CloseHandle(hDevice);
            }
            return "";
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DRIVERSTATUS
        {
            public byte bDriverError;
            public byte bIDEStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
            public uint[] dwReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct GETVERSIONOUTPARAMS
        {
            public byte bVersion;
            public byte bRevision;
            public byte bReserved;
            public byte bIDEDeviceMap;
            public uint fCapabilities;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
            public uint[] dwReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct IDEREGS
        {
            public byte bFeaturesReg;
            public byte bSectorCountReg;
            public byte bSectorNumberReg;
            public byte bCylLowReg;
            public byte bCylHighReg;
            public byte bDriveHeadReg;
            public byte bCommandReg;
            public byte bReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct IDSECTOR
        {
            public ushort wGenConfig;
            public ushort wNumCyls;
            public ushort wReserved;
            public ushort wNumHeads;
            public ushort wBytesPerTrack;
            public ushort wBytesPerSector;
            public ushort wSectorsPerTrack;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=3)]
            public ushort[] wVendorUnique;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=20)]
            public string sSerialNumber;
            public ushort wBufferType;
            public ushort wBufferSize;
            public ushort wECCSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]
            public string sFirmwareRev;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=40)]
            public string sModelNumber;
            public ushort wMoreVendorUnique;
            public ushort wDoubleWordIO;
            public ushort wCapabilities;
            public ushort wReserved1;
            public ushort wPIOTiming;
            public ushort wDMATiming;
            public ushort wBS;
            public ushort wNumCurrentCyls;
            public ushort wNumCurrentHeads;
            public ushort wNumCurrentSectorsPerTrack;
            public uint ulCurrentSectorCapacity;
            public ushort wMultSectorStuff;
            public uint ulTotalAddressableSectors;
            public ushort wSingleWordDMA;
            public ushort wMultiWordDMA;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x80)]
            public byte[] bReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SENDCMDINPARAMS
        {
            public uint cBufferSize;
            public IDEUtility.IDEREGS irDriveRegs;
            public byte bDriveNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=3)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
            public uint[] dwReserved;
            public byte bBuffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SENDCMDOUTPARAMS
        {
            public uint cBufferSize;
            public IDEUtility.DRIVERSTATUS DriverStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x201)]
            public byte[] bBuffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SRB_IO_CONTROL
        {
            public uint HeaderLength;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]
            public string Signature;
            public uint Timeout;
            public uint ControlCode;
            public uint ReturnCode;
            public uint Length;
        }
    }
}
