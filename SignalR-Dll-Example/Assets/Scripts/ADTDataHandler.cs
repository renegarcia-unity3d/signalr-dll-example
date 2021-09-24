// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Unity;
using UnityEngine;

public class AdtDataHandler : MonoBehaviour
{
	SignalRService m_SignalRService;

	public string Url = "";

	void Start()
	{
		this.RunSafeVoid(CreateServiceAsync);
	}

	void OnDestroy()
	{
		if (m_SignalRService != null)
		{
			m_SignalRService.OnConnected -= HandleConnected;
			m_SignalRService.OnTelemetryMessage -= HandleTelemetryMessage;
			m_SignalRService.OnDisconnected -= HandleDisconnected;
			m_SignalRService.OnPropertyMessage -= HandlePropertyMessage;
		}
	}

	/// <summary>
	/// Received a message from SignalR. Note, this message is received on a background thread.
	/// </summary>
	/// <param name="message">
	/// The message.
	/// </param>
	void HandleTelemetryMessage(TelemetryMessage message)
	{
		// Finally update Unity GameObjects, but this must be done on the Unity Main thread.
		UnityDispatcher.InvokeOnAppThread(() =>
		{
			Debug.Log(message);
		});
	}

	/// <summary>
	/// Received a Property message from SignalR. Note, this message is received on a background thread.
	/// </summary>
	/// <param name="message">
	/// The message
	/// </param>
	void HandlePropertyMessage(PropertyMessage message)
	{
		UnityDispatcher.InvokeOnAppThread(() =>
		{
			Debug.Log(message);
		});
	}

	Task CreateServiceAsync()
	{
		m_SignalRService = new SignalRService();
		m_SignalRService.OnConnected += HandleConnected;
		m_SignalRService.OnDisconnected += HandleDisconnected;
		m_SignalRService.OnTelemetryMessage += HandleTelemetryMessage;
		m_SignalRService.OnPropertyMessage += HandlePropertyMessage;

		return m_SignalRService.StartAsync(Url);
	}

	void HandleConnected(string obj)
	{
		Debug.Log("Connected");
	}

	void HandleDisconnected()
	{
		Debug.Log("Disconnected");
	}
}
