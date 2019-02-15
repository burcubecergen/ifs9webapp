using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
//This site developed by Vahit Kuruosman,Gökhan Biçer and Halil Bozkurt
namespace ifs9webapp.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        AccountController ac;
        OracleDataAdapter oda, oda_dropbox;
        OracleCommand cmd, new_cmd;
        DataSet ds;

        Ifs ifs;
        MaterialRequest mr;
        PurchaseRequest pr;
        QuotationOrderAndBlanked qo;
        ContractValuation cv;
        PurchaseOrder po;


        string[] words = null;
        private List<Ifs> setTableIfsToList(Ifs pIfs, DataTable pTable)
        {

            List<Ifs> ifsList = new List<Ifs>();

            for (int i = 0; i < pTable.Rows.Count; i++){
                ifsList.Add(new Ifs { Company = Convert.ToString(pTable.Rows[i]["COMPANY"]) ,Site = Convert.ToString(pTable.Rows[i]["SITE"]), BelgeTuru = Convert.ToString(pTable.Rows[i]["BELGE_TURU"])
                    ,BelgeNo = Convert.ToString(pTable.Rows[i]["BELGE_NO"]), IlgiliKisi = Convert.ToString(pTable.Rows[i]["ILGILI_KISI"]),
                   BelgeTarihi = Convert.ToString(pTable.Rows[i]["BELGE_TARIHI"]), Aut = Convert.ToString(pTable.Rows[i]["AUTHORIZE_GROUP_ID"])});
            }

            ViewBag.fill = ifsList.Count();
            return ifsList;
        }
        private List<MaterialRequest> setTableMatReqToList(MaterialRequest pMr, DataTable pTable)
        {
            List<MaterialRequest> mrList = new List<MaterialRequest>();
            //PART_NO,PART_DESCRIPTION,QTY_DUE,UNIT_MEAS

            for (int i = 0; i < pTable.Rows.Count; i++){
                mrList.Add(new MaterialRequest
                {
                    PartNo = Convert.ToString(pTable.Rows[i]["PART_NO"]), Description = Convert.ToString(pTable.Rows[i]["PART_DESCRIPTION"]),
                    QtyDue = Convert.ToInt32(pTable.Rows[i]["QTY_DUE"]), UnitMeas = Convert.ToString(pTable.Rows[i]["UNIT_MEAS"]),
                    OrderNo = Convert.ToString(pTable.Rows[i]["ORDER_NO"]),LineNo = Convert.ToString(pTable.Rows[i]["LINE_NO"]),
                    ReleaseNo = Convert.ToString(pTable.Rows[i]["RELEASE_NO"]),LineItemNo = Convert.ToInt32(pTable.Rows[i]["LINE_ITEM_NO"]),
                    OrderClassDb = Convert.ToString(pTable.Rows[i]["ORDER_CLASS_DB"]),Rule = Convert.ToString(pTable.Rows[i]["RULE"]),
                    Step = Convert.ToInt32(pTable.Rows[i]["STEP"]),ActivityAdi = Convert.ToString(pTable.Rows[i]["ACTIVITY_ADI"]),
                    Proje = Convert.ToString(pTable.Rows[i]["PROJE"]),NoteText = Convert.ToString(pTable.Rows[i]["NOTE_TEXT"]),
                    ObjId = Convert.ToString(pTable.Rows[i]["OBJID"]),ObjVersion = Convert.ToString(pTable.Rows[i]["OBJVERSION"]),
                });
                ViewBag.orderno = Convert.ToString(pTable.Rows[i]["ORDER_NO"]);
                ViewBag.allsuccess = Convert.ToString(pTable.Rows[0]["ALLSUCCESS"]);
            }
           
            
            ViewBag.fill = mrList.Count();

            return mrList;
        }
        
        private List<PurchaseOrder> setTablePurOrdToList(PurchaseOrder pPo,DataTable pTable)
        {
            List<PurchaseOrder> poList = new List<PurchaseOrder>();
            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                poList.Add(new PurchaseOrder
                {
                    PartNo = Convert.ToString(pTable.Rows[i]["PART_NO"]),Description = Convert.ToString(pTable.Rows[i]["DESCRIPTION"]),
                    BuyQtyDue = Convert.ToInt32(pTable.Rows[i]["BUY_QTY_DUE"]),BuyUnitMeas = Convert.ToString(pTable.Rows[i]["BUY_UNIT_MEAS"]),
                    BuyUnitPrice = Convert.ToInt32(pTable.Rows[i]["BUY_UNIT_PRICE"]),TotalPrice = Convert.ToInt32(pTable.Rows[i]["TOTAL_PRICE"]),
                    NoteText = Convert.ToString(pTable.Rows[i]["NOTE_TEXT"]),OrderNo = Convert.ToString(pTable.Rows[i]["ORDER_NO"]),
                    SequenceNo = Convert.ToInt32(pTable.Rows[i]["SEQUENCE_NO"])
                });
            }
            

            return poList;
        }
        private List<PurchaseRequest> setTablePurReqToList(PurchaseRequest pr,DataTable pTable)
        {
            List<PurchaseRequest> prList = new List<PurchaseRequest>();
            try { 
                for (int i = 0; i < pTable.Rows.Count; i++)
                 {
                    prList.Add(new PurchaseRequest
                    {
                        PartNo = Convert.ToString(pTable.Rows[i]["PART_NO"]),
                        PartDescription = Convert.ToString(pTable.Rows[i]["PART_DESCRIPTION"]),
                        Qty = Convert.ToInt32(pTable.Rows[i]["QTY"]),
                        UnitMeas = Convert.ToString(pTable.Rows[i]["UNIT_MEAS"]),
                        DestinationId = Convert.ToString(pTable.Rows[i]["DESTINATION_ID"])
                    });
                 }
            }
            catch(Exception e){
                Console.WriteLine("error purchasereq");
            }
            

            return prList;
        }
        private int  returnCount(List<string> pFindNullCount)
        {
            int count = 0;
            for(int i = 0; i<6; i++) {
                if (pFindNullCount[i] != "")
                count++;
            }

            return count;
        }
        private List<QuotationOrderAndBlanked> setTableQuotToList(QuotationOrderAndBlanked pQu,DataTable pTable)
        {
            List<QuotationOrderAndBlanked> poList = new List<QuotationOrderAndBlanked>();
            List<String> findNullCount = new List<String>();
            if (pTable.Columns.Count > 3){ 
                for (int i = 0; i < pTable.Rows.Count; i++){
                    try { 
                        poList.Add(new QuotationOrderAndBlanked
                        {
                            PartNo       =  Convert.ToString(pTable.Rows[i]["PART_NO"]),
                            Quantity     =  Convert.ToString(pTable.Rows[i]["QUANTITY"]),
                            UnitMeas     =  Convert.ToString(pTable.Rows[i]["UNIT_MEAS"]),
                            CompanyOne   =  Convert.ToString(pTable.Rows[i]["FIRMA1"]),
                            CompanyTwo   =  Convert.ToString(pTable.Rows[i]["FIRMA2"]),
                            CompanyThree =  Convert.ToString(pTable.Rows[i]["FIRMA3"]),
                            CompanyFour  =  Convert.ToString(pTable.Rows[i]["FIRMA4"]),
                            CompanyFive  =  Convert.ToString(pTable.Rows[i]["FIRMA5"]),
                            CompanySix   =  Convert.ToString(pTable.Rows[i]["FIRMA6"]),
                            CompanySeven =  Convert.ToString(pTable.Rows[i]["FIRMA7"]),
                            CompanyEight =  Convert.ToString(pTable.Rows[i]["FIRMA8"])

                        });
                        if(i == 0){
                            for(int k = 1; k<7; k++)
                             findNullCount.Add(Convert.ToString(pTable.Rows[i]["FIRMA"+k]));
                            
                             ViewBag.notnullcount = returnCount(findNullCount);
                        }
                    }
                    catch(Exception e){
                        Console.WriteLine("error quotation");
                    }
                }
            }
            else
            {
                poList.Clear();
                for (int i = 0; i < pTable.Rows.Count; i++){
                    try{
                        
                        poList.Add(new QuotationOrderAndBlanked
                        {
                            VendorNo = Convert.ToString(pTable.Rows[i]["VENDOR_NO"]),
                            SuppName = Convert.ToString(pTable.Rows[i]["SUPPNAME"]),
                           
                        });
                    }
                    catch (Exception e){
                        Console.WriteLine("error");
                    }
                }
            }


            return poList;
        }
        
        private List<ContractValuation> setTableContToList(ContractValuation cv,DataTable pTable)
        {
            List<ContractValuation> cvList = new List<ContractValuation>();
            for(int i=0; i < pTable.Rows.Count; i++){
                cvList.Add(new ContractValuation
                {
                    Aciklama = Convert.ToString(pTable.Rows[i]["ACIKLAMA"]),
                    Deger = Convert.ToString(pTable.Rows[i]["Deger"]),
                });
            }
            ViewBag.stepNoandLineNo = cvList[cvList.Count - 1].Deger;
            cvList.RemoveAt(cvList.Count - 1);//last item delete 

            return cvList;
        }
        [HttpGet]
        public IActionResult Ifs()
        {
            ac = new AccountController();
            ifs = new Ifs();
            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            string password = HttpContext.Session.GetString("kullaniciParola");

            ViewBag.loginname = loginName;
            string query = @"select IFSAPP.site_api.Get_Company(SITE) company, SITE, BELGE_TURU, BELGE_NO, ILGILI_KISI, BELGE_TARIHI, nvl(g.authorize_id, t.AUT) AUT,
            g.authorize_group_id from IFSAPP.CCN_PENDING_AUTH_FORWEB_REP t left join IFSAPP.PURCH_AUTHORIZE_GROUP_LINE g on IFSAPP.site_api.Get_Company(t.site)=g.company and t.aut=g.authorize_group_id
            where nvl(g.authorize_id, t.AUT)='" + loginName + "'";
     
            var rCon = ac.openConnection(ac.oracleConnection(loginName, password));
            oda = new OracleDataAdapter(query,rCon);
            ds = new DataSet();
            oda.Fill(ds);
            
            DataTable table = ds.Tables[0];

            ViewBag.list = setTableIfsToList(ifs, table);
            ac.closeConnection(rCon);
            return View();
        }

        
        [HttpGet]
        public IActionResult MaterialRequest(RequestViewModel rvm,bool error)
        {

            ac = new AccountController();
            mr = new MaterialRequest();

            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            string password = HttpContext.Session.GetString("kullaniciParola");
            string id = Request.Query["id"].ToString();
            ViewBag.loginname = loginName;

            if (rvm.RedirectToIfs)
            return RedirectToAction("Ifs", "Home",new { loginname = loginName });
            
            string query = @"select mrl.PART_NO, pp.description PART_DESCRIPTION, mrl.qty_due, mrl.unit_meas,
            mrl.order_no, mrl.line_no, mrl.release_no, mrl.line_item_no, mrl.order_class_db, p.rule, p.step,
            mrl.activity_seq || '-' || IFSAPP.activity_api.Get_Description(mrl.activity_seq) activity_adi,mrl.contract || '-' ||
            IFSAPP.activity_api.Get_Sub_Project_Id(mrl.activity_seq) || '-' || IFSAPP.activity_api.Get_Sub_Project_Description(mrl.activity_seq) proje,mrl.note_text,
            p.objid,p.objversion, listagg(to_char(aa.release_no) || '/') within group(order by aa.order_no) || listagg(to_char(aa.step) || '-') within group(order by aa.order_no) || 
            listagg(to_char(aa.line_no) || '?') within group(order by aa.order_no) as allsuccess  from IFSAPP.material_requis_line mrl
            inner join IFSAPP.ccn_mat_req_conf_path p on mrl.order_no=p.order_no and mrl.line_no=p.line_no and mrl.release_no=p.release_no and mrl.line_item_no=p.line_item_no
            left join IFSAPP.PURCH_AUTHORIZE_GROUP_LINE g on p.authorize_group_id=g.authorize_group_id and IFSAPP.site_api.Get_Company(mrl.contract)=g.company
            inner join IFSAPP.purchase_part pp on mrl.contract=pp.contract and mrl.part_no=pp.part_no
            inner join IFSAPP.Ccn_Mat_Req_Conf_Path_Ext aa on aa.order_no = mrl.order_no  and aa.authorize_group_id = p.authorize_group_id
            where 
            p.step = (SELECT MIN(C.STEP)
                        FROM IFSAPP.Ccn_Mat_Req_Conf_Path_Ext C
                       WHERE C.ORDER_NO = p.ORDER_NO
                         AND C.LINE_NO = p.LINE_NO
                         AND C.RELEASE_NO = p.RELEASE_NO
                         AND C.LINE_ITEM_NO = p.LINE_ITEM_NO
                         AND C.approver IS NULL
                         AND C.rejected_by IS NULL)
           AND (mrl.CONFIRMATION_STATE LIKE 'K%' OR mrl.CONFIRMATION_STATE = 'Onaylanacak')
           AND mrl.order_no='" + id + "' and nvl(g.authorize_id,p.authorize_id)='" +loginName + "'"+ "GROUP BY mrl.PART_NO,"+
           "pp.description, mrl.qty_due,mrl.unit_meas,mrl.order_no,mrl.line_no,mrl.release_no,mrl.line_item_no,mrl.order_class_db,"+
           "p.rule,p.step,mrl.activity_seq || '-' ||IFSAPP.activity_api.Get_Description(mrl.activity_seq) , mrl.contract || '-' ||"+
           "IFSAPP.activity_api.Get_Sub_Project_Id(mrl.activity_seq) || '-' ||IFSAPP.activity_api.Get_Sub_Project_Description(mrl.activity_seq),"+
           "mrl.note_text,p.objid,p.objversion";
           
            

            var rCon = ac.openConnection(ac.oracleConnection(loginName, password));
            oda = new OracleDataAdapter(query, rCon);
            ds = new DataSet();
            oda.Fill(ds);
            DataTable table = ds.Tables[0];
            ViewBag.list = setTableMatReqToList(mr, table);

            ac.closeConnection(rCon);
            return View();
        }
        [HttpPost]
        public IActionResult MaterialRequest(string orderno,string lineno,bool success,
        string releaseno,int lineitemno,string orderclassdb,string rule,int step,
        int quantity,string objid,string objversion,int qtydue,string rednote,int listcount,
        string allsuccess,bool allapprove)
        {
            ac = new AccountController();

            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            string password = HttpContext.Session.GetString("kullaniciParola");

            bool check_error = quantity > qtydue ? check_error = true : check_error = false;
            if (allapprove)
            {
                string[] rparam = allsuccess.Split('/');
                string[] sparam = allsuccess.Split('-');
                string[] lparam = allsuccess.Split('?');
            }
            if (!check_error)
            {
                string OraApi = !allapprove ? success ? quantity == 0 ? @"DECLARE
                               p0_ VARCHAR2(32000) := '" + orderno + "';" +
                               "p1_ VARCHAR2(32000) := '" + lineno + "';" +
                               "p2_ VARCHAR2(32000) := '" + releaseno + "';" +
                               "p3_ FLOAT := " + lineitemno + ";" +
                               "p4_ VARCHAR2(32000) := '" + orderclassdb + "';" +
                               "p5_ VARCHAR2(32000) := '" + rule + "';" +
                               "p6_ FLOAT := " + step + ";" +
                               "p7_ FLOAT := NULL;" +
                            "BEGIN " +
                            "IFSAPP.Ccn_Mat_Req_Conf_Path_Api.Approve( p0_ ,  p1_ ,  p2_ ,  p3_ ,  p4_ ,  p5_ ,  p6_ ,  p7_ ); " +
                            "commit; " +
                            "END;" : @"DECLARE
                                p0_ VARCHAR2(32000) := '';" +
                               "p1_ VARCHAR2(32000) := '" + objid + "';" +
                               "p2_ VARCHAR2(32000) := '" + objversion + "';" +
                               "p3_ VARCHAR2(32000) := 'APPROVED_QTY'||chr(31)||'" + quantity + "'||chr(30);" +
                               "p4_ VARCHAR2(32000) := 'DO';" +
                               "p5_ VARCHAR2(32000) := '" + orderno + "';" +
                               "p6_ VARCHAR2(32000) := '" + lineno + "';" +
                               "p7_ VARCHAR2(32000) := '" + releaseno + "';" +
                               "p8_ FLOAT := " + lineitemno + ";" +
                               "p9_ VARCHAR2(32000) := '" + orderclassdb + "';" +
                               "p10_ VARCHAR2(32000) := '" + rule + "';" +
                               "p11_ FLOAT := " + step + ";" +
                               "p12_ FLOAT := " + quantity + ";" +
                            "BEGIN " +
                            "IFSAPP.Ccn_Mat_Req_Conf_Path_Api.MODIFY__( p0_ , p1_ , p2_ , p3_ , p4_ ); " +
                            "IFSAPP.Ccn_Mat_Req_Conf_Path_Api.Approve( p5_ ,  p6_ ,  p7_ ,  p8_ ,  p9_ ,  p10_ ,  p11_ ,  p12_ ); " +
                            "commit; " +
                            "END;" : @"DECLARE
                                p0_ VARCHAR2(32000) := '';" +
                               "p1_ VARCHAR2(32000) := '" + objid + "';" +
                               "p2_ VARCHAR2(32000) := '" + objversion + "';" +
                               "p3_ VARCHAR2(32000) := 'REJECT_NOTE'||chr(31)||'" + rednote + "'||chr(30);" +
                               "p4_ VARCHAR2(32000) := 'DO';" +
                               "p5_ VARCHAR2(32000) := '" + orderno + "';" +
                               "p6_ VARCHAR2(32000) := '" + lineno + "';" +
                               "p7_ VARCHAR2(32000) := '" + releaseno + "';" +
                               "p8_ FLOAT := " + lineitemno + ";" +
                               "p9_ VARCHAR2(32000) := '" + orderclassdb + "';" +
                               "p10_ VARCHAR2(32000) := '" + rule + "';" +
                               "p11_ FLOAT := " + step + ";" +
                               "p12_ VARCHAR2(32000)  := '" + rednote + "';" +
                            "BEGIN " +
                            "IFSAPP.Ccn_Mat_Req_Conf_Path_Api.MODIFY__( p0_ , p1_ , p2_ , p3_ , p4_ ); " +
                            "IFSAPP.Ccn_Mat_Req_Conf_Path_Api.Reject( p5_ ,  p6_ ,  p7_ ,  p8_ ,  p9_ ,  p10_ ,  p11_ ,  p12_ ); " +
                            "commit; " +
                            "END;":@"DECLARE
                               p0_ VARCHAR2(32000) := '" + orderno + "';" +
                               "p1_ VARCHAR2(32000) := '" + lineno + "';" +
                               "p2_ VARCHAR2(32000) := '" + releaseno + "';" +
                               "p3_ FLOAT := " + lineitemno + ";" +
                               "p4_ VARCHAR2(32000) := '" + orderclassdb + "';" +
                               "p5_ VARCHAR2(32000) := '" + rule + "';" +
                               "p6_ FLOAT := " + step + ";" +
                               "p7_ FLOAT := NULL;" +
                            "BEGIN " +
                            "IFSAPP.Ccn_Mat_Req_Conf_Path_Api.Approve( p0_ ,  p1_ ,  p2_ ,  p3_ ,  p4_ ,  p5_ ,  p6_ ,  p7_ ); " +
                            "commit; " +
                            "END;";

                var rCon = ac.openConnection(ac.oracleConnection(loginName, password));
                cmd = rCon.CreateCommand();
                cmd.CommandText = OraApi;

                if (allapprove) {

                    cmd.ExecuteNonQuery();
                }
                return Content("success");
            }
            else
            return Content("error");
        }
        [HttpGet]
        public IActionResult PurchaseOrder(RequestViewModel rvm)
        {

            ac = new AccountController();
            po = new PurchaseOrder();

            var loginName = HttpContext.Session.GetString("kullaniciAdi");
            var password = HttpContext.Session.GetString("kullaniciParola");

            string id = Request.Query["id"].ToString();
            string company = Request.Query["company"].ToString();

            ViewBag.loginname = loginName;
            if (rvm.RedirectToIfs)
            return RedirectToAction("Ifs", "Home",new { loginname = loginName });//IIS log controli icin

        
            ViewBag.id = id;
            ViewBag.company = company;

            string query = @"select
            a.order_no,a.approver_sign,a.sequence_no,cc.part_no,cc.description,cc.buy_qty_due,cc.buy_unit_meas,cc.buy_unit_price,cc.buy_qty_due* cc.buy_unit_price total_price, cc.note_text
            from ifsapp.PURCHASE_ORDER t
            left join ifsapp.PURCHASE_ORDER_APPROVAL a on a.order_no = t.order_no
            left join ifsapp.PURCHASE_ORDER_LINE_PART cc on cc.order_no = t.order_no
            where t.objstate = 'Planned' and a.approver_sign is null
            and a.sequence_no = (select min(bb.sequence_no) from ifsapp.PURCHASE_ORDER_APPROVAL bb where bb.order_no = a.order_no and bb.approver_sign is null)
            and cc.order_no =" + id;


            var rCon = ac.openConnection(ac.oracleConnection(loginName, password));
            oda = new OracleDataAdapter(query, rCon);
            ds = new DataSet();
            oda.Fill(ds);
            DataTable table = ds.Tables[0];
            ViewBag.list = setTablePurOrdToList(po, table);
            ac.closeConnection(rCon);
            return View();
        }
        [HttpPost]
        public IActionResult PurchaseOrder(string id , string company,int sequenceno)
        {
            ac = new AccountController();
            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            string password = HttpContext.Session.GetString("kullaniciParola");

            var rCon = ac.openConnection(ac.oracleConnection(loginName, password));
            cmd = rCon.CreateCommand();
           
            string OraApi = @"DECLARE" +
            " p0_ VARCHAR2(32000) := '" + id + "';" +
            " p1_ VARCHAR2(32000) := '*';" +
            " p2_ FLOAT := " + sequenceno + ";" +
            " p3_ VARCHAR2(32000) := '" + company + "';" +
            " p4_ VARCHAR2(32000) := '" + loginName + "';" +
            " p5_ VARCHAR2(32000) := '';" +
            " p6_ VARCHAR2(32000) := '';" +
            " p7_ VARCHAR2(32000) := '';" +
            " p8_ VARCHAR2(32000) := '';" +
            " BEGIN" +
            " IFSAPP.Purchase_Order_Approval_API.Authorize(p0_, p1_, p2_, p3_, p4_, p5_, p6_, p7_, p8_);" +
            " commit;" +
            " END;";
            cmd.CommandText = OraApi;
           
            cmd.ExecuteNonQuery();
            ac.closeConnection(rCon);
            return Content("success");
        }
        [HttpGet]
        public IActionResult PurchaseRequest(RequestViewModel rvm)
        {
            ac = new AccountController();
            pr = new PurchaseRequest();

            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            string password = HttpContext.Session.GetString("kullaniciParola");

            string id = Request.Query["id"].ToString();
            string company = Request.Query["company"].ToString();
            string autgid = Request.Query["autgid"].ToString();

            ViewBag.loginname = loginName;
            if (rvm.RedirectToIfs)
            return RedirectToAction("Ifs", "Home",new { loginname = loginName });

            ViewBag.id = id;
            ViewBag.company = company;
            ViewBag.autgid = autgid;

            string query = @"SELECT
            a.part_no, a.description part_description, a.original_qty qty, a.buy_unit_meas unit_meas,d.destination_id destination_id
            FROM IFSAPP.PURCH_REQ_LINE_APPROVAL B
            INNER JOIN IFSAPP.PURCHASE_REQ_LINE_all A ON B.REQUISITION_NO = A.REQUISITION_NO AND B.LINE_NO = A.LINE_NO AND B.RELEASE_NO = A.RELEASE_NO
            INNER JOIN IFSAPP.purchase_requisition d ON a.requisition_no = d.requisition_no
            left join IFSAPP.PURCH_AUTHORIZE_GROUP_LINE g on B.authorize_group_id = g.authorize_group_id and IFSAPP.site_api.Get_Company(A.contract) = g.company
            WHERE
            A.OBJSTATE IN('Released', 'Partially Authorized')
            AND B.SEQUENCE_NO = (SELECT MIN(C.SEQUENCE_NO)
                          FROM IFSAPP.PURCH_REQ_LINE_APPROVAL C
                         WHERE C.REQUISITION_NO = B.REQUISITION_NO
                           AND C.LINE_NO = B.LINE_NO
                           AND C.RELEASE_NO = B.RELEASE_NO
                           AND C.DATE_APPROVED IS NULL)
            AND A.requisition_no='" + id + "' AND nvl(g.authorize_id, B.authorize_id) = '" + loginName+ "'";

            var rCon = ac.openConnection(ac.oracleConnection(loginName, password));
            oda = new OracleDataAdapter(query, rCon);
            ds = new DataSet();
            oda.Fill(ds);
            DataTable table = ds.Tables[0];
            ViewBag.list = setTablePurReqToList(pr,table);

            ac.closeConnection(rCon);
            return View();
        }
        [HttpPost]
        public IActionResult PurchaseRequest(string id,string company,string autgid)
        {
            ac = new AccountController();
            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            
            string password = HttpContext.Session.GetString("kullaniciParola");

            var rCon = ac.openConnection(ac.oracleConnection(loginName, password));
            cmd = rCon.CreateCommand();


           string OraApi = @"DECLARE
           p3_ VARCHAR2(32000) := '" + id + "';" +
           "p4_ VARCHAR2(32000) := '';" +
           "p5_ FLOAT := 1;" +
           "p6_ VARCHAR2(32000) := '" + company+ "';" +
           "p7_ VARCHAR2(32000) := '" + (autgid == null ? loginName : "") + "';" +
           "p8_ VARCHAR2(32000) := '"+ autgid + "';" +
           "p9_ VARCHAR2(32000) := '';" +
           "p10_ VARCHAR2(32000) := '';" +
           "p11_ VARCHAR2(32000) := '';" +
           "p12_ VARCHAR2(32000) := '';" +
            "BEGIN " +
            "   IFSAPP.Purch_Req_Approval_API.Authorize__( p4_ , p3_ , p5_ , p6_ , p7_ , p8_ , p9_ , p10_ , p11_ , p12_ );" +
            "commit;" +
            "END;";

            cmd.CommandText = OraApi;

            cmd.ExecuteNonQuery();

            ac.closeConnection(rCon);
            return Content("success");
        }
        [HttpGet]
        public IActionResult QuotationOrder(RequestViewModel rvm)
        {
            ac = new AccountController();
            qo = new QuotationOrderAndBlanked();

            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            string password = HttpContext.Session.GetString("kullaniciParola");

            string p0 = Request.Query["id"].ToString();
            ViewBag.Id = p0;

            string p1 = p0.Substring(0, p0.IndexOf(" "));
            string p2 = p0.Substring(p0.IndexOf("Rev.") + 4, p0.Length - p0.IndexOf("Rev.") - 5);

            ViewBag.loginname = loginName;
            if (rvm.RedirectToIfs)
            return RedirectToAction("Ifs", "Home",new { loginname = loginName });

        

            string query = @"select 
            '' part_no, '' quantity, '' unit_meas, '' Firma1, '' Firma2, '' Firma3, '' Firma4, '' Firma5, '' Firma6, '' Firma7, '' Firma8 from dual where 1=0
            union all
            select * from 
            (
              select i.sira, '' part_no, '' quantity, '' unit_meas, i.suppname from 
              (
                select insupp.sira, insupp.inquiry_no, insupp.vendor_no, insupp.vendor_no||'-'||IFSAPP.supplier_info_api.get_name(insupp.vendor_no) suppname, inlp.line_no, inlp.quantity, inlp.buy_unit_meas unit_meas, inlp.part_no, inlp.description from 
                (SELECT rownum sira, inquiry_no, vendor_no FROM IFSAPP.INQUIRY_ORD_SUPPLIER t WHERE inquiry_no=@p1) insupp
                inner join IFSAPP.INQUIRY_LINE_PART_ORDER inlp on insupp.inquiry_no=inlp.inquiry_no
                inner join IFSAPP.INQUIRY_ORDER io on insupp.inquiry_no=io.inquiry_no
              ) i
              inner join IFSAPP.QUOTATION_LINE_PART_ORD_CON qlp on i.inquiry_no=qlp.inquiry_no and i.line_no=qlp.line_no and i.vendor_no=qlp.vendor_no
              where qlp.revision_no = @p2
            )
            pivot (min(suppname) for sira in (1,2,3,4,5,6,7,8))
            union all
            select * from 
            (
              select i.sira, i.part_no||'-'||i.description part_no, to_char(i.quantity), i.unit_meas, 'BF:' || trim(to_char(qlp.price,'999,999,999.99')) ||' -> ' || trim(to_char((i.quantity*qlp.price),'999,999,999,999.99'))  || qo.currency_code tutar from 
              (
                select insupp.sira, insupp.inquiry_no, insupp.vendor_no, inlp.line_no, inlp.quantity, inlp.buy_unit_meas unit_meas, inlp.part_no, inlp.description from 
                (SELECT rownum sira, inquiry_no, vendor_no FROM IFSAPP.INQUIRY_ORD_SUPPLIER t WHERE inquiry_no=@p1) insupp
                inner join IFSAPP.INQUIRY_LINE_PART_ORDER inlp on insupp.inquiry_no=inlp.inquiry_no
                inner join IFSAPP.INQUIRY_ORDER io on insupp.inquiry_no=io.inquiry_no
              ) i
              inner join IFSAPP.QUOTATION_LINE_PART_ORD_CON qlp on i.inquiry_no=qlp.inquiry_no and i.line_no=qlp.line_no and i.vendor_no=qlp.vendor_no
              inner join IFSAPP.QUOTATION_ORDER qo on qlp.inquiry_no=qo.inquiry_no and qlp.vendor_no=qo.vendor_no and qlp.revision_no=qo.revision_no
              where qlp.revision_no = @p2
            )
            pivot (min(tutar)    for sira in (1,2,3,4,5,6,7,8))
            union all
            select * from 
            (
              select i.sira, '' part_no, '', '' unit_meas, trim(to_char(sum(i.quantity*qlp.price),'999,999,999,999.99')) || qo.currency_code tutar from 
              (
                select insupp.sira, insupp.inquiry_no, insupp.vendor_no, inlp.line_no, inlp.quantity, inlp.buy_unit_meas unit_meas, inlp.part_no, inlp.description from 
                (SELECT rownum sira, inquiry_no, vendor_no FROM IFSAPP.INQUIRY_ORD_SUPPLIER t WHERE inquiry_no=@p1) insupp
                inner join IFSAPP.INQUIRY_LINE_PART_ORDER inlp on insupp.inquiry_no=inlp.inquiry_no
                inner join IFSAPP.INQUIRY_ORDER io on insupp.inquiry_no=io.inquiry_no
              ) i
              inner join IFSAPP.QUOTATION_LINE_PART_ORD_CON qlp on i.inquiry_no=qlp.inquiry_no and i.line_no=qlp.line_no and i.vendor_no=qlp.vendor_no
              inner join IFSAPP.QUOTATION_ORDER qo on qlp.inquiry_no=qo.inquiry_no and qlp.vendor_no=qo.vendor_no and qlp.revision_no=qo.revision_no
              where qlp.revision_no = @p2
              group by i.sira, qo.currency_code
            )
            pivot (min(tutar)    for sira in (1,2,3,4,5,6,7,8))
            union all
            select * from 
            (
              select i.sira, '' part_no, '' quantity, '' unit_meas, qo.c_note_text from 
              (
                select insupp.sira, insupp.inquiry_no, insupp.vendor_no, insupp.vendor_no||'-'||IFSAPP.supplier_info_api.get_name(insupp.vendor_no) suppname, inlp.line_no, inlp.quantity, inlp.buy_unit_meas unit_meas, inlp.part_no, inlp.description from 
                (SELECT rownum sira, inquiry_no, vendor_no FROM IFSAPP.INQUIRY_ORD_SUPPLIER t WHERE inquiry_no=@p1) insupp
                inner join IFSAPP.INQUIRY_LINE_PART_ORDER inlp on insupp.inquiry_no=inlp.inquiry_no 
                inner join IFSAPP.INQUIRY_ORDER io on insupp.inquiry_no=io.inquiry_no
              ) i
              inner join IFSAPP.QUOTATION_LINE_PART_ORD_CON qlp on i.inquiry_no=qlp.inquiry_no and i.line_no=qlp.line_no and i.vendor_no=qlp.vendor_no
              inner join IFSAPP.QUOTATION_ORDER qo on qlp.inquiry_no=qo.inquiry_no and qlp.vendor_no=qo.vendor_no and qlp.revision_no=qo.revision_no
              where qlp.revision_no = @p2
            )
            pivot (min(c_note_text) for sira in (1,2,3,4,5,6,7,8))";


            query = query.Replace("@p1", p1);
            query = query.Replace("@p2", p2);


            var rCon = ac.openConnection(ac.oracleConnection(loginName, password));
            oda = new OracleDataAdapter(query, rCon);
            ds = new DataSet();
            oda.Fill(ds);
            DataTable table = ds.Tables[0];
            ViewBag.list = setTableQuotToList(qo,table);

            string dropbox_query = @"SELECT vendor_no, IFSAPP.supplier_info_api.get_name(vendor_no) suppname FROM IFSAPP.INQUIRY_ORD_SUPPLIER t WHERE inquiry_no=" + p1;
            oda_dropbox = new OracleDataAdapter(dropbox_query, rCon);
            DataTable companies = new DataTable();
            oda_dropbox.Fill(companies);
            ViewBag.dropdownList = setTableQuotToList(qo, companies);
            ac.closeConnection(rCon);

            return View();
        }
        [HttpPost]
        public IActionResult QuotationOrder(string id,string selected,bool success)
        {
            ac = new AccountController();

        
            string new_id = id.Substring(0, id.IndexOf(" "));
            string new_rev = id.Substring(id.IndexOf("Rev.") + 4, id.Length - id.IndexOf("Rev.") - 5);

            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            string password = HttpContext.Session.GetString("kullaniciParola");

            var rCon = ac.openConnection(ac.oracleConnection(loginName, password));

            cmd = rCon.CreateCommand();

            string sSqlLineNo =  @"select nvl(min(line_no), 0) from IFSAPP.Ccn_Quotation_Auth a left join IFSAPP.PURCH_AUTHORIZE_GROUP_LINE b on a.authorize_group_id=b.authorize_group_id and "+
            "IFSAPP.inquiry_api.get_company(a.inquiry_no)=b.company where inquiry_no=" + new_id + " and vendor_no='" + selected + "' and revision_no=" + new_rev + " and nvl(b.authorize_id, a.authorize_id)='" + loginName + "'";

            cmd.CommandText = sSqlLineNo;

            string execute_scalar = cmd.ExecuteScalar().ToString();

            string OraApi = success ?  @"DECLARE" +
            " p0_ FLOAT := " + new_id + ";" +
            " p1_ VARCHAR2(32000) := '" + selected + "';" +
            " p2_ FLOAT := " + new_rev + ";" +
            " p3_ FLOAT := " + execute_scalar + ";" +
            " BEGIN" +
            " IFSAPP.ccn_quotation_auth_api.Authorize(p0_, p1_, p2_, p3_ );" +
            " commit;" +
            " END;": @"DECLARE" +
            " p0_ FLOAT := " + new_id + ";" +
            " p1_ VARCHAR2(32000) := '" + selected + "';" +
            " p2_ FLOAT := " + new_rev + ";" +
            " p3_ FLOAT := " + execute_scalar + ";" +
            " BEGIN" +
            " IFSAPP.ccn_quotation_auth_api.Revise(p0_, p1_, p2_, p3_ );" +
            " commit;" +
            " END;";

            new_cmd = rCon.CreateCommand();
            new_cmd.CommandText = OraApi;
            new_cmd.ExecuteNonQuery();

            ac.closeConnection(rCon);
            return Content("success");
        }
        [HttpGet]
        public IActionResult QuotationBlanket(RequestViewModel rvm)
        {
            ac = new AccountController();
            qo = new QuotationOrderAndBlanked();

            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            string password = HttpContext.Session.GetString("kullaniciParola");

            string p0 = Request.Query["id"].ToString();
            ViewBag.Id = p0;
            string p1 = p0.Substring(0, p0.IndexOf(" "));
            string p2 = p0.Substring(p0.IndexOf(".")+1 ,(p0.Length-p0.IndexOf("."))- (p0.Length-p0.IndexOf(")")+1));

            ViewBag.loginname = loginName;
            if (rvm.RedirectToIfs)
            return RedirectToAction("Ifs", "Home", new { loginname = loginName });


            string query = @" select
            '' part_no, '' quantity, '' unit_meas, '' Firma1, '' Firma2, '' Firma3, '' Firma4, '' Firma5, '' Firma6, '' Firma7, '' Firma8 from dual where 1 = 0
            union all
            select* from
            (
              select i.sira, '' part_no, '' quantity, '' unit_meas, i.suppname from
              (
                select insupp.sira, insupp.inquiry_no, insupp.vendor_no, insupp.vendor_no|| '-' || IFSAPP.supplier_info_api.get_name(insupp.vendor_no) suppname, inlp.line_no, inlp.quantity, inlp.buy_unit_meas unit_meas, inlp.part_no, inlp.description from
                   (SELECT rownum sira, inquiry_no, vendor_no FROM IFSAPP.INQUIRY_ORD_SUPPLIER t WHERE inquiry_no = @p1) insupp
                     inner join IFSAPP.INQUIRY_LINE_PART_BLK inlp on insupp.inquiry_no = inlp.inquiry_no
                inner join IFSAPP.INQUIRY_BLANKET io on insupp.inquiry_no = io.inquiry_no
              ) i
              inner join IFSAPP.QUOTATION_LINE_PART_BLK_CON qlp on i.inquiry_no = qlp.inquiry_no and i.line_no = qlp.line_no and i.vendor_no = qlp.vendor_no
              where qlp.revision_no = @p2
            )
            pivot(min(suppname) for sira in (1, 2, 3, 4, 5, 6, 7, 8))
                union all
            select* from
            (
              select i.sira, i.part_no|| '-' || i.description part_no, to_char(i.quantity), i.unit_meas, 'BF:' || trim(to_char(qlp.price, '999,999,999.99')) || ' -> ' || trim(to_char((i.quantity * qlp.price), '999,999,999,999.99')) || qo.currency_code tutar from
                     (
                       select insupp.sira, insupp.inquiry_no, insupp.vendor_no, inlp.line_no, inlp.quantity, inlp.buy_unit_meas unit_meas, inlp.part_no, inlp.description from
       
                       (SELECT rownum sira, inquiry_no, vendor_no FROM IFSAPP.INQUIRY_ORD_SUPPLIER t WHERE inquiry_no = @p1) insupp
                         inner join IFSAPP.INQUIRY_LINE_PART_BLK inlp on insupp.inquiry_no = inlp.inquiry_no
                inner join IFSAPP.INQUIRY_BLANKET io on insupp.inquiry_no = io.inquiry_no
              ) i
              inner join IFSAPP.QUOTATION_LINE_PART_BLK_CON qlp on i.inquiry_no = qlp.inquiry_no and i.line_no = qlp.line_no and i.vendor_no = qlp.vendor_no
              inner join IFSAPP.QUOTATION_BLANKET qo on qlp.inquiry_no = qo.inquiry_no and qlp.vendor_no = qo.vendor_no and qlp.revision_no = qo.revision_no
              where qlp.revision_no = @p2
            )
            pivot(min(tutar)    for sira in (1, 2, 3, 4, 5, 6, 7, 8))
                union all
            select* from
            (
              select i.sira, '' part_no, '', '' unit_meas, trim(to_char(sum(i.quantity * qlp.price), '999,999,999,999.99')) || qo.currency_code tutar from
                 (
                   select insupp.sira, insupp.inquiry_no, insupp.vendor_no, inlp.line_no, inlp.quantity, inlp.buy_unit_meas unit_meas, inlp.part_no, inlp.description from
   
                   (SELECT rownum sira, inquiry_no, vendor_no FROM IFSAPP.INQUIRY_ORD_SUPPLIER t WHERE inquiry_no = @p1) insupp
                    inner join IFSAPP.INQUIRY_LINE_PART_BLK inlp on insupp.inquiry_no = inlp.inquiry_no
                inner join IFSAPP.INQUIRY_BLANKET io on insupp.inquiry_no = io.inquiry_no
              ) i
              inner join IFSAPP.QUOTATION_LINE_PART_BLK_CON qlp on i.inquiry_no = qlp.inquiry_no and i.line_no = qlp.line_no and i.vendor_no = qlp.vendor_no
              inner join IFSAPP.QUOTATION_BLANKET qo on qlp.inquiry_no = qo.inquiry_no and qlp.vendor_no = qo.vendor_no and qlp.revision_no = qo.revision_no
              where qlp.revision_no = @p2
              group by i.sira, qo.currency_code
            )
            pivot(min(tutar)    for sira in (1, 2, 3, 4, 5, 6, 7, 8))
                union all
            select* from
            (
              select i.sira, '' part_no, '' quantity, '' unit_meas, qo.c_note_text from
              (
                select insupp.sira, insupp.inquiry_no, insupp.vendor_no, insupp.vendor_no|| '-' || IFSAPP.supplier_info_api.get_name(insupp.vendor_no) suppname, inlp.line_no, inlp.quantity, inlp.buy_unit_meas unit_meas, inlp.part_no, inlp.description from
                   (SELECT rownum sira, inquiry_no, vendor_no FROM IFSAPP.INQUIRY_ORD_SUPPLIER t WHERE inquiry_no = @p1)insupp
                    inner join IFSAPP.INQUIRY_LINE_PART_BLK inlp on insupp.inquiry_no = inlp.inquiry_no
                inner join IFSAPP.INQUIRY_BLANKET io on insupp.inquiry_no = io.inquiry_no
              ) i
              inner join IFSAPP.QUOTATION_LINE_PART_BLK_CON qlp on i.inquiry_no = qlp.inquiry_no and i.line_no = qlp.line_no and i.vendor_no = qlp.vendor_no
              inner join IFSAPP.QUOTATION_BLANKET qo on qlp.inquiry_no = qo.inquiry_no and qlp.vendor_no = qo.vendor_no and qlp.revision_no = qo.revision_no
              where qlp.revision_no = @p2
            )
            pivot(min(c_note_text) for sira in (1, 2, 3, 4, 5, 6, 7, 8))";

            
            query = query.Replace("@p1", p1);
            query = query.Replace("@p2", p2);
            //tablo listeleme
            var rCon = ac.openConnection(ac.oracleConnection(loginName, password));
            oda = new OracleDataAdapter(query, rCon);
            ds = new DataSet();
            oda.Fill(ds);
            DataTable table = ds.Tables[0];
            ViewBag.list = setTableQuotToList(qo, table);
            
            //tedarikçi combobox için
            string dropbox_query = @"SELECT vendor_no, IFSAPP.supplier_info_api.get_name(vendor_no) suppname FROM IFSAPP.INQUIRY_ORD_SUPPLIER t WHERE inquiry_no=" + p1;
            oda_dropbox = new OracleDataAdapter(dropbox_query, rCon);
            DataTable companies = new DataTable();
            oda_dropbox.Fill(companies);
            ViewBag.dropdownList = setTableQuotToList(qo, companies);
            ac.closeConnection(rCon);
            return View();
        }
        [HttpPost]
        public IActionResult QuotationBlanket(string id, string selected, bool success)
        {
            ac = new AccountController();

            string new_id = id.Substring(0, id.IndexOf(" "));
            string new_rev = id.Substring(id.IndexOf(".") + 1, (id.Length - id.IndexOf(".")) - (id.Length - id.IndexOf(")") + 1));

            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            string password = HttpContext.Session.GetString("kullaniciParola");

            var rCon = ac.openConnection(ac.oracleConnection(loginName, password));

            cmd = rCon.CreateCommand();

            string sSqlLineNo = @"select nvl(min(line_no), 0) from IFSAPP.ccn_quot_blanket_auth a left join IFSAPP.PURCH_AUTHORIZE_GROUP_LINE b on a.authorize_group_id=b.authorize_group_id and " +
            "IFSAPP.inquiry_api.get_company(a.inquiry_no)=b.company where inquiry_no=" + new_id + " and vendor_no='" + selected + "' and revision_no=" + new_rev + " and nvl(b.authorize_id, a.authorize_id)='" + loginName + "'";

            cmd.CommandText = sSqlLineNo;

            string execute_scalar = cmd.ExecuteScalar().ToString();

            string OraApi =  success ?  @"DECLARE" +
            " p0_ FLOAT := " + new_id + ";" +
            " p1_ VARCHAR2(32000) := '" + selected + "';" +
            " p2_ FLOAT := " + new_rev + ";" +
            " p3_ FLOAT := " + execute_scalar + ";" +
            " BEGIN" +
            " IFSAPP.ccn_quot_blanket_auth_api.Authorize(p0_ , p1_ , p2_ , p3_ ); " +
            " commit;" +
            " END;": @"DECLARE" +
            " p0_ FLOAT := " + new_id + ";" +
            " p1_ VARCHAR2(32000) := '" + selected + "';" +
            " p2_ FLOAT := " + new_rev + ";" +
            " p3_ FLOAT := " + execute_scalar + ";" +
            " BEGIN" +
            " IFSAPP.ccn_quot_blanket_auth_api.Revise(p0_, p1_, p2_, p3_ );" +
            " commit;" +
            " END;";

            new_cmd = rCon.CreateCommand();
            new_cmd.CommandText = OraApi;
            new_cmd.ExecuteNonQuery();

            ac.closeConnection(rCon);
            return Content("success");
        }
        public IActionResult ContractValuation(RequestViewModel rvm)
        {
            ac = new AccountController();
            cv = new ContractValuation();

            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            string password = HttpContext.Session.GetString("kullaniciParola");

            string p0 = Request.Query["id"].ToString();
            ViewBag.Id = p0;
            words = p0.Split('-');


            string p1 = words.Length == 2 ? p0.Substring(0, p0.IndexOf("-")) : words[0] + "-" + words[1];//words.Length == 2 ise varolan degilse words al
            string p2 = words.Length == 2 ? p0.Substring((p0.IndexOf("-")) + 1, (p0.Length - p0.IndexOf("-")) - 1) : words[2]; //words[2]; words.Length == 2 ise varolan degilse words al

            ViewBag.loginname = loginName;
            if (rvm.RedirectToIfs)
            return RedirectToAction("Ifs", "Home", new { loginname = loginName });


            string query = @"select 'Sözleşme' Aciklama,
            sub_con_no || '-' || valuation_no || '-' || ifsapp.sub_contract_api.Get_Sub_Con_Name(sub_con_no) Deger
            from ifsapp.SUBVAL_VALUATION 
            where sub_con_no = '@p1' AND valuation_no=@p2 
            union all
            select 'Para Birimi' Aciklama,
                   ifsapp.sub_contract_api.Get_Currency_Code(sub_con_no)
              from ifsapp.SUBVAL_VALUATION
            where sub_con_no='@p1' AND valuation_no=@p2
            union all
            select 'Ilgili Kisi' Aciklama, ifsapp.PERSON_INFO_API.Get_Name(created_by)
              from ifsapp.SUBVAL_VALUATION
            where sub_con_no ='@p1' AND valuation_no=@p2
            union all
            select 'HAKEDİŞ KAPAK BİLGİLERİ', ''
              FROM DUAL
            union all

            select

            case
               when aa.row_no = '6' then
                'Toplam Imalat Tutari'
               when aa.row_no = '2' then
                'Imalat Tutari'
               when aa.row_no = '3' then
                'Ilave Isler Tutari'
               when aa.row_no = '8' then
                'Kdv Tutari'
               when aa.row_no = '9' then
                'Stopaj Vergisi'
               when aa.row_no = '10' then
                'Avans Stopaj Vergisi'
               when aa.row_no = '11' then
                'Kdv Tevkifati'
               when aa.row_no = '14' then
                'Ihzarat Tutari'
               when aa.row_no = '15' then
                'Ihzarat Kesintisi Tutari'
               when aa.row_no = '18' then
                'Malzeme Kesintisi'
               when aa.row_no = '19' then
                'Ekipman Kesintisi'
               when aa.row_no = '20' then
                'Yemek Kesintisi'
               when aa.row_no = '21' then
                'Personel Kesintisi'
               when aa.row_no = '22' then
                'Tutanak Ceza Kesintisi'
               when aa.row_no = '23' then
                'Diger Kesintiler'
               when aa.row_no = '24' then
                'Kesinti Kdvsi'
               when aa.row_no = '31' then
                'Ali Konulan Bedel'
               when aa.row_no = '35' then
                'Net Odenecek Tutar'
               else
                'DGR'
            end ozet,
            rtrim(to_char(ROUND(aa.this_tot_amount,2), 'FM9G999G999G999G999G999D999', 'NLS_NUMERIC_CHARACTERS='',.'''), ',') as  tutar

                 from ifsapp.CCN_SUB_VAL_SUMMARY aa
                where sub_Con_no = '@p1' AND valuation_no=@p2
                  and row_no in
                      (6, 2, 3, 8, 9, 10, 11, 14, 15, 18, 19, 20, 21, 22, 23, 24, 31, 35)
            union all

            select 'ONAY' Aciklama,
                        aa.line_no  || '*' ||   aa.step_no as Deger
      
                        from ifsapp.SUBVAL_VALUATION t
                        left join ifsapp.APPROVAL_ROUTING aa on aa.key_ref=t.key_ref
                        where sub_con_no = '@p1' AND valuation_no=@p2 
                        and t.certificate_no is null
                        and t.objstate = 'Certification In Progress'
                        and aa.approval_status is null
                and aa.step_no = (select min(step_no)
                                    from ifsapp.approval_routing bb
                                    where bb.key_ref = aa.key_Ref
                                and bb.app_sign is null)";
          



            query = query.Replace("@p1", p1);
            query = query.Replace("@p2", p2);

            
            var rCon = ac.openConnection(ac.oracleConnection(loginName, password));
            oda = new OracleDataAdapter(query, rCon);
            ds = new DataSet();
            oda.Fill(ds);
            DataTable table = ds.Tables[0];

            ViewBag.list = setTableContToList(cv, table);

            ac.closeConnection(rCon);
            return View();
        }
        [HttpPost]
        public IActionResult ContractValuation(string id,string stepnoandlineno)
        {
            ac = new AccountController();
            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            string password = HttpContext.Session.GetString("kullaniciParola");

            words = id.Split('-');


            string p1 = words.Length == 2 ? id.Substring(0, id.IndexOf("-")) : words[0] + "-" + words[1];//words.Length == 2 ise varolan degilse words al
            string p2 = words.Length == 2 ? id.Substring((id.IndexOf("-")) + 1, (id.Length - id.IndexOf("-")) - 1) : words[2]; //words[2]; words.Length == 2 ise varolan degilse words al

            string lineno = stepnoandlineno.Substring(0, stepnoandlineno.IndexOf("*"));

            string stepno = stepnoandlineno.Substring((stepnoandlineno.IndexOf("*")) + 1, (stepnoandlineno.Length - stepnoandlineno.IndexOf("*")) - 1);

            var rCon = ac.openConnection(ac.oracleConnection(loginName, password));

            cmd = rCon.CreateCommand();


            string OraApi = @"DECLARE
              p0_ VARCHAR2(32000) := 'SubvalValuation';
              p1_ VARCHAR2(32000) := 'SUB_CON_NO=@p1^VALUATION_NO=@p2^';
              p2_ FLOAT := @lineno;
              p3_ FLOAT := @stepno;
              p4_ VARCHAR2(32000) := 'APP';
              BEGIN 
                IFSAPP.APPROVAL_ROUTING_API.Set_Next_App_Step(p0_,p1_,p2_,p3_,p4_);
              commit;
              END;";


            OraApi = OraApi.Replace("@p1", p1);
            OraApi = OraApi.Replace("@p2", p2);
            OraApi = OraApi.Replace("@lineno", lineno);
            OraApi = OraApi.Replace("@stepno", stepno);

            cmd = rCon.CreateCommand();
            cmd.CommandText = OraApi;
            cmd.ExecuteNonQuery();

            ac.closeConnection(rCon);
            return Content("success");
        }
        public IActionResult Contact()
        {
            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            ViewBag.loginname = loginName;
            return View();
        }
    }

}
