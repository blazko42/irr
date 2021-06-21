using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;

namespace IRR_WS.Models
{
	public class FeatureExtractor
	{
		public int FEId { get; set; }
		public string Code { get; set; }
		public string FeatureExtractorName { get; set; }
		public bool IsActive { get; set; }
		public bool IsComputed { get; set; }

		internal static FeatureExtractor GetActiveFeatureExtractor()
		{
			using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
			{
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandText = "IRR.GetActiveFeatureExtractor";
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				connection.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
					return new FeatureExtractor()
					{
						FEId = Convert.ToInt32(reader["FEId"]),
						Code = (string)reader["Code"],
						FeatureExtractorName = (string)reader["FeatureExtractorName"],
						IsActive = Convert.ToBoolean(reader["IsActive"]),
						IsComputed = Convert.ToBoolean(reader["IsComputed"])
					};
				return null;
			}
		}
	}
}