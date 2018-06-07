using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;

public class UdpSender : MonoBehaviour {
	private static bool bSentMsg = false;
	//private Coroutine sendMsgCorountine = null;

    private string serverIp = "127.0.0.1";
    private int serverPort = 9999;

    // === Core functions ===================================
    //--------------------------------------------
    public void SetServerInfo(string _server, int _port)
    {
        serverIp = _server;
        serverPort = _port;
    }

    //--------------------------------------------
    void Update() {
		
	}

    // === UDP sender functions ===================================
    //--------------------------------------------
    public void SendUdpMessage(string _message)
    {
        SendUdpMessage(serverIp, serverPort, _message);
    }

    //--------------------------------------------
    public void SendUdpMessage(string _server, int _port, string _message)
	{
		/*if (sendMsgCorountine == null) {
			sendMsgCorountine = StartCoroutine (SendUdpMsgCoroutine (_server, _port, _message));
		}*/

        StartCoroutine(SendUdpMsgCoroutine(_server, _port, _message));
    }

	//--------------------------------------------
	IEnumerator SendUdpMsgCoroutine(string _server, int _port, string _message)
	{
		var udpClient = new UdpClient();
		udpClient.EnableBroadcast = true;
		udpClient.Connect(_server, _port);

		var sendBytes = System.Text.Encoding.ASCII.GetBytes(_message);
		bSentMsg = false;
		udpClient.BeginSend(sendBytes, sendBytes.Length, 
			new System.AsyncCallback(SentUdpCallback), udpClient);
		while (!bSentMsg) {
			yield return null;
		}

		udpClient.Close();
		//sendMsgCorountine = null;
	}

    //--------------------------------------------
    public void SendUdpBytes(byte[] _bytes)
    {
        SendUdpBytes(serverIp, serverPort, _bytes);
    }

    //--------------------------------------------
    public void SendUdpBytes(string _server, int _port, byte[] _bytes)
	{
		/*if (sendMsgCorountine == null) {
			sendMsgCorountine = StartCoroutine (SendUdpBytesCoroutine (_server, _port, _bytes));
		}*/

        StartCoroutine(SendUdpBytesCoroutine(_server, _port, _bytes));
    }

	//--------------------------------------------
	IEnumerator SendUdpBytesCoroutine(string _server, int _port, byte[] _bytes)
	{
		var udpClient = new UdpClient();
		udpClient.EnableBroadcast = true;

		udpClient.Send (_bytes, _bytes.Length, _server, _port);
		udpClient.Close();
		//sendMsgCorountine = null;

		yield return null;
	}

	//--------------------------------------------
	public static void SentUdpCallback(System.IAsyncResult _asyncResult)
	{
		// System.Net.Sockets.UdpClient u = (System.Net.Sockets.UdpClient)_asyncResult.AsyncState;
		bSentMsg = true;
	}

	// === WOL functions ===================================
	//--------------------------------------------
	public void SendMagicPacket(string _server, int _port, string _macAddress) {
		//Split on common MAC separators
		char? splitChar = null;
		if (_macAddress.Contains('-'))
			splitChar = '-';
		else if (_macAddress.Contains(':'))
			splitChar = ':';
		else if (_macAddress.Contains(' '))
			splitChar = ' ';

		//Turn MAC into array of bytes
		byte[] macAsArray;
		if (splitChar != null)
		{
			macAsArray = _macAddress.Split((char)splitChar)
				.Select(b => Convert.ToByte(b, 16))
				.ToArray<byte>();
		}
		else
		{
			//Jump through MAC-string, reading 2 chars at a time
			macAsArray = Enumerable.Range(0, _macAddress.Length)
				.Where(x => x % 2 == 0)
				.Select(x => Convert.ToByte(_macAddress.Substring(x, 2), 16)) //16 = hexadecimal
				.ToArray();
		}

		List<byte> magicPacket = new List<byte> { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

		//A WoLAN magic packet is just FF FF FF FF FF FF, then the target MAC adress repeated 16 times.
		for (int i = 0; i < 16; i++)
		{
			magicPacket = magicPacket.Concat(macAsArray).ToList();
		}

		SendUdpBytes(_server, _port, magicPacket.ToArray<byte>());
	}
}
