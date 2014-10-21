﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using GeoSI.Website.Common;


namespace GeoSI.Website.Modules.VehiculeProche
{
    public partial class LoadVehicule : Form
    {

        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetNomModule("VehiculeProche");
            
             String _req1 = "SELECT poiid,libelle  FROM Poi where clientid=" + this.getCurrentUser().getClientId() + " and actif='1'";
                    this.FillStore(_req1, this.StorePoint);

                    
              
        }
        protected void MyData_Refresh(object sender, StoreReadDataEventArgs e)
        {
            String _latitude = this.latitude.Text.ToString();
            String _longitude = this.longitude.Text.ToString();
            string rayon = this.txt_raduis.Text.ToString();
            //String req_vehciule = "select * from (select *, SQRT(SQUARE(latitude-" + _latitude + ")+ SQUARE((-1*longitude)+" + _longitude + ")) as distance from (select v.matricule, v.vehiculeid, b.imei, (select top 1 latitude from Datatracker d where d.imei = b.imei order by SendingDateTime desc) as latitude , (select top 1 longitude from Datatracker d where d.imei = b.imei order by SendingDateTime desc)  as longitude from vehicules v inner join affectation_vehicule_boitier vb on vb.vehiculeid = v.vehiculeid and vb.actif =1 inner join boitier b on b.boitierid = vb.boitierid and b.actif = 1 ) aff ) aff2 order by distance asc";
            String query_vehciule = "select * from (select *, GEOGRAPHY::STPointFromText('POINT(' + CAST([longitude] AS VARCHAR(20))+ ' ' + CAST([latitude] AS VARCHAR(20)) + ')', 4326).STDistance(GEOGRAPHY::STGeomFromText('POINT(" + _longitude + " " + _latitude + ")', 4326))as distance from (select v.matricule, v.vehiculeid, b.imei,(select top 1 latitude from Datatracker d where d.imei = b.imei order by SendingDateTime desc) as latitude,(select top 1 SendingDateTime from Datatracker d where d.imei = b.imei order by SendingDateTime desc) as SendingDateTime,(select top 1 longitude from Datatracker d where d.imei = b.imei order by SendingDateTime desc)  as longitude from vehicules v inner join affectation_vehicule_boitier vb on vb.vehiculeid = v.vehiculeid and vb.actif =1 inner join boitier b on b.boitierid = vb.boitierid and b.actif = 1 ) aff ) aff2 where (latitude !=0 and longitude!=0 and distance <=" + rayon + ") order by distance asc";
            this.FillStore(query_vehciule, this.StoreProche);
        }


