using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Serializable]
public struct MoveData
{
    [MarshalAs(UnmanagedType.R4)]
    public float m_x;
    [MarshalAs(UnmanagedType.R4)]
    public float m_y;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Serializable]
public struct RecvMoveData
{
    [MarshalAs(UnmanagedType.I4)]
    public int m_id;
    [MarshalAs(UnmanagedType.R4)]
    public float m_x;
    [MarshalAs(UnmanagedType.R4)]
    public float m_y;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Serializable]
public struct IDData
{
    [MarshalAs(UnmanagedType.I4)]
    public int m_id;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Serializable]
public struct ListData
{
    [MarshalAs(UnmanagedType.I4)]
    public int m_size;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public int[] m_list;
}