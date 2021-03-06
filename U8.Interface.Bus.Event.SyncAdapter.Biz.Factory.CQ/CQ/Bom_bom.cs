﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MSXML2;


namespace U8.Interface.Bus.Event.SyncAdapter.Biz.Factory.CQ
{
    public class Bom_bom : BizBase
    {

        private string opertype;
        int bomId;
        System.Data.DataSet ds;

        Bom_opcomponent detailBiz;
        Bom_opcomponentsub detailSubBiz;

        /// <summary>
        /// 保存前事件使用(保存并自动审核时，BOM单不会自动调用审核事件)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="ds"></param>
        /// <param name="ufConnStr"></param>
        public Bom_bom(ref ADODB.Connection conn, ref System.Data.DataSet ds, string ufConnStr, string _opertype)
            : base(conn, ufConnStr)
        {


            oracleTableName = "MES_CQ_bom_bom";   //目标表名
            oraclePriKey = "bomid";      //目标表逻辑主键 
            fieldcmpTablename = "MES_CQ_bom_bom";
            ufTableName = "bom_bom"; // "SaleOrderQ";       //来源表名
            ufPriKey = "bomid";          //来源表主键
            this.opertype = _opertype; 
            this.ds = ds;
         
        }


        public Bom_bom(ref ADODB.Connection conn, int bomId, string ufConnStr, string _opertype)
            : base(conn, ufConnStr)
        {
            oracleTableName = "MES_CQ_bom_bom";   //目标表名
            oraclePriKey = "bomid";      //目标表逻辑主键 
            fieldcmpTablename = "MES_CQ_bom_bom";
            ufTableName = "bom_bom"; // "SaleOrderQ";       //来源表名
            ufPriKey = "bomid";          //来源表主键
            this.opertype = _opertype; 
            this.bomId = bomId; 
        }


        public override object Insert()
        {
            if (bomId > 0)
            {
                SetAddData(bomId);
            }
            else
            {
                if (CheckAutoAudit(ref ds))
                {
                    SetAddData(ref ds);
                }

            }
            if (l.Count == 0 && lst.Count == 0)
            {
                return null;
            }
            else
            {
                StringBuilder sbd = new StringBuilder();
                if (bNoCase)
                {
                    sbd.Append(detailSubBiz.CreateDeleteString());
                    sbd.Append(detailBiz.CreateDeleteString());
                    sbd.Append(this.CreateDeleteString()); 
                }

                StringBuilder sbi = new StringBuilder();
                sbi.Append(this.CreateInsertString());
                sbi.Append(detailBiz.CreateInsertString());
                sbi.Append(detailSubBiz.CreateInsertString());
                sbi.Replace("main|##newguid", Guid.NewGuid().ToString());
                return ExecSql(sbd.ToString() + sbi.ToString());
            }
        }


        public override object Delete()
        {
            if (bomId > 0)
            {
                SetDelData(bomId);
            }
            else
            {
                if (CheckAutoAudit(ref ds))
                {
                    SetDelData(ref ds);
                }

            }

            StringBuilder sbd = new StringBuilder();
            if (bNoCase)
            {
                sbd.Append(detailSubBiz.CreateDeleteString());
                sbd.Append(detailBiz.CreateDeleteString());
                sbd.Append(this.CreateDeleteString()); 
            }

            StringBuilder sbi = new StringBuilder();
            if (bSaveOper)
            {
                lst = new List<List<BaseMode>>();
                l = new List<BaseMode>();
                detailBiz.l = new List<BaseMode>();
                detailBiz.lst = new List<List<BaseMode>>();

                if (bomId > 0)
                {
                    SetAddData(bomId);
                }
                else
                {
                    if (CheckAutoAudit(ref ds))
                    {
                        SetAddData(ref ds);
                    }

                }
                if (l.Count == 0 && lst.Count == 0)
                {
                    return null;
                }
                else
                { 
                    sbi.Append(this.CreateInsertString());
                    sbi.Append(detailBiz.CreateInsertString());
                    sbi.Append(detailSubBiz.CreateInsertString());
                    sbi.Replace("main|##newguid", Guid.NewGuid().ToString());

                }
 
            }

            StringBuilder sbm = new StringBuilder();
            sbm.Append(sbd);
            sbm.Append(sbi);
            if (sbm.Length > 0)
            {
                return ExecSql(sbm.ToString());
            }
            return 1;

        }


  

