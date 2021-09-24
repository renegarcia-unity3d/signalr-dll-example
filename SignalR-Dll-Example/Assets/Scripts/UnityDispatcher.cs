// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.Unity
{
	/// <summary>
	/// A helper class for dispatching actions to run on various Unity threads.
	/// </summary>
	public class UnityDispatcher : MonoBehaviour
	{
		#region Member Variables

		static UnityDispatcher s_Instance;
		static Queue<Action> s_Queue = new Queue<Action>(8);
		static volatile bool s_Queued;

		#endregion // Member Variables

		#region Internal Methods

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Initialize()
		{
			lock (s_Queue)
			{
				if (s_Instance == null)
				{
					s_Instance = new GameObject("Dispatcher").AddComponent<UnityDispatcher>();
					DontDestroyOnLoad(s_Instance.gameObject);
				}
			}
		}

		#endregion // Internal Methods

		#region Unity Overrides

		protected virtual void Update()
		{
			// Action placeholder
			Action action;

			// Do this as long as there's something in the queue
			while (s_Queued)
			{
				// Lock only long enough to take an item
				lock (s_Queue)
				{
					// Get the next action
					action = s_Queue.Dequeue();

					// Have we exhausted the queue?
					if (s_Queue.Count == 0) { s_Queued = false; }
				}

				// Execute the action outside of the lock
				action();
			}
		}

		#endregion // Unity Overrides

		#region Public Methods

		/// <summary>
		/// Schedules the specified action to be run on Unity's main thread.
		/// </summary>
		/// <param name="action">
		/// The action to run.
		/// </param>
		public static void InvokeOnAppThread(Action action)
		{
			// Validate
			if (action == null) throw new ArgumentNullException(nameof(action));

			// Lock to be thread-safe
			lock (s_Queue)
			{
				// Add the action
				s_Queue.Enqueue(action);

				// Action is in the queue
				s_Queued = true;
			}
		}

		#endregion // Public Methods
	}

	// #endif // Not using UWP-specific version anymore
}
