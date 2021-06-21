using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

namespace IRR_WS.Controllers.api
{
	public class FindLandmarkController : ApiController
	{
		[HttpPost]
		public async Task<List<Models.Landmark>> FindLandmark()
		{
			// Check if the request contains multipart/form-data.  
			if (!Request.Content.IsMimeMultipartContent())
			{
				throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			}

			var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());

			Models.Landmark landmark = new Models.Landmark();

			IList<HttpContent> files = provider.Files;

			HttpContent file = files[0];
			byte[] fileContent = await file.ReadAsByteArrayAsync();
			var fileName = file.Headers.ContentDisposition.FileName.Trim('\"');

			landmark.LandmarkUploadedImageName = fileName;
			landmark.LandmarkUploadedIamgeContent = fileContent;

			try
			{
				List<string> similarLandmarks = Models.Landmark.GetSimilarImagesGUID(landmark.LandmarkUploadedIamgeContent, Models.FeatureExtractor.GetActiveFeatureExtractor().Code);

				return await Models.Landmark.RetrieveLandmarks(similarLandmarks);
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

	}
}