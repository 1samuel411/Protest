using Newtonsoft.Json;
using ProtestBackend.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ProtestBackend.Controllers.Locations
{
    public class LocationController : Controller
    {
        public const string GEOCODINGAPI = "GeocodingAPI";
        public const string GEOCODINGAPITimeZone = "GeocodingAPIZones";

        // GET: Location
        // POST: Location
        public ActionResult GetCoords()
        {
            string location = Request.QueryString["location"];
            if (String.IsNullOrEmpty(location))
                location = Request.Form["location"];

            if (String.IsNullOrEmpty(location))
                return Content(Error.Create("Invalid request"));


            var request = WebRequest.Create(string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}", Uri.EscapeDataString(location), Uri.EscapeDataString(WebConfigurationManager.AppSettings[GEOCODINGAPI]))) as HttpWebRequest;

            string responseContent = null;

            try
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Content(Error.Create("Invalid request"));
            }

            dynamic Data = JsonConvert.DeserializeObject(responseContent);
            if(Data.results.Count <= 0)
            {
                return Content(Error.Create("Invalid request"));
            }
            float latitude = Data.results[0].geometry.location.lat;
            float longitude = Data.results[0].geometry.location.lng;
            string address = Data.results[0].formatted_address;
            return Content(SuccessCoordinates.Create("Successfully got the coordinates", address, (float) Math.Round(latitude,4), (float) Math.Round(longitude,4)));
        }
    }
}