﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using Yuanta.xDataBase;

namespace TestPP
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
            Test2();
        }

        static void Test1()
        {
            DataBase db= new DataBase("SBK");

            DataTable dt=db.DoQuery(() =>
            {
                return "select * from feclient where feclientidno=:idno";
            }, dic => 
            {
                dic.Add("idno", "L120678791");
            });

            foreach (DataRow row in dt.Rows)
            {
                string aa = row["feclientidno"].ToString();
            }
        }

        static void Test2()
        { 
            //做一個更新的Case
            DataBase db = new DataBase("SBK");

            db.DoTransaction(true,()=> {

                //更新資料
                db.DoCommand(() => 
                {
                    return @"update tmp_feclienttrade_0922 
                             set fecontactname=:pName 
                             where feclientid=:pId";
                
                }, dic =>
                {
                    dic.Add("pId", "09361410");
                    dic.Add("pName", "取號測試");
                });
            });
        }
    }
}