        /// <summary>
        /// 保存时,默认为审核状态,需要往MES表中插入数据
        /// ITEMCODE
        /// SBITEMCODE
        /// SBSITEMCODE
        /// SBITEMQTY
        /// SBITEMECN
        /// SBITEMEFFDATE
        /// SBITEMINVDATE
        /// SBITEMUOM
        /// SBOMVER
        /// LOCATION
        /// </summary>
        /// <param name="doc"></param>
        public void SetAddData(ref DataSet ds)
        {

            detailBiz = new Bom_opcomponent(ref conn, bomId, ufConnStr, opertype);

            //主表 
            string InvCode;
            string VersionEffDate;
            string VersionEndDate;
            string version;
            string BomId;

            string DInvCode;
            string DInvUnit;

            List<BaseMode> tmpMainLst = new List<BaseMode>();
            //for (int i = 0; i < ds.Tables["StandardBom"].Rows.Count; i++)
            //{
            BomId = ds.Tables["StandardBom"].Rows[0]["BomId"].ToString();
            InvCode = GetInvCodeByPartid(ds.Tables["BomParent"].Rows[0]["ParentId"].ToString());
            version = ds.Tables["StandardBom"].Rows[0]["Version"].ToString();
            VersionEffDate = ds.Tables["StandardBom"].Rows[0]["VersionEffDate"].ToString();
            VersionEndDate = ds.Tables["StandardBom"].Rows[0]["VersionEndDate"].ToString();


            tmpMainLst.Add(new BaseMode(null, null, null, "id", "main|##newguid", null, null));
            tmpMainLst.Add(new BaseMode("bomid", BomId, null, "bomid", BomId, null, null));
            tmpMainLst.Add(new BaseMode(null, null, null, "Version", version, null, null));
            tmpMainLst.Add(new BaseMode(null, null, null, "VersionEffDate", VersionEffDate, null, null));
            tmpMainLst.Add(new BaseMode(null, null, null, "cInvCode", InvCode, null, null));
            tmpMainLst.Add(new BaseMode(null, null, null, "status", "0", null, null));
            if (opertype.Equals("a"))
            {
                tmpMainLst.Add(new BaseMode(null, null, null, "opertype", "0", null, null));
            }
            else
            {
                tmpMainLst.Add(new BaseMode(null, null, null, "opertype", "2", null, null));
            }


            this.lst.Add(tmpMainLst);
            //}


            //子件表
            for (int i = 0; i < ds.Tables["BomComponents"].Rows.Count; i++)
            {
                List<BaseMode> tmpDetailLst = new List<BaseMode>();

                if (ds.Tables["BomComponents"].Rows[i].RowState.ToString().ToLower().Equals("deleted"))
                {
                    continue;
                }
                DInvCode = GetInvCodeByPartid(ds.Tables["BomComponents"].Rows[i]["ComponentId"].ToString());

                tmpDetailLst.Add(new BaseMode(null, null, null, "id", "main|##newguid", null, null));
                tmpDetailLst.Add(new BaseMode(null, null, null, "did", Guid.NewGuid().ToString(), null, null));
                tmpDetailLst.Add(new BaseMode(null, null, null, "cInvCode", DInvCode, null, null));
                tmpDetailLst.Add(new BaseMode(null, null, null, "BaseQtyN", ds.Tables["BomComponents"].Rows[i]["BaseQtyN"].ToString(), null, null));
                tmpDetailLst.Add(new BaseMode(null, null, null, "BaseQtyD", ds.Tables["BomComponents"].Rows[i]["BaseQtyD"].ToString(), null, null));
                if (opertype.Equals("a"))
                {
                    tmpDetailLst.Add(new BaseMode(null, null, null, "opertype", "0", null, null));
                }
                else
                {
                    tmpDetailLst.Add(new BaseMode(null, null, null, "opertype", "2", null, null));
                }
                detailBiz.lst.Add(tmpDetailLst);
            }

            //替代料 ds.Tables["ComponentSubstitutes"]

        }





