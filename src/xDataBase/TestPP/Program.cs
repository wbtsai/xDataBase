using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using Yuanta.xDataBase;
using Yuanta.xDataBase.Mapping;

namespace TestPP
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test1();
            //Test2();
            //Test3();
            //Test4();

            //Test ParserCT
            
            Test5();

            
        }

        private static void Test5()
        {
            
        }

        private static Parser<string> satisfy(Func<string, int, bool> condition)
        { 
            Func<string[]> expectedChars=()=>{
                var cs=new List<string>();
                for(var i=32;i<=126;i++)
                {
                    var c=char(i);
                    if(condition(c,i))
                    {
                        cs.Add(c);
                    }
                }
                return cs.ToArray();
            };

            Func<State,Reply<string>> satisfyParser=s=>{
                if(s.Position<s.Source.Length)
                {
                    var c=s.Source[s.Position];
                    var i=(int)s.Source[s.Position];
                }

                return new Reply<string>(new State("HH",0,null),true,"2",()=>{return "OK";});
            };

            return new Parser<string>(satisfyParser);
        }

        private Reply<A> Parse<A>(Parser<A> parser,State s)
        {
            return parser(s);
        }

        private static void Test3()
        {
            DataBase db = new DataBase("SBK");

            Test3Class a = new Test3Class() { idno = "L120678791" };

            DataTable dt = db.DoQuery(() =>
            {
                return "select feclientidno,feclientname from feclient where feclientidno=:idno";
            }, dic =>
            {
                dic.ToMappingParameter(a);
                //dic.Add("idno", "L120678791");
            });

            List<TestClass> list = dt.ToMapping<TestClass>();

            foreach (TestClass c in list)
            {
                string aa = c.ID;
            }
        }

        static void Test1()
        {
            DataBase db= new DataBase("SBK");

            DataTable dt=db.DoQuery(() =>
            {
                return "select feclientidno,feclientname from feclient where feclientidno=:idno";
            }, dic => 
            {
                dic.Add("idno", "L120678791");
            });

            List<TestClass> list = dt.ToMapping<TestClass>();
            
            foreach (TestClass c in list)
            {
                string aa =c.ID;
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

        static void Test4()
        {
            //做一個更新的Case
            DataBase db = new DataBase("SBK");

            db.DoTransaction(false, () =>
            {
                //更新資料
                db.DoCommand(() =>
                {
                    return @"grant select on tmp_feclienttrade_0922 to ibt";
                });
            });
        }
    }

    public class TestClass
    {
        [ColumnName(ColumnName="feclientidno")]
        public string ID { get; set; }
       [ColumnName(ColumnName = "feclientname")]
        public string Name { get; set; }
    }

    public class Test3Class
    {
        public string idno { get; set; }
    }
}
