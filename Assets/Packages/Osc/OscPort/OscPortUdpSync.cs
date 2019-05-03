using System.Net.Sockets;
using System;
using System.Net;
using Osc;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Threading;

namespace Osc {
	public class OscPortUdpSync : OscPort {
		UdpClient _udp;
		Thread _reader;

        public override void OnEnable() {
			base.OnEnable ();
			try {
				_udp = new UdpClient (localPort, AddressFamily.InterNetwork);
				_reader = new Thread(Reader);
				_reader.Start();
			} catch (System.Exception e) {
				RaiseError (e);
			}
		}
        public override void OnDisable() {
			if (_udp != null) {
				_udp.Close();
				_udp = null;
			}
			if (_reader != null) {
				_reader.Abort ();
				_reader = null;
			}
			base.OnDisable ();
		}

		public override void Send(byte[] oscData, IPEndPoint remote) {
			try {
				_udp.Send (oscData, oscData.Length, remote);
			} catch (System.Exception e) {
				RaiseError (e);
			}
		}

			
		void Reader() {
			while (_udp != null) {
				try {
					var clientEndpoint = new IPEndPoint (IPAddress.Any, 0);
					var receivedData = _udp.Receive(ref clientEndpoint);
					_oscParser.FeedData (receivedData, receivedData.Length);
					while (_oscParser.MessageCount > 0) {
						lock (_received) {
							var msg = _oscParser.PopMessage ();
							Receive(new Capsule (msg, clientEndpoint));
						}
					}
				} catch (Exception e) {
					RaiseError (e);
				}
			}
		}
	}
}