        /// <summary>
        /// 保存时,默认为审核状态,需要往MES表中插入数据
        /// ITEMCODE
        /// SBITEMCODE
        /// SBSITEMCODE
        /// SBITEMQTY
        /// SBITEMECN
        /// SBITEMEFFDATE
        /// SBITEMINVDATE
        /// SBITEMUOM
        /// SBOMVER
        /// LOCATION
        /// </summary>
        /// <param name="doc"></param>
        public void SetDelData(ref DataSet ds)
        {


            //主表 
            string InvCode;
            string VersionEffDate;
            string VersionEndDate;
            string version;
            string BomId;
            string DInvCode;
            string DInvUnit;

            List<BaseMode> tmpMainLst = new List<BaseMode>();
            //for (int i = 0; i < ds.Tables["StandardBom"].Rows.Count; i++)
            //{

            BomId = ds.Tables["StandardBom"].Rows[0]["BomId"].ToString();
            InvCode = GetInvCodeByPartid(ds.Tables["BomParent"].Rows[0]["ParentId"].ToString());
            version = ds.Tables["StandardBom"].Rows[0]["Version"].ToString();
            VersionEffDate = ds.Tables["StandardBom"].Rows[0]["VersionEffDate"].ToString();
            VersionEndDate = ds.Tables["StandardBom"].Rows[0]["VersionEndDate"].ToString();

            tmpMainLst.Add(new BaseMode(null, null, null, "bomid", BomId, null, null));
            tmpMainLst.Add(new BaseMode(null, null, null, "Version", version, null, null));
            tmpMainLst.Add(new BaseMode(null, null, null, "VersionEffDate", VersionEffDate, null, null));
            tmpMainLst.Add(new BaseMode(null, null, null, "cInvCode", InvCode, null, null));
            tmpMainLst.Add(new BaseMode(null, null, null, "status", "0", null, null)); 
            tmpMainLst.Add(new BaseMode(null, null, null, "opertype", "2", null, null));

            this.lst.Add(tmpMainLst);
            //}
 

            //子件表
            for (int i = 0; i < ds.Tables["BomComponents"].Rows.Count; i++)
            {
                List<BaseMode> tmpDetailLst = new List<BaseMode>();

                if (ds.Tables["BomComponents"].Rows[i].RowState.ToString().ToLower().Equals("deleted"))
                {
                    continue;
                }
                DInvCode = GetInvCodeByPartid(ds.Tables["BomComponents"].Rows[i]["ComponentId"].ToString());

                tmpDetailLst.Add(new BaseMode(null, null, null, "cInvCode", DInvCode, null, null));
                tmpDetailLst.Add(new BaseMode(null, null, null, "BaseQtyN", ds.Tables["BomComponents"].Rows[i]["BaseQtyN"].ToString(), null, null));
                tmpDetailLst.Add(new BaseMode(null, null, null, "BaseQtyD", ds.Tables["BomComponents"].Rows[i]["BaseQtyD"].ToString(), null, null));
                tmpDetailLst.Add(new BaseMode(null, null, null, "opertype", "2", null, null));
                detailBiz.lst.Add(tmpDetailLst);
            }

            //替代料 ds.Tables["ComponentSubstitutes"]
           
        }



        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="bomId"></param>
        public void SetAddData(int bomId)
        {
            detailBiz = new Bom_opcomponent(ref conn, bomId, ufConnStr,opertype);
            detailSubBiz = new Bom_opcomponentsub(ref conn, bomId, ufConnStr, opertype);

            //主表  bom_bom
            DataSet dsMainBom = new DataSet();
            DataTable dtMainBom = GetBomHeadByBomId(bomId);
            dsMainBom.Tables.Add(dtMainBom.Copy());
            //子表 bom_
            DataSet dsBom = new DataSet();
            DataTable dtBom = GetChildByBomId(bomId);
            dsBom.Tables.Add(dtBom.Copy());
            //替代料
            DataSet dsSubChildBom = new DataSet();
            DataTable dtSubChildBom = GetSubChildByBomId(bomId);
            dsSubChildBom.Tables.Add(dtSubChildBom.Copy());
 
            List<BaseMode> tmpMainLst = new List<BaseMode>(); 
 
            //表头
            for (int i = 0; i < dsMainBom.Tables.Count; i++)
            {
                if (dsMainBom.Tables[i].Rows.Count > 0)
                {
                    for (int j = 0; j < dsMainBom.Tables[i].Rows.Count; j++)
                    {
                        // h.InvCode,h.InvName,h.version,h.bomid, h.VersionEffDate 
                        tmpMainLst.Add(new BaseMode(null, null, null, "id", "main|##newguid", null, null));
                        tmpMainLst.Add(new BaseMode("bomid",dsMainBom.Tables[i].Rows[j]["bomid"].ToString(), null, "bomid", dsMainBom.Tables[i].Rows[j]["bomid"].ToString(), null, null));
                        tmpMainLst.Add(new BaseMode(null, null, null, "Version", dsMainBom.Tables[i].Rows[j]["version"].ToString(), null, null));
                        tmpMainLst.Add(new BaseMode(null, null, null, "VersionEffDate", dsMainBom.Tables[i].Rows[j]["VersionEffDate"].ToString(), null, null));
                        tmpMainLst.Add(new BaseMode(null, null, null, "cInvCode", dsMainBom.Tables[i].Rows[j]["InvCode"].ToString(), null, null));
                        tmpMainLst.Add(new BaseMode(null, null, null, "status", "0", null, null));
                        if (opertype.Equals("a"))
                        {
                            tmpMainLst.Add(new BaseMode(null, null, null, "opertype", "0", null, null));
                        }
                        else
                        {
                            tmpMainLst.Add(new BaseMode(null, null, null, "opertype", "2", null, null));
                        }
                        this.lst.Add(tmpMainLst);
                    }
                }
            }
            //DataTable dtLocation = GetLocation(bomId);
            //表体
            for (int i = 0; i < dsBom.Tables.Count; i++)
            {
                if (dsBom.Tables[i].Rows.Count > 0)
                { 
                    for (int j = 0; j < dsBom.Tables[i].Rows.Count; j++)
                    {

                        List<BaseMode> tmpDetailLst = new List<BaseMode>();
                        tmpDetailLst.Add(new BaseMode(null, null, null, "id", "main|##newguid", null, null));
                        string matid = Guid.NewGuid().ToString();
                        string opcomponentid = dsBom.Tables[i].Rows[j]["OpComponentId"].ToString();
                        tmpDetailLst.Add(new BaseMode(null, null, null, "did", matid, null, null));
                        tmpDetailLst.Add(new BaseMode(null, null, null, "cInvCode", dsBom.Tables[i].Rows[j]["DInvCode"].ToString(), null, null));
                        tmpDetailLst.Add(new BaseMode(null, null, null, "BaseQtyN", dsBom.Tables[i].Rows[j]["DBaseQtyN"].ToString(), null, null));
                        tmpDetailLst.Add(new BaseMode(null, null, null, "BaseQtyD", dsBom.Tables[i].Rows[j]["DBaseQtyD"].ToString(), null, null));
                        if (opertype.Equals("a"))
                        {
                            tmpDetailLst.Add(new BaseMode(null, null, null, "opertype", "0", null, null));
                        }
                        else
                        {
                            tmpDetailLst.Add(new BaseMode(null, null, null, "opertype", "2", null, null));
                        }
                        detailBiz.lst.Add(tmpDetailLst);


                        //替代料
                        for (int f = 0; f < dsSubChildBom.Tables.Count; f++)
                        {
                            if (dsSubChildBom.Tables[f].Rows.Count > 0)
                            {
                                DataRow[] dr = dsSubChildBom.Tables[f].Select("OpComponentId =" + opcomponentid);
                                for (int h = 0; h < dr.Length; h++)
                                {
                                    List<BaseMode> tmpDetailSubLst = new List<BaseMode>();

                                    tmpDetailSubLst.Add(new BaseMode(null, null, null, "did", matid, null, null));
                                    tmpDetailSubLst.Add(new BaseMode(null, null, null, "ddid", Guid.NewGuid().ToString(), null, null));
                                    tmpDetailSubLst.Add(new BaseMode(null, null, null, "cInvCode", dr[h]["cInvCode"].ToString(), null, null));
                                    tmpDetailSubLst.Add(new BaseMode(null, null, null, "Sequence", dr[h]["Sequence"].ToString(), null, null));
                                    tmpDetailSubLst.Add(new BaseMode(null, null, null, "Factor", dr[h]["Factor"].ToString(), null, null));
                                    tmpDetailSubLst.Add(new BaseMode(null, null, null, "ReplaceFlag", dr[h]["ReplaceFlag"].ToString(), null, null));
                       
                                    if (opertype.Equals("a"))
                                    {
                                        tmpDetailSubLst.Add(new BaseMode(null, null, null, "opertype", "0", null, null));
                                    }
                                    else
                                    {
                                        tmpDetailSubLst.Add(new BaseMode(null, null, null, "opertype", "2", null, null));
                                    }
                                    detailSubBiz.lst.Add(tmpDetailSubLst);

                                }
                            }
                        }
   
                    }
                    
                }
            }


        }



        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="bomId"></param>
        public void SetDelData(int bomId)
        {

            detailBiz = new Bom_opcomponent(ref conn, bomId, ufConnStr, opertype);
            detailSubBiz = new Bom_opcomponentsub(ref conn, bomId, ufConnStr, opertype);
  
            //主表  bom_bom
            DataSet dsMainBom = new DataSet();
            DataTable dtMainBom = GetBomHeadByBomId(bomId);
            dsMainBom.Tables.Add(dtMainBom.Copy());
            //子表 bom_
            DataSet dsBom = new DataSet();
            DataTable dtBom = GetChildByBomId(bomId);
            dsBom.Tables.Add(dtBom.Copy());
            //替代料
            DataSet dsSubChildBom = new DataSet();
            DataTable dtSubChildBom = GetSubChildByBomId(bomId);
            dsSubChildBom.Tables.Add(dtSubChildBom.Copy());


            //表头
            for (int i = 0; i < dsMainBom.Tables.Count; i++)
            {
                if (dsMainBom.Tables[i].Rows.Count > 0)
                {
                    for (int j = 0; j < dsMainBom.Tables[i].Rows.Count; j++)
                    {  
                        List<BaseMode> tmpMainLst = new List<BaseMode>();
                        // h.InvCode,h.InvName,h.version,h.bomid, h.VersionEffDate
                        tmpMainLst.Add(new BaseMode("bomid",Convert.ToString(bomId), null, "bomid", dsMainBom.Tables[i].Rows[j]["bomid"].ToString(), null, null));
                        tmpMainLst.Add(new BaseMode(null, null, null, "Version", dsMainBom.Tables[i].Rows[j]["version"].ToString(), null, null));
                        tmpMainLst.Add(new BaseMode(null, null, null, "VersionEffDate", dsMainBom.Tables[i].Rows[j]["VersionEffDate"].ToString(), null, null));
                        tmpMainLst.Add(new BaseMode(null, null, null, "cInvCode", dsMainBom.Tables[i].Rows[j]["InvCode"].ToString(), null, null));
                        tmpMainLst.Add(new BaseMode(null, null, null, "status", "0", null, null));
                        tmpMainLst.Add(new BaseMode(null, null, null, "opertype", "2", null, null));
                        this.lst.Add(tmpMainLst);
                    }
                }
            }
            //DataTable dtLocation = GetLocation(bomId);
            //表体
            for (int mti = 0; mti < dsBom.Tables.Count; mti++)
            {
                if (dsBom.Tables[mti].Rows.Count > 0)
                {
                    for (int j = 0; j < dsBom.Tables[mti].Rows.Count; j++)
                    {
                        //材料
                        List<BaseMode> tmpDetailLst = new List<BaseMode>();
                        string matid = Guid.NewGuid().ToString();
                        string opcomponentid = dsBom.Tables[mti].Rows[j]["OpComponentId"].ToString();
                        tmpDetailLst.Add(new BaseMode(null, null, null, "did", matid, null, null));
                        tmpDetailLst.Add(new BaseMode(null, null, null, "cInvCode", dsBom.Tables[mti].Rows[j]["DInvCode"].ToString(), null, null));
                        tmpDetailLst.Add(new BaseMode(null, null, null, "BaseQtyN", dsBom.Tables[mti].Rows[j]["DBaseQtyN"].ToString(), null, null));
                        tmpDetailLst.Add(new BaseMode(null, null, null, "BaseQtyD", dsBom.Tables[mti].Rows[j]["DBaseQtyD"].ToString(), null, null));
                        tmpDetailLst.Add(new BaseMode(null, null, null, "opertype", "2", null, null));
                        detailBiz.lst.Add(tmpDetailLst);
 

                        //替代料
                        for (int f = 0; f < dsSubChildBom.Tables.Count; f++)
                        {
                            if (dsSubChildBom.Tables[f].Rows.Count > 0)
                            {
                                DataRow[] dr = dsSubChildBom.Tables[f].Select("OpComponentId =" + opcomponentid);
                                for (int h = 0; h < dr.Length; h++)
                                {
                                    List<BaseMode> tmpDetailSubLst = new List<BaseMode>();

                                    tmpDetailSubLst.Add(new BaseMode(null, null, null, "did", matid, null, null));
                                    tmpDetailSubLst.Add(new BaseMode(null, null, null, "ddid", Guid.NewGuid().ToString(), null, null));
                                    tmpDetailSubLst.Add(new BaseMode(null, null, null, "cInvCode", dr[h]["cInvCode"].ToString(), null, null));
                                    tmpDetailSubLst.Add(new BaseMode(null, null, null, "Sequence", dr[h]["Sequence"].ToString(), null, null));
                                    tmpDetailSubLst.Add(new BaseMode(null, null, null, "Factor", dr[h]["Factor"].ToString(), null, null));
                                    tmpDetailSubLst.Add(new BaseMode(null, null, null, "ReplaceFlag", dr[h]["ReplaceFlag"].ToString(), null, null));

                                    if (opertype.Equals("a"))
                                    {
                                        tmpDetailSubLst.Add(new BaseMode(null, null, null, "opertype", "0", null, null));
                                    }
                                    else
                                    {
                                        tmpDetailSubLst.Add(new BaseMode(null, null, null, "opertype", "2", null, null));
                                    }
                                    detailSubBiz.lst.Add(tmpDetailSubLst);

                                }
                            }
                        }

                    }

                }
            }
        }



