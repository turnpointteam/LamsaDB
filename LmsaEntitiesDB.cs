using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace EBEWebForm.Models
{
    //Use This Interface When You Calculate Quantity in ExcutionOrder
    public interface LmsaCalcQuntiy
    {
        string CalcQuntity(IEnumerable<LmsaEntitiesDB.Store> lst);

        LmsaEntitiesDB.Branch Card_CalcQuntity(IEnumerable<LmsaEntitiesDB.Branch> lst);
        string Balance_CalcQuntity(IEnumerable<LmsaEntitiesDB.Store> lst);
    }
    public class LmsaEntitiesDB
    {
        public static string ConvertDate(string date)
        {
            //string bbsget;
            string sadater;
            //string stimer;
            //DateTime utcTime = DateTime.UtcNow;
            //TimeZoneInfo myZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            //DateTime custDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, myZone);
            //bbsget = custDateTime.ToString("HH:mm");
            string jj = DateTime.Parse(date).Year + "-" + DateTime.Parse(date).Month + "-" + DateTime.Parse(date).Day;
            sadater = "Cast(CONVERT(varchar,'" + jj + "', 102) AS DATETIME)";
            //stimer = bbsget;
            return sadater;
        }
        private static List<T> ConvertDT<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        if (dr[column.ColumnName] == DBNull.Value)
                            pro.SetValue(obj, null, null);
                        else
                            pro.SetValue(obj, dr[column.ColumnName], null);
                    }
                    else
                        continue;
                }
            }
            return obj;
        }

        public class Store
        {
            private ConnectDataBase.ConnectFunction fun = new ConnectDataBase.ConnectFunction();
            private List<Store> storelst;
            public Store()
            {
            }
            public int ID { set; get; }
            public string Name { set; get; }
            public int UserID { set; get; }
            public int BranchID { set; get; }

            public int StoreID;
            public void maxid()
            {
                try
                {
                    StoreID = int.Parse(fun.FireSql("select max(ID) from Store ").ToString()) + 1;
                }
                catch (Exception ex)
                {
                    StoreID = 1;
                }

            }
            public object Operations(string method, object content = null)
            {
                if (method == "Add")
                {
                    storelst = (List<Store>)content;
                    if (storelst.Count > 0)
                    {
                        foreach (var it in storelst)
                        {
                            DataTable dtexist = fun.GetData("select * from store where name=N'" + it.Name + "'");
                            if (dtexist.Rows.Count > 0)
                            {
                                return 1003; //Exist
                            }
                            fun.FireSql("insert  into Store(Name,UserID,BranchID)values(N'" + it.Name + "'," + it.UserID + "," + it.BranchID + ")");
                        }
                        return 1001; //Success
                    }
                }
                else if (method == "Edit")
                {
                    storelst = (List<Store>)content;
                    if (storelst.Count > 0)
                    {
                        try
                        {
                            foreach (var row in storelst)
                            {
                                string update = "update Store set Name=N'" + row.Name + "',UserID=" + row.UserID + ",BranchID=" + row.BranchID + " where ID=" + row.ID + "";
                                fun.FireSql(update);
                            }
                            return 1001;//Success
                        }
                        catch
                        {
                            return 1002;// Failed
                        }
                    }



                }
                else if (method == "Delete")
                {
                    storelst = (List<Store>)content;
                    if (storelst.Count > 0)
                    {
                        fun.FireSql("delete from Store where ID=" + storelst.First().ID + "");
                        return 1001;//successs
                    }
                }
                return 1009;

            }

            public List<Store> Get_AllStore()
            {
                string getall = "select * from store";
                DataTable allstore = fun.GetData(getall);
                if (allstore != null)
                    return ConvertDT<Store>(allstore);
                return 1008;
            }

            public Store Get_StoreByID(int ID)
            {
                string getall = "select * from store where ID=" + ID + "";
                DataTable allstore = fun.GetData(getall);
                if (allstore != null)
                    return ConvertDT<Store>(allstore).FirstOrDefault();
                else return 1008;
            }

            public List<Store> Get_AllStoreByBranch(int Branch_ID)
            {
                string getall = "select * from store where BranchID=" + Branch_ID + "";
                DataTable allstore = fun.GetData(getall);
                if (allstore != null)
                    return ConvertDT<Store>(allstore);
                return 1008;
            }
            public List<Store> Get_AllStoreByStoreName(string Store_Name)
            {
                string getall = "select * from store where Name=N'%" + Store_Name + "%'";
                DataTable allstore = fun.GetData(getall);
                if (allstore != null)
                    return ConvertDT<Store>(allstore);
                else return 1008;
            }
            public List<Store> Get_AllStoreByUser(int User_ID)
            {
                string getall = "select * from store where BranchID=" + User_ID + "";
                DataTable allstore = fun.GetData(getall);
                return ConvertDT<Store>(allstore);
            }
        }

        public class Branch
        {
            private ConnectDataBase.ConnectFunction fun = new ConnectDataBase.ConnectFunction();
            private List<Branch> branchlst;

            public int ID { set; get; }
            public string Name { set; get; }
            public string Address { set; get; }
            public string Phone { set; get; }
            public int UserID { set; get; }

            public Store store { set; get; }
            public int BranchID;
            public Branch()
            {
                store = new Store();
            }
            public void maxid()
            {
                try
                {
                    BranchID = int.Parse(fun.FireSql("select max(ID) from Branch ").ToString()) + 1;
                }
                catch (Exception ex)
                {
                    BranchID = 1;
                }

            }
            public object Operations(string method, object content = null)
            {
                if (method == "Add")
                {
                    branchlst = (List<Branch>)content;
                    if (branchlst.Count > 0)
                    {
                        try
                        {

                            foreach (var it in branchlst)
                            {
                                DataTable dtexist = fun.GetData("select * from Branch where name=N'" + it.Name + "'");
                                if (dtexist.Rows.Count > 0)
                                {
                                    return 1003;//Exist
                                }
                                dtexist = fun.GetData("select * from Branch where ID=N'" + it.ID + "'");
                                if (dtexist.Rows.Count > 0)
                                {
                                    return 1003;//Exist
                                }
                                fun.FireSql("insert  into Branch(Name,Address,Phone,UserID)values(N'" + it.Name + "',N'" + it.Address + "',N'" + it.Phone + "'," + it.UserID + ")");
                            }

                            return 1001;//Success
                        }
                        catch
                        {
                            return 1002;
                        }

                    }
                }
                else if (method == "Edit")
                {
                    branchlst = (List<Branch>)content;
                    if (branchlst.Count > 0)
                    {
                        try
                        {

                        
                        foreach (var row in branchlst)
                        {
                            string update = "update Branch set Name=N'" + row.Name + "',Address=N'" + row.Address + "',Phone=N'" + row.Phone + "',UserID=" + row.UserID + " where ID=" + row.ID + "";
                            fun.FireSql(update);
                        }
                        return 1001;
                        }
                        catch
                        {
                            return 1002;
                        }
                    }
                }
                else if (method == "Delete")
                {
                    branchlst = (List<Branch>)content;
                    if (branchlst.Count > 0)
                    {
                        fun.FireSql("delete from Branch where ID=" + branchlst.First().ID + "");
                        return 1001;
                    }
                }
                return 1009;
            }

            public List<Branch> Get_AllBranches()
            {
                string getall = "select * from Branch";
                DataTable allbranches = fun.GetData(getall);

                if(allbranches!=null)
                    return ConvertDT<Branch>(allbranches);
                return 1008;
            }

            public Branch Get_BranchByID(int ID)
            {
                string getall = "select * from Branch where ID=" + ID + "";
                DataTable allBranch = fun.GetData(getall);
                if(allBranch != null)
                    return ConvertDT<Branch>(allBranch).FirstOrDefault();
                return 1008;
            }

            public List<Store> Get_AllStoreByBranch(int Branch_ID)
            {
                string getall = "select * from store where BranchID=" + Branch_ID + "";
                DataTable allstore = fun.GetData(getall);
                if (allstore != null)
                    return ConvertDT<Store>(allstore);
                return 1008;
            }
            public List<Branch> Get_AllBranchesByBrancheName(string Branch_Name)
            {
                string getall = "select * from Branch where Name=N'%" + Branch_Name + "%'";
                DataTable allBranch = fun.GetData(getall);
                if(allBranch!=null)
                    return ConvertDT<Branch>(allBranch);
                return 1008;

            }
            public List<Branch> Get_AllBranchesByUser(int User_ID)
            {
                string getall = "select * from Branch where UserID=" + User_ID + "";
                DataTable allBranches = fun.GetData(getall);
                if (allBranches != null)
                    return ConvertDT<Branch>(allBranches);
                return 1008;
            }
        }

        public class Privilage
        {
            private ConnectDataBase.ConnectFunction fun = new ConnectDataBase.ConnectFunction();
            private List<Privilage> branchlst;

            public int ID { set; get; }
            public string Title { set; get; }
         
            public int PrivilageID;
            public Privilage()
            {
            }
            public void maxid()
            {
                try
                {
                    PrivilageID = int.Parse(fun.FireSql("select max(ID) from Privilage").ToString()) + 1;
                }
                catch (Exception ex)
                {
                    PrivilageID = 1;
                }

            }
            public object Operations(string method, object content = null)
            {
                if (method == "Add")
                {
                    Privilagehlst = (List<Privilage>)content;
                    if (Privilagehlst.Count > 0)
                    {
                        try
                        {

                            foreach (var it in Privilagehlst)
                            {
                                DataTable dtexist = fun.GetData("select * from Privilage where Title=N'" + it.Title + "'");
                                if (dtexist.Rows.Count > 0)
                                {
                                    return 1003;//Exist
                                }
                                fun.FireSql("insert  into Privilage(Title)values(N'" + it.Title +")");
                            }

                            return 1001;//Success
                        }
                        catch
                        {
                            return 1002;
                        }

                    }
                }
                else if (method == "Edit")
                {
                    Privilagehlst = (List<Privilage>)content;
                    if (Privilagehlst.Count > 0)
                    {
                        try
                        {


                            foreach (var row in Privilagehlst)
                            {
                                string update = "update Privilage set Title=N'" + row.Title+ "' where ID=" + row.ID + "";
                                fun.FireSql(update);
                            }
                            return 1001;
                        }
                        catch
                        {
                            return 1002;
                        }
                    }
                }
                else if (method == "Delete")
                {
                    Privilagehlst = (List<Privilage>)content;
                    if (Privilagehlst.Count > 0)
                    {
                        fun.FireSql("delete from Privilage where ID=" + Privilagehlst.First().ID + "");
                        return 1001;
                    }
                }
                return 1009;
            }

            public List<Privilage> Get_AllPrivilages()
            {
                
                string getall = "select * from Privilage";
                DataTable allPrivilage = fun.GetData(getall);
                if (allPrivilage != null)
                    return ConvertDT<Privilage>(allPrivilage);
                return 1008;
            }

            public Privilage Get_PrivilageByID(int ID)
            {
                string getall = "select * from Privilage where ID=" + ID + "";
                DataTable allPrivilage = fun.GetData(getall);
                if (allPrivilage != null)
                    return ConvertDT<Privilage>(allPrivilage).FirstOrDefault();
                return 1008;
            }

           
            
           
        }

        public class Role
        {
            private ConnectDataBase.ConnectFunction fun = new ConnectDataBase.ConnectFunction();
            private List<Role> Rolelist;
            public int ID { set; get; }
            public string Name { set; get; }

            public int RoleID;
            public Role()
            {
            }
            public void maxid()
            {
                try
                {
                    RoleID = int.Parse(fun.FireSql("select max(ID) from Role").ToString()) + 1;
                }
                catch (Exception ex)
                {
                    RoleID = 1;
                }

            }
            public object Operations(string method, object content = null)
            {
                if (method == "Add")
                {
                    Rolelist = (List<Role>)content;
                    if (Rolelist.Count > 0)
                    {
                        try
                        {
                            foreach (var it in Rolelist)
                            {
                                DataTable dtexist = fun.GetData("select * from Role where Name=N'" + it.Name + "'");
                                if (dtexist.Rows.Count > 0)
                                {
                                    return 1003;//Exist
                                }
                                fun.FireSql("insert  into Role(Name)values(N'" + it.Name + ")");
                            }

                            return 1001;//Success
                        }
                        catch
                        {
                            return 1002;
                        }

                    }
                }
                else if (method == "Edit")
                {
                    Rolelist = (List<Role>)content;
                    if (Rolelist.Count > 0)
                    {
                        try
                        {
                            foreach (var row in Rolelist)
                            {
                                string update = "update Role set Name=N'" + row.Name + "' where ID=" + row.ID + "";
                                fun.FireSql(update);
                            }
                            return 1001;
                        }
                        catch
                        {
                            return 1002;
                        }
                    }
                }
                else if (method == "Delete")
                {
                    Rolelist = (List<Role>)content;
                    if (Rolelist.Count > 0)
                    {
                        fun.FireSql("delete from Role where ID=" + Rolelist.First().ID + "");
                        return 1001;
                    }
                }
                return 1009;
            }
            public List<Role> Get_AllRoles()
            {

                string getall = "select * from Role";
                DataTable allRoles = fun.GetData(getall);
                if (allRoles != null)
                    return ConvertDT<Role>(allRoles);
                return 1008;
            }
            public Role Get_RoleByID(int ID)
            {
                string getall = "select * from Role where ID=" + ID;
                DataTable allRoles = fun.GetData(getall);
                if (allRoles != null)
                    return ConvertDT<Role>(allRoles).FirstOrDefault();
                return 1008;
            }
        }

        public class RolePrivlageFT
        {
            private ConnectDataBase.ConnectFunction fun = new ConnectDataBase.ConnectFunction();
            private List<RolePrivlageFT> RolePrivlist;
            public int ID { set; get; }
            public int RoleID { set; get; }
            public int PrivilageID { set; get; }


            public int RolePrivID;
            public RolePrivlageFT()
            {
            }
            public void maxid()
            {
                try
                {
                    RolePrivID = int.Parse(fun.FireSql("select max(ID) from RolePrivlageFT").ToString()) + 1;
                }
                catch (Exception ex)
                {
                    RoleID = 1;
                }

            }
            public object Operations(string method, object content = null)
            {
                if (method == "Add")
                {
                    RolePrivlist = (List<RolePrivlageFT>)content;
                    if (RolePrivlist.Count > 0)
                    {
                        try
                        {
                            foreach (var it in RolePrivlist)
                            {
                                DataTable dtexist = fun.GetData("select * from RolePrivlageFT where ( RoleID="+it.RoleID+ " and  PrivilageID=" + it.PrivilageID +")");
                                if (dtexist.Rows.Count > 0)
                                {
                                    return 1003;//Exist
                                }
                                fun.FireSql("insert  into RolePrivlageFT(RoleID,PrivilageID)values("+it.RoleID+","+it.PrivilageID+")");
                            }
                            return 1001;//Success
                        }
                        catch
                        {
                            return 1002;
                        }
                    }
                }
                else if (method == "Edit")
                {
                    RolePrivlist = (List<RolePrivlageFT>)content;
                    if (RolePrivlist.Count > 0)
                    {
                        try
                        {
                            foreach (var row in RolePrivlist)
                            {
                                string update = "update RolePrivlageFT set RoleID=" + row.RoleID + ",PrivilageID=" + row.PrivilageID + " where ID=" + row.ID + "";
                                fun.FireSql(update);
                            }
                            return 1001;
                        }
                        catch
                        {
                            return 1002;
                        }
                    }
                }
                else if (method == "Delete")
                {
                    RolePrivlist = (List<RolePrivlageFT>)content;
                    if (RolePrivlist.Count > 0)
                    {
                        fun.FireSql("delete from RolePrivlageFT where ID=" + RolePrivlist.First().ID + "");
                        return 1001;
                    }
                }
                return 1009;
            }
            public List<RolePrivlageFT> Get_AllRelationByRole(int ID)
            {

                string getall = "select * from RolePrivlageFT where RoleID ="+ID;
                DataTable allRelation = fun.GetData(getall);
                if (allRelation != null)
                    return ConvertDT<RolePrivlageFT>(allRelation);
                return 1008;
            }
            public List<RolePrivlageFT> Get_AllRelationByPrivlage(int ID)
            {

                string getall = "select * from RolePrivlageFT where PrivilageID =" + ID;
                DataTable allRelation = fun.GetData(getall);
                if (allRelation != null)
                    return ConvertDT<RolePrivlageFT>(allRelation);
                return 1008;
            }
            public Role Get_RelationByID(int ID)
            {
                string getall = "select * from RolePrivlageFT where ID=" + ID + "";
                DataTable allRelation = fun.GetData(getall);
                if (allRelation != null)
                    return ConvertDT<Role>(allRelation).FirstOrDefault();
                return 1008;
            }
        }

        public class User
        {
            private ConnectDataBase.ConnectFunction fun = new ConnectDataBase.ConnectFunction();
            private List<User> Userlist;
            public Role Role { get; set; }

            public int ID { set; get; }
            public string User_Name { set; get; }
            public string Password { set; get; }
            public string Email { set; get; }
            public string Phone { set; get; }
            public int RoleID { set; get; }

            public int UserID;

            public User()
            {
            }
            public void maxid()
            {
                try
                {
                    UserID = int.Parse(fun.FireSql("select max(ID) from User ").ToString()) + 1;
                }
                catch (Exception ex)
                {
                    UserID = 1;
                }

            }
            public object Operations(string method, object content = null)
            {
                if (method == "Add")
                {
                    Userlist = (List<User>)content;
                    if (Userlist.Count > 0)
                    {
                        foreach (var it in Userlist)
                        {
                            DataTable dtexist = fun.GetData("select * from User where Email=N'" + it.Email + "' OR Phone=N'"=it.Phone+"'");
                            if (dtexist.Rows.Count > 0)
                            {
                                return 1003; //Exist
                            }
                            fun.FireSql("insert  into User(Username,Password,Email,Phone,RoleID)values(N'" + it.User_Name + "',N'" + it.Password + "'," + it.Email + ",N'"+it.Phone+"',"+it.RoleID+")");
                        }
                        return 1001; //Success
                    }
                }
                else if (method == "Edit")
                {
                    Userlist = (List<User>)content;
                    if (Userlist.Count > 0)
                    {
                        try
                        {
                            foreach (var row in Userlist)
                            {
                                string update = "update User set Username=N'" + row.User_Name + "',Password=N'" + row.Password + "',Email=N'" + row.Email + "' Phone=N'" + row.Phone+ ",RoleID="+row.RoleID+" where ID=" + row.ID + "";
                                fun.FireSql(update);
                            }
                            return 1001;//Success
                        }
                        catch
                        {
                            return 1002;// Failed
                        }
                    }
                }
                else if (method == "Delete")
                {
                    Userlist = (List<User>)content;
                    if (Userlist.Count > 0)
                    {
                        fun.FireSql("delete from User where ID=" + Userlist.First().ID + "");
                        return 1001;//successs
                    }
                }
                return 1009;
            }
            public List<User> Get_AllUser()
            {
                string getall = "select * from User";
                DataTable allUser = fun.GetData(getall);
                if (allUser != null)
                    return ConvertDT<User>(allUser);
                return 1008;
            }
            public User Get_UserByID(int ID)
            {
                string getall = "select * from User where ID=" + ID + "";
                DataTable allUser = fun.GetData(getall);
                if (allUser != null)
                    return ConvertDT<User>(allUser).FirstOrDefault();
                else return 1008;
            }
            public List<User> Get_AllUsersByName(string User_Name)
            {
                string getall = "select * from User where Username=N'%" + User_Name + "%'";
                DataTable allUser = fun.GetData(getall);
                if (allUser != null)
                    return ConvertDT<User>(allUser);
                else return 1008;
            }
            public List<User> Get_AllUserByRole(int Role_ID)
            {
                string getall = "select * from User where RoleID=" + Role_ID;
                DataTable allUser = fun.GetData(getall);
                return ConvertDT<User>(allUser);
            }
        }

        public class MainAccount
        {
            private ConnectDataBase.ConnectFunction fun = new ConnectDataBase.ConnectFunction();
            private List<MainAccount> MainAccountlist;
            public User User { get; set; }

            public long ID { set; get; }
            public string Name { set; get; }
            public long UpAccount { set; get; }
            public int Level { set; get; }
            public int UserID { set; get; }

            public int MainAccountID;

            public MainAccount()
            {
            }
            public void maxid()
            {
                try
                {
                    MainAccountID = int.Parse(fun.FireSql("select max(ID) from MainAccount ").ToString()) + 1;
                }
                catch (Exception ex)
                {
                    UserID = 1;
                }

            }
            public object Operations(string method, object content = null)
            {
                if (method == "Add")
                {
                    MainAccountlist = (List<MainAccount>)content;
                    if (MainAccountlist.Count > 0)
                    {
                        foreach (var it in MainAccountlist)
                        {
                            DataTable dtexist = fun.GetData("select * from MainAccount where Name=N'" + it.Name +"'");
                            if (dtexist.Rows.Count > 0)
                            {
                                return 1003; //Exist
                            }
                            fun.FireSql("insert  into MainAccount(Name,UpAccount,Level,UserID)values(N'" + it.Name+"',"+it.UpAccount+","+it.Level+","+it.UserID+")");
                        }
                        return 1001; //Success
                    }
                }
                else if (method == "Edit")
                {
                    MainAccountlist = (List<MainAccount>)content;
                    if (MainAccountlist.Count > 0)
                    {
                        try
                        {
                            foreach (var row in MainAccountlist)
                            {
                                string update = "update MainAccount set Name=N'" + row.Name + "',UpAccount=" + row.UpAccount + ",Level=" + row.Level + "UserID=" + row.UserID + "where ID=" + row.ID;
                                fun.FireSql(update);
                            }
                            return 1001;//Success
                        }
                        catch
                        {
                            return 1002;// Failed
                        }
                    }
                }
                else if (method == "Delete")
                {
                    MainAccountlist = (List<User>)content;
                    if (MainAccountlist.Count > 0)
                    {
                        fun.FireSql("delete from MainAccount where ID=" + MainAccountlist.First().ID);
                        return 1001;//successs
                    }
                }
                return 1009;
            }
            public List<MainAccount> Get_AllMainAccount()
            {
                string getall = "select * from MainAccount";
                DataTable allMainAccount = fun.GetData(getall);
                if (allMainAccount != null)
                    return ConvertDT<MainAccount>(allMainAccount);
                return 1008;
            }
            public MainAccount Get_MainAccountByID(int ID)
            {
                string getall = "select * from MainAccount where ID=" + ID;
                DataTable allMainAccount = fun.GetData(getall);
                if (allMainAccount != null)
                    return ConvertDT<MainAccount>(allMainAccount).FirstOrDefault();
                else return 1008;
            }
            public List<MainAccount> Get_MainAccountsByName(string MainAccount_Name)
            {
                string getall = "select * from MainAccount where Name=N'%" + MainAccount_Name + "%'";
                DataTable AllMainAccount = fun.GetData(getall);
                if (AllMainAccount != null)
                    return ConvertDT<MainAccount>(AllMainAccount);
                else return 1008;
            }
            public List<MainAccount> Get_AllMainAccountByUserID(int User_ID)
            {
                string getall = "select * from MainAccount where UserID=" + User_ID;
                DataTable allMainAccount = fun.GetData(getall);
                return ConvertDT<MainAccount>(allMainAccount);
            }
        }

        public class SubAccount
        {
            private ConnectDataBase.ConnectFunction fun = new ConnectDataBase.ConnectFunction();
            private List<SubAccount> SubAccountlist;
            public User user { get; set; }
            public MainAccount MainAccount { get; set; }

            public int ID { set; get; }
            public string Name { set; get; }
            public long UpAccount { set; get; }
            public int Level { set; get; }
            public double Balance { set; get; }
            public string Type { set; get; }
            public string RegisterDate { set; get; }
            public int UserID { set; get; }
            public long MainAccount { set; get; }

            public int SubAccountID;
            public SubAccount()
            {
            }
            public void maxid()
            {
                try
                {
                    SubAccountID = int.Parse(fun.FireSql("select max(ID) from SubAccount").ToString()) + 1;
                }
                catch (Exception ex)
                {
                    SubAccountID = 1;
                }

            }
            public object Operations(string method, object content = null)
            {
                if (method == "Add")
                {
                    SubAccountlist = (List<SubAccount>)content;
                    if (SubAccountlist.Count > 0)
                    {
                        try
                        {
                            foreach (var it in SubAccountlist)
                            {
                                DataTable dtexist = fun.GetData("select * from SubAccount where Name=N'" + it.Name + "'");
                                if (dtexist.Rows.Count > 0)
                                {
                                    return 1003;//Exist
                                }
                                fun.FireSql("insert  into SubAccount(Name,UpAccount,Level,Balance,Type,RegisterDate,UserID,MainAccount)values(N'" + it.Name + "'," + it.UpAccount + "," + it.Level + "," + it.Balance + ",N'"+ it.Type + "',N'"+ it.RegisterDate + "',"+ it.UserID +"," + it.UserID + ")");
                            }
                            return 1001;//Success
                        }
                        catch
                        {
                            return 1002;
                        }

                    }
                }
                else if (method == "Edit")
                {
                    SubAccountlist = (List<SubAccount>)content;
                    if (SubAccountlist.Count > 0)
                    {
                        try
                        {
                            foreach (var row in SubAccountlist)
                            {
                                string update = "update SubAccount set Name=N'" + row.Name + "',UpAccount=" + row.UpAccount + ",Level=" + row.Level+ ",Balance="+row.Balance+",Type = N'"+row.Type+ "',RegisterDate = N'" + row.RegisterDate + "',UserID="+ row.UserID + ",MainAccount=" + row.MainAccount + "where ID=" + row.ID;
                                fun.FireSql(update);
                            }
                            return 1001;
                        }
                        catch
                        {
                            return 1002;
                        }
                    }
                }
                else if (method == "Delete")
                {
                    SubAccountlist = (List<SubAccount>)content;
                    if (SubAccountlist.Count > 0)
                    {
                        fun.FireSql("delete from SubAccount where ID=" + SubAccountlist.First().ID);
                        return 1001;
                    }
                }
                return 1009;
            }
            public List<SubAccount> Get_AllSubAccount()
            {

                string getall = "select * from SubAccount";
                DataTable allSubAccount = fun.GetData(getall);
                if (allSubAccount != null)
                    return ConvertDT<SubAccount>(allSubAccount);
                return 1008;
            }
            public SubAccount Get_SubAccountByID(int ID)
            {
                string getall = "select * from SubAccount where ID=" + ID;
                DataTable allSubAccount = fun.GetData(getall);
                if (allSubAccount != null)
                    return ConvertDT<SubAccount>(allSubAccount).FirstOrDefault();
                else return 1008;
            }
            public List<SubAccount> Get_SubAccountsByName(string SubAccount_Name)
            {
                string getall = "select * from SubAccount where Name=N'%" + SubAccount_Name + "%'";
                DataTable AllSubAccount = fun.GetData(getall);
                if (AllSubAccount != null)
                    return ConvertDT<SubAccount>(AllSubAccount);
                else return 1008;
            }
            public List<SubAccount> Get_AllSubAccountByUserID(int User_ID)
            {
                string getall = "select * from SubAccount where UserID=" + User_ID;
                DataTable allSubAccount = fun.GetData(getall);
                return ConvertDT<SubAccount>(allSubAccount);
            }
            public List<SubAccount> Get_AllSubAccountByMainAccount(int MainAccount)
            {
                string getall = "select * from SubAccount where MainAccount=" + MainAccount;
                DataTable allSubAccount = fun.GetData(getall);
                return ConvertDT<SubAccount>(allSubAccount);
            }
        }
         
        public class Indx
        {
            private ConnectDataBase.ConnectFunction fun = new ConnectDataBase.ConnectFunction();
            private List<Indx> Indxlist;

            public int ID { set; get; }
            public string Email { set; get; }
            public string Phone { set; get; }
            public string Address { set; get; }
            public long SubID { set; get; }

            public int IndxID;
            public Indx()
            {
            }
            public void maxid()
            {
                try
                {
                    IndxID = int.Parse(fun.FireSql("select max(ID) from Indx").ToString()) + 1;
                }
                catch (Exception ex)
                {
                    IndxID = 1;
                }

            }
            public object Operations(string method, object content = null)
            {
                if (method == "Add")
                {
                    Indxlist = (List<Indx>)content;
                    if (Indxlist.Count > 0)
                    {
                        try
                        {
                            foreach (var it in Indxlist)
                            {
                                DataTable dtexist = fun.GetData("select * from Indx where Email=N'"+it.Email +"' OR Phone=N'"+ it.Phone+"'");
                                if (dtexist.Rows.Count > 0)
                                {
                                    return 1003;//Exist
                                }
                                fun.FireSql("insert  into Role(Email,Phone,Address)values(N'"+it.Email+"',N'"+it.Phone+"',N'"+it.Address+"'");
                            }
                            return 1001;//Success
                        }
                        catch
                        {
                            return 1002;
                        }

                    }
                }
                else if (method == "Edit")
                {
                    Indxlist = (List<Indx>)content;
                    if (Indxlist.Count > 0)
                    {
                        try
                        {
                            foreach (var row in Indxlist)
                            {
                                string update = "update Indx set Email=N'"+row.Email+"',Phone=N'"+row.Phone+"'Address=N'"+row.Address+"' where ID=" + row.ID ;
                                fun.FireSql(update);
                            }
                            return 1001;
                        }
                        catch
                        {
                            return 1002;
                        }
                    }
                }
                else if (method == "Delete")
                {
                    Indxlist = (List<Indx>)content;
                    if (Indxlist.Count > 0)
                    {
                        fun.FireSql("delete from Indx where ID=" + Indxlist.First().ID);
                        return 1001;
                    }
                }
                return 1009;
            }
            public List<Indx> Get_AllIndx()
            {

                string getall = "select * from Indx";
                DataTable allIndx = fun.GetData(getall);
                if (allIndx != null)
                    return ConvertDT<Indx>(allIndx);
                return 1008;
            }
            public Indx Get_IndxByID(int ID)
            {
                string getall = "select * from Indx where ID=" + ID;
                DataTable allIndx = fun.GetData(getall);
                if (allIndx != null)
                    return ConvertDT<Indx>(allIndx).FirstOrDefault();
                return 1008;
            }
        }

        public class Order
        {
            private ConnectDataBase.ConnectFunction fun = new ConnectDataBase.ConnectFunction();
            private List<Order> Orderlist;
            public User User { get; set; }
            public SubAccount User { get; set; }


            public int ID { set; get; }
            public string Date { set; get; }
            public bool State { set; get; }
            public long SubID { set; get; }
            public int UserID { set; get; }
            public string ReceiptDate { set; get; }
            public string Palce { set; get; }
            public string Notes { set; get; }

            public int OrderID;
            public Order()
            {
            }
            public void maxid()
            {
                try
                {
                    OrderID = int.Parse(fun.FireSql("select max(ID) from Order ").ToString()) + 1;
                }
                catch (Exception ex)
                {
                    OrderID = 1;
                }

            }
            public object Operations(string method, object content = null)
            {
                if (method == "Add")
                {
                    Orderlist = (List<Order>)content;
                    if (Orderlist.Count > 0)
                    {
                        foreach (var it in Orderlist)
                        {
                            //DataTable dtexist = fun.GetData("select * from Order where Name=N'" + it.Name + "'");
                            //if (dtexist.Rows.Count > 0)
                            //{
                            //    return 1003; //Exist
                            //}
                            fun.FireSql("insert  into Order(Date,State,SubID,UserID,ReceiptDate,Palce,Notes)values(N'"+ it.Date+ "',"+it.State+"," + it.SubID + "," + it.UserID + ",N'"+it.ReceiptDate+"',N'"+it.Palce+"',N'"+it.Notes+"')");
                        }
                        return 1001; //Success
                    }
                }
                else if (method == "Edit")
                {
                    Orderlist = (List<Order>)content;
                    if (Orderlist.Count > 0)
                    {
                        try
                        {
                            foreach (var row in Orderlist)
                            {
                                string update = "update Order set Date=N'" + row.Date + "',State=" + row.State + ",SubID=" + row.SubID + ",UserID=" + row.UserID + ",ReceiptDate=N'"+row.ReceiptDate+"',Palce=N'"+row.Palce+"',Notes=N'"+row.Notes+"' where ID=" + row.ID;
                                fun.FireSql(update);
                            }
                            return 1001;//Success
                        }
                        catch
                        {
                            return 1002;// Failed
                        }
                    }
                }
                else if (method == "Delete")
                {
                    Orderlist = (List<Order>)content;
                    if (Orderlist.Count > 0)
                    {
                        fun.FireSql("delete from Order where ID=" + Orderlist.First().ID);
                        return 1001;//successs
                    }
                }
                return 1009;
            }
            public List<Order> Get_AllOrder()
            {
                string getall = "select * from Order";
                DataTable allOrder = fun.GetData(getall);
                if (allOrder != null)
                    return ConvertDT<Order>(allOrder);
                return 1008;
            }
            public Order Get_OrderByID(int ID)
            {
                string getall = "select * from Order where ID=" + ID;
                DataTable allOrder = fun.GetData(getall);
                if (allOrder != null)
                    return ConvertDT<Order>(allOrder).FirstOrDefault();
                else return 1008;
            }
            public List<Order> Get_OrderByDate(string Date)
            {
                string getall = "select * from Order where Date=N'" + Date+"'";
                DataTable allOrder = fun.GetData(getall);
                if (allOrder != null)
                    return ConvertDT<Order>(allOrder);
                else return 1008;
            }
            public List<Order> Get_OrderByState(bool State)
            {
                string getall = "select * from Order where State=" + State;
                DataTable allOrder = fun.GetData(getall);
                if (allOrder != null)
                    return ConvertDT<Order>(allOrder);
                else return 1008;
            }
            public List<Order> Get_OrderByReceiptDate(string Date)
            {
                string getall = "select * from Order where ReceiptDate=N'" + Date+"'";
                DataTable allOrder = fun.GetData(getall);
                if (allOrder != null)
                    return ConvertDT<Order>(allOrder);
                else return 1008;
            }
            public List<Order> Get_OrderByPalce(string Palce)
            {
                string getall = "select * from Order where Palce=N'%" + Palce + "%'";
                DataTable AllOrder = fun.GetData(getall);
                if (AllOrder != null)
                    return ConvertDT<Order>(AllOrder);
                else return 1008;
            }
            public List<Order> Get_AllOrderByUserID(int User_ID)
            {
                string getall = "select * from Order where UserID=" + User_ID;
                DataTable allOrder = fun.GetData(getall);
                return ConvertDT<Order>(allOrder);
            }
            public List<Order> Get_AllOrderBySubID(int SubID)
            {
                string getall = "select * from Order where SubID=" + SubID;
                DataTable allOrder = fun.GetData(getall);
                return ConvertDT<Order>(allOrder);
            }
        }
    }
}