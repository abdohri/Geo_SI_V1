using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Ext.Net;
using GeoSI.Website.Common;

namespace GeoSI.Website.Handler
{
    /// <summary>
    /// Summary description for GetVehiculeByZone
    /// </summary>
    public class GetVehiculeByZone : IHttpHandler
    {

        // Remplissage du DATASET
        protected DataSet json_encode(string sql)
        {
            DataSet ds = new DataSet();
            using (SqlDataAdapter da = new SqlDataAdapter(sql, _Global.CurrentConnection))
            {
                try
                {

                    da.SelectCommand.CommandTimeout = 120;
                    da.Fill(ds);
                }
                catch (Exception ex) { }

            }


            return ds;


        }
        //Accès à la base de données
        protected SqlDataReader Select(string _Requette)
        {
            SqlDataReader dr;
            try
            {
                SqlConnection cnx = _Global.CurrentConnection;
                SqlCommand cmd = new SqlCommand(_Requette, cnx);
                dr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                dr = null;
                ex.ToString();
            }
            return dr;
        }
        //Récuération des donées du trajet 
        public void ProcessRequest(HttpContext context)
        {

            context.Response.ContentType = "application/json";
            //long _Unitid = long.Parse(context.Request.Form["Unitid"]);
            //string _req1 = "select top 1 CONVERT(VARCHAR(10), GPSDateTime, 103) date, DATENAME(hh,GPSDateTime)+':'+ DATENAME(mi,GPSDateTime)+':'+ DATENAME(SS,GPSDateTime) time,d.latitude,d.longitude,d.speed,d.contact,v.matricule,v.code from datatracker d,boitier b,vehicules v,affectation_vehicule_boitier avb where d.imei=999999 and b.imei=d.imei and b.boitierid=avb.boitierid and avb.vehiculeid=v.vehiculeid order by GPSDateTime desc ";
            int _Unitid = int.Parse(context.Request.Form["Unitid"]);



            string _req1 = "select * from ( "
+" select  vz.vehiculeid ,d.latitude,d.longitude,d.contact,d.speed, "
+ " d.GPSDateTime,d.Odometer,v.matricule,tv.libelle,vz.interdit,v.imgVehicule,"
+"     ROW_NUMBER() OVER (PARTITION BY vz.vehiculeid ORDER BY vz.vehiculeid DESC , d.datatrackerid desc ) AS rn "
+"  from Vehicule_Zone vz "
+" inner join affectation_vehicule_boitier abv on abv.vehiculeid=vz.vehiculeid "
+" inner join boitier b on b.boitierid=abv.boitierid "
+" inner join Datatracker d on d.imei=b.imei "
+" inner join vehicules v on v.vehiculeid=vz.vehiculeid "
+" inner join typevehicule tv on tv.typevehiculeid=v.typevehiculeid "

+ " where zoneid="+_Unitid+" )aff  where rn=1 ";
            bool hasRows = json_encode(_req1).Tables.Cast<DataTable>().Any(table => table.Rows.Count != 0);

            if (hasRows)
                context.Response.Write(JSON.Serialize(this.json_encode(_req1).Tables[0]));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}