        #region 辅助方法

        /// <summary>
        /// 判断标准BOM是不是保存后默认为审核状态
        /// </summary>
        /// <returns></returns>
        private bool CheckAutoAudit(ref System.Data.DataSet ds)
        {
            string bomId = ds.Tables["StandardBom"].Rows[0]["BomId"].ToString();
            string sql;
            DataTable dt;
            //string sql = " select count(1) as  cc from bom_bom with(nolock) where bomid ='" + bomId + "'  ";
            //DataTable dt = UFSelect(sql);
            //if (dt.Rows.Count > 0)
            //{
            //    if ((int)dt.Rows[0][0] > 0)
            //    {
            //        return false;
            //    }
            //}

            sql = " select BomDefaultStatus  from mom_parameter  with(nolock)  ";
            dt = UFSelect(sql);
            if (dt.Rows.Count > 0)
            {
                return (bool)(dt.Rows[0][0].ToString().Trim().Equals("3"));
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 根据partid取存货编码
        /// </summary>
        /// <param name="partid"></param>
        /// <returns></returns>
        private string GetInvCodeByPartid(string partid)
        {
            string sql = " select top 1 InvCode from bas_part with(nolock) where PartId = '" + partid + "' ";
            DataTable dt = UFSelect(sql);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            return "";
        }



        /// <summary>
        /// 根据存货编码取计量单位
        /// </summary>
        /// <param name="partid"></param>
        /// <returns></returns>
        private string GetUnitCodeByInvCode(string cinvcode)
        {
            string sql = " select cComUnitCode from inventory with(nolock) where cinvcode = '" + cinvcode + "' ";
            DataTable dt = UFSelect(sql);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            return "";
        }



        /// <summary>
        /// 根据partid取计量单位
        /// </summary>
        /// <param name="partid"></param>
        /// <returns></returns>
        private string GetUnitCodeByPartid(string partid)
        {
            string sql = " select  i.cComUnitCode from bas_part rm with(nolock) INNER join inventory i with(nolock) on rm.InvCode = i.cInvCode  where rm.PartId = '" + partid + "' ";
            DataTable dt = UFSelect(sql);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            return "";
        }

        private DataTable GetBomHeadByBomId(int bomId)
        {

            DataTable dtBom = new DataTable();
            StringBuilder sb = new StringBuilder(); 
            sb.Append("     SELECT  ");
            sb.Append("            h.bomid,h.InvCode,h.InvName,h.version,h.bomid, h.VersionEffDate   "); 
            sb.Append("     FROM v_bom_head h  with(nolock) "); 
            sb.Append("     WHERE  H.bomid ='" + bomId + "' "); 

            dtBom = UFSelect(sb.ToString());
            return dtBom;
        }

        /// <summary>
        /// 获取子件(不含替代料)
        /// </summary>
        /// <param name="bomId"></param>
        /// <returns></returns>
        private DataTable GetChildByBomId(int bomId)
        {
            DataTable dtBom = new DataTable();
            StringBuilder sb = new StringBuilder();
             
            sb.Append(" SELECT V.* FROM   (     ");
            sb.Append(" SELECT              bom.bomid,bas.InvCode,inv.cinvname as InvName,bom.version,bom.VersionEffDate,   ");
            sb.Append(" D.BaseQtyN as DBaseQtyN,D.BaseQtyD as DBaseQtyD, bas1.InvCode as DInvCode,bas1.InvCode AS subInvCode,d.EffBegDate as DEffBegDate,d.EffEndDate as DEffEndDate,  ");
            sb.Append(" d.OpComponentId        ");
            sb.Append(" FROM bom_bom bom with(nolock) ");
            sb.Append(" inner join bom_parent bp with(nolock) on bp.bomid=bom.bomid ");
            sb.Append(" inner join bas_part bas  with(nolock) on bas.partid=bp.parentid ");
            sb.Append(" inner join inventory inv with(nolock) on inv.cinvcode=bas.InvCode     ");
            sb.Append(" inner JOIN bom_opcomponent d  with(nolock) ON bom.bomid = d.bomid  ");
            sb.Append("  inner join bas_part bas1 with(nolock) on bas1.partid=d.componentid    WHERE  bom.bomid ='" + bomId + "'  ) V ");
            sb.Append("  ORDER BY  V.DInvCode,V.subInvCode ");


            dtBom = UFSelect(sb.ToString());
            return dtBom;

        }


        /// <summary>
        /// 获取替代料
        /// </summary>
        /// <param name="bomId"></param>
        /// <returns></returns>
        private DataTable GetSubChildByBomId(int bomId)
        {
            DataTable dtBom = new DataTable();
            StringBuilder sb = new StringBuilder();

            sb.Append(" SELECT V.* FROM   (     ");
            sb.Append(" SELECT   bom.bomid,bom.version,bom.VersionEffDate,    ");
            sb.Append(" d.OpComponentId      , ");
            sb.Append(" subd.PartId,subd.Sequence,subd.ReplaceFlag, subd.Factor , ");
            sb.Append(" bas1.InvCode as cInvCode,inv.cinvname as cInvName  ");
            sb.Append(" FROM bom_bom bom with(nolock) ");
            sb.Append(" inner JOIN bom_opcomponent d  with(nolock) ON bom.bomid = d.bomid  ");
            sb.Append(" inner JOIN bom_opcomponentsub subd  with(nolock) ON d.OpComponentId = subd.OpComponentId  ");
            sb.Append(" inner join bas_part bas1 with(nolock) on bas1.partid=subd.PartId    ");
            sb.Append(" inner join inventory inv with(nolock) on inv.cinvcode=bas1.InvCode   WHERE  bom.bomid ='" + bomId + "'  ) V ");
            sb.Append("  ORDER BY  V.cInvCode ");


            dtBom = UFSelect(sb.ToString());
            return dtBom;

        }


        /// <summary>
        /// 获取子件及替代料
        /// </summary>
        /// <param name="bomId"></param>
        /// <returns></returns>
        private DataTable GetChildAndSubByBomId(int bomId)
        {
            DataTable dtBom = new DataTable();
            StringBuilder sb = new StringBuilder();

            sb.Append(" SELECT V.* FROM  ");
            sb.Append(" ( ");
            sb.Append("     SELECT  ");
            sb.Append("           h.InvCode,h.InvName,h.version,  ");
            sb.Append("           D.DInvCode,D.DInvCode AS subInvCode,d.DEffBegDate,d.DEffEndDate,d.DInvUnit,d.DBaseQtyD,d.OpComponentId    ");
            sb.Append("     FROM v_bom_head h  with(nolock) ");
            sb.Append("           LEFT JOIN v_bom_detail d  with(nolock) ON h.bomid = d.bomid  ");
            sb.Append("     WHERE  H.bomid ='" + bomId + "' ");
            sb.Append("     UNION ALL ");
            sb.Append("     SELECT ");
            sb.Append("           h.InvCode,h.InvName,h.version, ");
            sb.Append("           D.DInvCode,rm.invcode AS subInvCode,d.DEffBegDate,d.DEffEndDate,i.cComUnitCode as DInvUnit,d.DBaseQtyD,d.OpComponentId   ");
            sb.Append("     FROM v_bom_head h  with(nolock) ");
            sb.Append("           INNER JOIN v_bom_detail d  with(nolock) ON h.bomid = d.bomid  ");
            sb.Append("           INNER JOIN bom_opcomponentsub r  with(nolock) ON d.opcomponentid = r.opcomponentid ");
            sb.Append("           INNER JOIN bas_part rm  with(nolock) ON r.PartId = rm.PartId ");
            sb.Append("           INNER JOIN inventory i  with(nolock) ON i.cInvCode = rm.InvCode ");
            sb.Append("           WHERE  H.bomid ='" + bomId + "' AND ISNULL(rm.InvCode,'') <> '' ");
            sb.Append(" ) V");
            sb.Append(" ORDER BY  V.DInvCode,V.subInvCode ");

            dtBom = UFSelect(sb.ToString());
            return dtBom;

        }


        /// <summary>
        /// 递归BOM
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="dt"></param>
        private void GetAllChildAndSubByInvCode(ref DataSet ds, DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataTable dtChild = new DataTable();
                dtChild = GetChildAndSubByInvcode(dt.Rows[i]["subInvCode"].ToString());
                if (dtChild.Rows.Count > 0)
                {
                    dtChild.TableName = "Child_" + Guid.NewGuid().ToString();
                    ds.Tables.Add(dtChild.Copy());
                    GetAllChildAndSubByInvCode(ref ds, dtChild);
                }
            }
        }


        /// <summary>
        /// 获取子件及替代料
        /// </summary>
        /// <param name="cInvcode"></param>
        /// <returns></returns>
        private DataTable GetChildAndSubByInvcode(string cInvcode)
        {
            DataTable dt = new DataTable();
            StringBuilder sb = new StringBuilder();

            sb.Append(" SELECT V.* FROM  ");
            sb.Append(" ( ");
            sb.Append("     SELECT  ");
            sb.Append("           h.InvCode,h.InvName,h.version,  ");
            sb.Append("           D.DInvCode,D.DInvCode AS subInvCode,d.DEffBegDate,d.DEffEndDate,d.DInvUnit ,d.DBaseQtyD,d.OpComponentId   ");
            sb.Append("     FROM v_bom_head h  with(nolock) ");
            sb.Append("           LEFT JOIN v_bom_detail d  with(nolock) ON h.bomid = d.bomid  ");
            sb.Append("     WHERE  H.InvCode ='" + cInvcode + "' ");
            sb.Append("     UNION ALL ");
            sb.Append("     SELECT ");
            sb.Append("           h.InvCode,h.InvName,h.version, ");
            sb.Append("           D.DInvCode,rm.invcode AS subInvCode,d.DEffBegDate,d.DEffEndDate,d.DInvUnit ,d.DBaseQtyD,d.OpComponentId  ");
            sb.Append("     FROM v_bom_head h with(nolock) ");
            sb.Append("           INNER JOIN v_bom_detail d with(nolock) ON h.bomid = d.bomid  ");
            sb.Append("           INNER JOIN bom_opcomponentsub r with(nolock) ON d.opcomponentid = r.opcomponentid ");
            sb.Append("           INNER JOIN bas_part rm with(nolock) ON r.PartId = rm.PartId ");
            sb.Append("           WHERE  H.InvCode ='" + cInvcode + "' AND ISNULL(rm.InvCode,'') <> '' ");
            sb.Append(" ) V");
            sb.Append(" ORDER BY  V.DInvCode,V.subInvCode ");

            dt = UFSelect(sb.ToString());

            return dt;
        }
        #endregion


        #region 组织  定位符
         
        /// <summary>
        /// 组织  定位符原数据
        /// </summary>
        /// <returns></returns>
        private DataTable GetLocation(int bomId)
        {
            DataTable dt = new DataTable();
            StringBuilder sb = new StringBuilder();

            sb.Append(" SELECT   a.OpComponentId,     loc=(select ' '+loc from bom_opcomponentloc b where b.OpComponentId=a.OpComponentId for xml path('')) ");
            sb.Append(" FROM bom_opcomponent with(nolock) ");
            sb.Append(" INNER JOIN bom_opcomponentloc a with(nolock) on a.OpComponentId = bom_opcomponent.OpComponentId ");
            sb.Append(" where bom_opcomponent.BomId = " + bomId + "");
            dt = UFSelect(sb.ToString());

            return dt;
        }

        /// <summary>
        /// 格式化定位符
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="OpComponentId"></param>
        /// <returns></returns>
        private string GetLocation(DataTable dt, string OpComponentId)
        {
            string res="";
            DataRow[] drArr = dt.Select("OpComponentId='" + OpComponentId + "'");
            for (int i = 0; i < drArr.Length; i++)
            {
                string tmpRes = drArr[i]["loc"].ToString();
                if (!string.IsNullOrEmpty(tmpRes))
                {
                    res += " " + tmpRes;
                }
            }
            return res;
        }


 
          
        #endregion




    }
}
