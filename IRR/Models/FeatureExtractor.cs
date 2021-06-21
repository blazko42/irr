using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace IRR.Models
{
	public class FeatureExtractor
	{
		public int FEId { get; set; }
		public string Code { get; set; }
		public string FeatureExtractorName { get; set; }
		public bool IsActive { get; set; }
		public bool IsComputed { get; set; }

		public static List<FeatureExtractor> GetFeatureExtractors()
		{
			List<FeatureExtractor> featureExtractors = new List<FeatureExtractor>();

			using (SqlConnection connection = new SqlConnection(User.GetSessionUser().GetConnectionString()))
			{
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandText = "IRR.GetFeatureExtractors";
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				connection.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
					featureExtractors.Add(new FeatureExtractor()
					{
						FEId = Convert.ToInt32(reader["FEId"]),
						Code = (string)reader["Code"],
						FeatureExtractorName = (string)reader["FeatureExtractorName"],
						IsActive = Convert.ToBoolean(reader["IsActive"]),
						IsComputed = Convert.ToBoolean(reader["IsComputed"])
					});
				return featureExtractors;
			}
		}

		internal static void SetActiveFeatureExtractor(int? ftId)
		{
			using (SqlConnection connection = new SqlConnection(User.GetSessionUser().GetConnectionString()))
			{
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandText = "IRR.SetActiveFeatureExtractor";
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("FEId", ftId);
				connection.Open();
				cmd.ExecuteNonQuery();
			}
		}
	}
}