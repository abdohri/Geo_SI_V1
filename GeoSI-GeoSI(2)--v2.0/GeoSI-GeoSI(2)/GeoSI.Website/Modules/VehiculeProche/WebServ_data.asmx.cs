﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Ext.Net;
using GeoSI.Website.Common;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace GeoSI.Website.Modules.VehiculeProche
{
    /// <summary>
    /// Summary description for WebServ_data
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebServ_data : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void get_list(string lat,string longitude, string raduis)
        {
            String _latitude = lat;
            String _longitude = longitude;
            string rayon = raduis;
            String query_vehciule = "select * from (select *, GEOGRAPHY::STPointFromText('POINT(' + CAST([longitude] AS VARCHAR(20))+ ' ' + CAST([latitude] AS VARCHAR(20)) + ')', 4326).STDistance(GEOGRAPHY::STGeomFromText('POINT(" + _longitude + " " + _latitude + ")', 4326))as distance from (select v.matricule, v.vehiculeid,v.typevehiculeid, b.imei,(select top 1 latitude from Datatracker d where d.imei = b.imei order by datatrackerid desc) as latitude,(select top 1 SendingDateTime from Datatracker d where d.imei = b.imei order by datatrackerid desc) as SendingDateTime,(select top 1 longitude from Datatracker d where d.imei = b.imei order by datatrackerid desc)  as longitude from vehicules v inner join affectation_vehicule_boitier vb on vb.vehiculeid = v.vehiculeid and vb.actif =1 inner join boitier b on b.boitierid = vb.boitierid and b.actif = 1 ) aff ) aff2 where (latitude !=0 and longitude!=0 and distance <=" + rayon + ") order by distance asc";
            SqlConnection conn = _Global.CurrentConnection;
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(query_vehciule, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            Context.Response.Flush();
            Context.Response.Write(JSON.Serialize(ds.Tables[0]));
            //string json = JSON.Serialize(dt);
            //return json;
            //return new JavaScriptSerializer().Serialize(dt);
            //return dt;

            //string json = DataSetToJSON(ds);
           // return ds.Tables[0];
            
        }


        public string DataSetToJSON(DataSet ds)
        {

            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (DataTable dt in ds.Tables)
            {
                object[] arr = new object[dt.Rows.Count + 1];

                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    arr[i] = dt.Rows[i].ItemArray;
                }

                dict.Add(dt.TableName, arr);
            }

            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(dict);
        }
    }
}
