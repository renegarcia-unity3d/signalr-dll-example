// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

public class SignalRService
{
	public HubConnection connection;

	public event Action<string> OnConnected;

	public event Action<TelemetryMessage> OnTelemetryMessage;

	public event Action<PropertyMessage> OnPropertyMessage;

	public event Action OnDisconnected;

	~SignalRService()
	{
		if (connection != null)
		{
			connection.StopAsync();
			connection = null;
		}
	}

	public async Task StartAsync(string url)
	{
		connection = new HubConnectionBuilder()
			.WithUrl(url)
			.Build();
		connection.On<PropertyMessage>(nameof(PropertyMessage), message =>
		{
			OnPropertyMessage?.Invoke(message);
		});
		connection.On<TelemetryMessage>(nameof(TelemetryMessage), message =>
		{
			OnTelemetryMessage?.Invoke(message);
		});
		await connection.StartAsync();
		OnConnected?.Invoke(connection.State.ToString());
		connection.Closed += async error =>
		{
			OnDisconnected?.Invoke();
			await connection.StartAsync();
		};
	}
}