        public void Recherche(object sender, DirectEventArgs e)
        {
           // ScriptManager.RegisterStartupScript(, this.GetType(), "e", "codeAddress", true);
            String _latitude= this.latitude.Text.ToString();
            String _longitude = this.longitude.Text.ToString();
            string rayon = this.txt_raduis.Text.ToString();
            //String req_vehciule = "select * from (select *, SQRT(SQUARE(latitude-" + _latitude + ")+ SQUARE((-1*longitude)+" + _longitude + ")) as distance from (select v.matricule, v.vehiculeid, b.imei, (select top 1 latitude from Datatracker d where d.imei = b.imei order by SendingDateTime desc) as latitude , (select top 1 longitude from Datatracker d where d.imei = b.imei order by SendingDateTime desc)  as longitude from vehicules v inner join affectation_vehicule_boitier vb on vb.vehiculeid = v.vehiculeid and vb.actif =1 inner join boitier b on b.boitierid = vb.boitierid and b.actif = 1) aff ) aff2 order by distance asc";
            String query_vehciule = "select * from (select *, GEOGRAPHY::STPointFromText('POINT(' + CAST([longitude] AS VARCHAR(20))+ ' ' + CAST([latitude] AS VARCHAR(20)) + ')', 4326).STDistance(GEOGRAPHY::STGeomFromText('POINT(" + _longitude + " " + _latitude + ")', 4326))as distance from (select v.matricule, v.vehiculeid,v.typevehiculeid, b.imei,(select top 1 latitude from Datatracker d where d.imei = b.imei order by datatrackerid desc) as latitude,(select top 1 longitude from Datatracker d where d.imei = b.imei order by datatrackerid desc)  as longitude from vehicules v inner join affectation_vehicule_boitier vb on vb.vehiculeid = v.vehiculeid and vb.actif =1 inner join boitier b on b.boitierid = vb.boitierid and b.actif = 1 ) aff ) aff2 where (latitude !=0 and longitude!=0 and distance <=" + rayon + ") order by distance asc";
            this.FillStore(query_vehciule, this.StoreProche);
        }
                        

       
        public string get_list() 
        {
            String _latitude = this.latitude.Text.ToString();
            String _longitude = this.longitude.Text.ToString();
            string rayon = this.txt_raduis.Text.ToString();
            String sql_vehciule = "select * from (select *, GEOGRAPHY::STPointFromText('POINT(' + CAST([longitude] AS VARCHAR(20))+ ' ' + CAST([latitude] AS VARCHAR(20)) + ')', 4326).STDistance(GEOGRAPHY::STGeomFromText('POINT(" + _longitude + " " + _latitude + ")', 4326))as distance from (select v.matricule, v.vehiculeid,v.typevehiculeid, b.imei,(select top 1 latitude from Datatracker d where d.imei = b.imei order by SendingDateTime desc) as latitude,(select top 1 longitude from Datatracker d where d.imei = b.imei order by SendingDateTime desc)  as longitude from vehicules v inner join affectation_vehicule_boitier vb on vb.vehiculeid = v.vehiculeid and vb.actif =1 inner join boitier b on b.boitierid = vb.boitierid and b.actif = 1 ) aff ) aff2 where (latitude !=0 and longitude!=0 and distance <=" + rayon + ") order by distance asc";
            string sql = "select * from Datatracker";
            SqlConnection conn = _Global.CurrentConnection;
            DataTable dt = new DataTable();
            //SqlCommand cmd = new SqlCommand(query_vehciule, conn);
            SqlDataAdapter da = new SqlDataAdapter(sql_vehciule, conn);
            da.Fill(dt);
            string json = JSON.Serialize(dt);
            return json;
        }

        public void Getlat(String _poiid)
        {
            
              String req_lat = "select latitude from Poi where libelle='" + _poiid + "' and clientid=" + this.getCurrentUser().getClientId() + " and actif='1'";

            SqlConnection cnx1 = _Global.CurrentConnection;
            SqlCommand cmd1 = new SqlCommand(req_lat, cnx1);
             cmd1.ExecuteScalar().ToString();

             this.latitude.Text = (String)cmd1.ExecuteScalar();
           
        }
        public void Getlon(String _poiid)
        {

            String req_lon = "select longitude from Poi where libelle='" + _poiid + "' and clientid=" + this.getCurrentUser().getClientId() + " and actif='1'";

            SqlConnection cnx = _Global.CurrentConnection;
            SqlCommand cmd = new SqlCommand(req_lon, cnx);
            String lon = cmd.ExecuteScalar().ToString();

            this.longitude.Text = lon;
        }
        public void GetPosition(object sender, DirectEventArgs e)
        {


            String _poiid = this.poiid.Text.ToString();
            this.Getlat(_poiid);
            this.Getlon(_poiid);

           
        }
        public void Vider(object sender, DirectEventArgs e)
        {

            this.latitude.Text = "";
            this.longitude.Text = "";
           
            this.adresse.Text = "";
       
        }
        public void Vider()
        {
              this.latitude.Text = "";
            this.longitude.Text = "";
         
            this.adresse.Text = "";
       
          
        }

        protected void Button_DirectClick(object sender, DirectEventArgs e)
        {

        }
    }
 }
