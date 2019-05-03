﻿using System.Net.Sockets;
using System;
using System.Net;
using Osc;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace Osc {
	public class OscPortUdpBegin : OscPort {
		UdpClient _udp;
		AsyncCallback _callback;

        public override void OnEnable() {
			base.OnEnable ();
			try {
				_callback = new AsyncCallback(HandleReceive);

				_udp = new UdpClient (localPort, AddressFamily.InterNetwork);
				_udp.BeginReceive(_callback, null);
			} catch (System.Exception e) {
				RaiseError (e);
			}
		}
        public override void OnDisable() {
			if (_udp != null) {
				_udp.Close();
				_udp = null;
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

		void HandleReceive(System.IAsyncResult ar) {
			try {
				if (_udp == null)
					return;
				var clientEndpoint = new IPEndPoint(0, 0);
				byte[] receivedData = _udp.EndReceive(ar, ref clientEndpoint);
				_oscParser.FeedData(receivedData, receivedData.Length);
				while (_oscParser.MessageCount > 0) {
					lock (_received) {
						var msg = _oscParser.PopMessage();
						Receive(new Capsule(msg, clientEndpoint));
					}
				}
			} catch (Exception e) {
				RaiseError (e);
			}
			_udp.BeginReceive(_callback, null);
		}
	}
}