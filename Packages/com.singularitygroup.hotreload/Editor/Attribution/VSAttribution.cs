using System;
using UnityEngine.Analytics;

#if UNITY_6000_0_OR_NEWER
namespace UnityEditor.VSAttribution.HotReload
{
	internal static class VSAttribution
	{
		
		const int k_VersionId = 4;
		const int k_MaxEventsPerHour = 10;
		const int k_MaxNumberOfElements = 1000;

		const string k_VendorKey = "unity.vsp-attribution";
		const string k_EventName = "vspAttribution";
		
		[Serializable]
		private class VSAttributionData : IAnalytic.IData {
			public string actionName;
			public string partnerName;
			public string customerUid;
			public string extra;
		}
		
		[AnalyticInfo(k_EventName, k_VendorKey, k_VersionId, k_MaxEventsPerHour, k_MaxNumberOfElements)]
		class VSAttributionAnalytics : IAnalytic {
			private VSAttributionData m_Data;

			private VSAttributionAnalytics(
				 string actionName,
				 string partnerName,
				 string customerUid,
				 string extra
			) {
				this.m_Data = new VSAttributionData {
					actionName = actionName,
					partnerName = partnerName,
					customerUid = customerUid,
					extra = extra
				};
			}

			public bool TryGatherData(out IAnalytic.IData data, out Exception error) {
				data = this.m_Data;
				error = null;
				return this.m_Data != null;
			}

			public static AnalyticsResult SendEvent(
				string actionName,
				string partnerName,
				string customerUid,
				string extra
			   ) {
				return EditorAnalytics.SendAnalytic(new VSAttributionAnalytics(actionName,
					partnerName,
					customerUid,
					extra
				 ));
			}
		}

		/// <summary>
		/// Registers and attempts to send a Verified Solutions Attribution event.
		/// </summary>
		/// <param name="actionName">Name of the action, identifying a place this event was called from.</param>
		/// <param name="partnerName">Identifiable Verified Solutions Partner's name.</param>
		/// <param name="customerUid">Unique identifier of the customer using Partner's Verified Solution.</param>
		public static AnalyticsResult SendAttributionEvent(string actionName, string partnerName, string customerUid)
		{
			try
			{
				return VSAttributionAnalytics.SendEvent(actionName, partnerName, customerUid, "{}");
			}
			catch
			{
				// Fail silently
				return AnalyticsResult.AnalyticsDisabled;
			}
		}
	}
}
#else
namespace UnityEditor.VSAttribution.HotReload
{
	internal static class VSAttribution
	{
		const int k_VersionId = 4;
		const int k_MaxEventsPerHour = 10;
		const int k_MaxNumberOfElements = 1000;

		const string k_VendorKey = "unity.vsp-attribution";
		const string k_EventName = "vspAttribution";

		static bool RegisterEvent()
		{
			AnalyticsResult result = EditorAnalytics.RegisterEventWithLimit(k_EventName, k_MaxEventsPerHour,
				k_MaxNumberOfElements, k_VendorKey, k_VersionId);

			var isResultOk = result == AnalyticsResult.Ok;
			return isResultOk;
		}

		[Serializable]
		struct VSAttributionData
		{
			public string actionName;
			public string partnerName;
			public string customerUid;
			public string extra;
		}

		/// <summary>
		/// Registers and attempts to send a Verified Solutions Attribution event.
		/// </summary>
		/// <param name="actionName">Name of the action, identifying a place this event was called from.</param>
		/// <param name="partnerName">Identifiable Verified Solutions Partner's name.</param>
		/// <param name="customerUid">Unique identifier of the customer using Partner's Verified Solution.</param>
		public static AnalyticsResult SendAttributionEvent(string actionName, string partnerName, string customerUid)
		{
			try
			{
				// Are Editor Analytics enabled ? (Preferences)
				if (!EditorAnalytics.enabled)
					return AnalyticsResult.AnalyticsDisabled;

				if (!RegisterEvent())
					return AnalyticsResult.InvalidData;

				// Create an expected data object
				var eventData = new VSAttributionData
				{
					actionName = actionName,
					partnerName = partnerName,
					customerUid = customerUid,
					extra = "{}"
				};

				return EditorAnalytics.SendEventWithLimit(k_EventName, eventData, k_VersionId);
			}
			catch
			{
				// Fail silently
				return AnalyticsResult.AnalyticsDisabled;
			}
		}
	}
}
#endif
