using System.Text.Json.Nodes;

namespace BackOffice.Common
{
	public class CriticalDomainRuleDetection
	{
		public static bool IsCritical(string jsonObject)
		{
			try
			{
				var jObject = JsonNode.Parse(jsonObject)!.AsObject();

				if (jObject.TryGetPropertyValue("minCredit", out var minCredit))
				{
					if ((int)minCredit! < 1000) return true;
				}

				return false;
			}
			catch 
			{
				return true;
			}
		}
	}
}
