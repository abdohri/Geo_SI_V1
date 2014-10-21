using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Threading;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using Ext.Net;
using System.Web.Security;
using GeoSI.Website.Common;
using System.Net.Mail;

using System.Web.UI.WebControls;
using System.Threading;
using System.Net;
using System.Net.Mail;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using GeoSI.Website.Common;
using Ext.Net;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;

namespace GeoSI.Website.Common
{
    public class Rapports : Page
    {




        PageBase pp = new PageBase();

        public void histo_arret()
        {
            Server.ScriptTimeout = 600;
           try
           {
                List<string> tab = new List<string>();

                string re = " drop table temp_runk select *  into  temp_runk from tts_runk2 "
                 + " select case when aff.contact = 0 then 'Arret' else 'depart' end as Etat,  aff.imei,p.personnelid,vp.vehiculeid , "
    + " aff.date as date1 ,aff2.date as date2, aff2.adress as depart , "
    + "  aff.adress as arrive, DATEDIFF(minute,  aff2.date,aff.date) ,aff.latitude,aff.longitude "
    + "  from (select *, ROW_NUMBER() over(order by runk) as n from temp_runk rn) aff "
    + " left outer join (select *, ROW_NUMBER() over(order by runk) as n from temp_runk rn )aff2 "
    + " on aff2.imei = aff.imei and aff.n = aff2.n+1  "
    + " inner join boitier b on b.imei=aff.imei "
    + "  inner join affectation_vehicule_boitier avb on avb.boitierid=b.boitierid  "
    + " inner join vehicule_personnel vp on vp.vehiculeid=avb.vehiculeid "
    + " inner join personnel p on p.personnelid=vp.personnelid "
    + " where aff.date > DATEADD (HOUR,-24,CURRENT_TIMESTAMP) ";
                SqlDataReader dr = pp.Select(re);


                while (dr.Read())
                {
                    string t = dr[0].ToString();
                    string t1 = dr[1].ToString();
                    string t2 = dr[2].ToString();
                    string t3 = dr[3].ToString();
                    string t4 = dr[4].ToString();
                    string t5 = dr[5].ToString();
                    string t15 = dr[6].ToString();
                    string t25 = dr[7].ToString();
                    string t35 = dr[8].ToString();

                 //   string t035 =dr[9].ToString().Replace(",", ".");
                 //   float aa = float.Parse(t035, System.Globalization.CultureInfo.InvariantCulture);
                   // float t0351 = float.Parse(dr[10].ToString().Replace(",", "."));

                    if (dr[0].ToString() == "Arret" && dr[8] != null && dr[8].ToString() != "" && dr[5] != null && dr[5].ToString() != "" && dr[9] != null && dr[9].ToString() != "" && dr[10] != null && dr[10].ToString() != "")
                    {


                        tab.Add("('" + dr[6].ToString().Replace("'"," ") + "','" + dr[5].ToString() + "'," + dr[8].ToString() + "," + dr[3].ToString() + "," + dr[2].ToString() + "," + float.Parse(dr[9].ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + "," + float.Parse(dr[10].ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + ")");
                    }
                }
                dr.Close();
                if (tab.Count > 0)
                {
                    string req = "insert into Historique_Arret(localiisation ,date,duree,vehiculeid,personnelid,latitude,longitude ) values ";
                    for (int i = 0; i < tab.Count; i++)
                    {
                        req = req + tab[i];
                        if (i == 995)
                        {
                            break;
                        }
                        if (i + 1 != tab.Count)
                        {
                            req = req + " , ";

                        }
                    }

                    pp.Insert(req);

                }
            }
            catch (Exception ee) { }


        }


        public void histo_trajet()
        {
            try
            {
                List<string> tab = new List<string>();

                string re = " drop table temp_runk select *  into  temp_runk from tts_runk2 "

               + " select case when aff.contact = 0 then 'Arret' else 'depart' end as Etat, "
    + " aff.imei,p.personnelid,vp.vehiculeid ,  convert(float ,aff.odometer - aff2.odometer )/10 as distance,  "
     + "  aff.date as date1 ,aff2.date as date2, aff2.adress as depart ,  "
    + "    aff.adress as arrive, DATEDIFF(minute,  aff2.date,aff.date)  "
    + "     from (select *, ROW_NUMBER() over(order by runk) as n from temp_runk rn) aff "
    + "      left outer join (select *, ROW_NUMBER() over(order by runk) as n from temp_runk rn )aff2  "
      + "    on aff2.imei = aff.imei and aff.n = aff2.n+1   inner join boitier b on b.imei=aff.imei  "
      + "     inner join affectation_vehicule_boitier avb on avb.boitierid=b.boitierid  "
       + "     inner join vehicule_personnel vp on vp.vehiculeid=avb.vehiculeid  "
       + "     inner join personnel p on p.personnelid=vp.personnelid  where aff.date > DATEADD (HOUR,-24,CURRENT_TIMESTAMP)  ";
                SqlDataReader dr = pp.Select(re);


                while (dr.Read())
                {
                    string t = dr[0].ToString();
                    string t1 = dr[1].ToString();
                    string t2 = dr[2].ToString();
                    string t3 = dr[3].ToString();
                    string t4 = dr[4].ToString();
                    string t5 = dr[5].ToString();
                    string t15 = dr[6].ToString();
                    string t25 = dr[7].ToString();
                    string t35 = dr[8].ToString();
                    string t035 = dr[9].ToString();

                    if (dr[0].ToString() == "depart" && dr[9] != null && dr[6].ToString() != "" && (double)dr[4] != null)
                    {
                        if ((int)dr[9] > 0)
                        {
                            double i = (60 / (int)dr[9]) * (double)dr[4];
                            tab.Add(""
                            + "(" + dr[2].ToString() + ",'" + dr[7].ToString() + "','" + dr[6].ToString() + "','" + dr[8].ToString() + "','" + dr[5].ToString() + "'," + dr[9].ToString() + "," + i.ToString(CultureInfo.InvariantCulture) + "," + ((double)dr[4]).ToString(CultureInfo.InvariantCulture) + "," + dr[3].ToString() + ")");

                        }
                    }
                }
                dr.Close();
                if (tab.Count > 0)
                {
                    string req = "insert into Historique_Trajet(conducteurid,adr_depart,date_depart,adr_fin,date_fin,duree,vitesse,distance,vehiculeid) values ";
                    for (int i = 0; i < tab.Count; i++)
                    {
                        req = req + tab[i];
                        if (i + 1 != tab.Count)
                        {
                            req = req + " , ";

                        }
                        // pp.Insert(tab[i]);
                    }

                    pp.Insert(req);
                }
            }
            catch (Exception ee) { }

        }


        public void histo_poi()
        {
            try
            {
                List<string> tab = new List<string>();

                string re = "select * from "
  +" (select p.poiid,per.personnelid,d.date as date_debut,DATEADD(minute,"
  +" d.duree,d.date) as date_fin,d.duree,d.vehiculeid,"
 +"  p.libelle as poi,p.type_poi ,"
 +"  p.longitude as lngP ,p.latitude as latP,d.latitude,d.longitude,d.localiisation,"
 +"     per.prenom+' '+per.nom as conducteur,p.tolerance,"
 +"      geography::STGeomFromText('POINT('+convert(varchar(20),p.longitude)+' '+convert(varchar(20),p.latitude)+')',4326).STDistance "
  +"      (geography::STGeomFromText('POINT('+convert(varchar(20),d.longitude)+' '+convert(varchar(20),d.latitude)+')',4326)) as distance "

 +" from Poi p  inner JOIN Historique_Arret  d "
   +"                    on p.actif=1 "
      +"                inner join personnel per on per.personnelid=d.personnelid"
    +"                  )aff where  aff.distance<aff.tolerance  and aff.date_debut > DATEADD (HOUR,-24,CURRENT_TIMESTAMP)"
    +"                  order by aff.date_debut ";
                SqlDataReader dr = pp.Select(re);


                while (dr.Read())
                {
                    string t = dr[0].ToString();
                    string t1 = dr[1].ToString();
                    string t2 = dr[2].ToString();
                    string t3 = dr[3].ToString();
                    string t4 = dr[4].ToString();
                    string t5 = dr[5].ToString();
                    string t15 = dr[6].ToString();
                    string t25 = dr[7].ToString();
                    string t35 = dr[8].ToString();

                    //   string t035 =dr[9].ToString().Replace(",", ".");
                    //   float aa = float.Parse(t035, System.Globalization.CultureInfo.InvariantCulture);
                    // float t0351 = float.Parse(dr[10].ToString().Replace(",", "."));

                    if (dr[0].ToString() != "" && dr[1] != null && dr[2].ToString() != "" && dr[3] != null && dr[4].ToString() != "" && dr[6].ToString() != "")
                    {


                        tab.Add("(" + dr[0].ToString() + "," + dr[1].ToString() + ",'" + dr[2].ToString() + "','" + dr[3].ToString() + "'," + dr[4].ToString() + "," + dr[5].ToString()+ ")");
                    }
                }
                dr.Close();
                if (tab.Count > 0)
                {
                    string req = "insert into Historique_Poi (poid,personelid,date_debut,date_fin,duree,vehiculeid)  values";
                    for (int i = 0; i < tab.Count; i++)
                    {
                        req = req + tab[i];
                        if (i == 995)
                        {
                            break;
                        }
                        if (i + 1 != tab.Count)
                        {
                            req = req + " , ";

                        }
                    }

                    pp.Insert(req);

                }
            }
            catch (Exception ee) { }


        }

    }
}