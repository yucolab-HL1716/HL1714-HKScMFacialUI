using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;

public class UdpReceivedEventArgs : EventArgs
{
    public string message;

    public UdpReceivedEventArgs(string _msg)
    {
        message = _msg;
    }
}

public class UdpReceiver : MonoBehaviour {
	public int serverPort = 11999;
	private static bool bReceivedMsg = false;
	private bool bListening = false;

	private struct UdpState {
		public IPEndPoint endPoint;
		public UdpClient udpClient;
	}

    public delegate void UdpReceivedEventDlgt(object sender, UdpReceivedEventArgs e);
    public event UdpReceivedEventDlgt UdpReceived = delegate { };

    // === Core functions ===================================
    //--------------------------------------------
    void Start () {
	}

	//--------------------------------------------
	public void StartListening (int _port) {
		if (!bListening) {
			serverPort = _port;
			bListening = true;
			StartCoroutine (ReceiveMsgLoop ());

            YucoDebugger.instance.Log("UdpReceiver start listening at port " + _port, "StartListening", "UdpReceiver");
		}
	}

	//--------------------------------------------
	public void StopListening () {
		bListening = false;
	}

	// === UDP receive functions ===================================
	//--------------------------------------------
	IEnumerator ReceiveMsgLoop() {
		var endPt = new IPEndPoint(IPAddress.Any, serverPort);
		var udpClient = new UdpClient(endPt);
		udpClient.EnableBroadcast = true;

		var udpState = new UdpState();
		udpState.endPoint = endPt;
		udpState.udpClient = udpClient;

		while(bListening) {
			bReceivedMsg = false;
			udpClient.BeginReceive(new System.AsyncCallback(ReceiveCallback), udpState);
			while (!bReceivedMsg) {
				yield return null;
			}
		}
	}

	//--------------------------------------------
	public void ReceiveCallback(System.IAsyncResult asyncResult) {
		UdpClient udpClient = (UdpClient)((UdpState)(asyncResult.AsyncState)).udpClient;
		IPEndPoint endPt = (IPEndPoint)((UdpState)(asyncResult.AsyncState)).endPoint;
		var receiveBytes = udpClient.EndReceive(asyncResult, ref endPt);
		var receiveString = System.Text.Encoding.ASCII.GetString(receiveBytes);
        
        bReceivedMsg = true;

        //YucoDebugger.instance.debug(string.Format("UDP ReceiveCallback: {0}", receiveString), "ReceiveCallback", "UdpReceiver");
        UdpReceived(this, new UdpReceivedEventArgs(receiveString));
    }

}
