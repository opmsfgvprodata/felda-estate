//using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC_SYSTEM.Class
{
    public class GetIdentity
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        public bool MySuperAdmin(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super Power Admin" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_MasterModels dc = new MVC_SYSTEM_MasterModels())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }
        
        public string MyName(string username)
        {
            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
            tblUser User;
            string Name;
            Name = "";
            User = db.tblUsers.Where(u => u.fldUserName.Equals(username)).FirstOrDefault();
            if (User != null)
            {
                Name = User.fldUserShortName.ToString();
            }

            return Name;
        }

        public string MyName2(int? userid)
        {
            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
            tblUser User;
            string Name;
            Name = "";
            User = db.tblUsers.Where(u => u.fldUserID == userid).FirstOrDefault();
            if (User != null)
            {
                Name = User.fldUserShortName.ToString();
            }

            return Name;
        }

        public string MyNameFullName(int? userid)
        {
            tblUser User;
            string Name;
            Name = "";
            User = db.tblUsers.Where(u => u.fldUserID == userid).FirstOrDefault();
            if (User != null)
            {
                Name = User.fldUserFullName.ToString();
            }

            return Name;
        }

        public string Username(int ID)
        {
            tblUser User;
            string Name;
            Name = "";
            User = db.tblUsers.Where(u => u.fldUserID.Equals(ID)).FirstOrDefault();
            if (User != null)
            {
                Name = User.fldUserName.ToString();
            }

            return Name;
        }

        public string Username2(int? ID)
        {
            tblUser User;
            string Name;
            Name = "";
            User = db.tblUsers.Where(u => u.fldUserID == ID).FirstOrDefault();
            if (User != null)
            {
                Name = User.fldUserName.ToString();
            }

            return Name;
        }

        public int ID(string username)
        {
            tblUser User;
            int ID = 0;
            User = db.tblUsers.Where(u => u.fldUserName.Equals(username)).FirstOrDefault();
            if (User != null)
            {
                ID = User.fldUserID;
            }

            return ID;
        }

        public int? ClientID(string username)
        {
            tblUser User;
            int? ID = 0;
            User = db.tblUsers.Where(u => u.fldUserName.Equals(username)).FirstOrDefault();
            if (User != null)
            {
                ID = User.fldClientID;
            }

            return ID;
        }

        //new identity function
        public bool SuperPowerAdmin(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super Power Admin" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_MasterModels dc = new MVC_SYSTEM_MasterModels())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool SuperAdmin(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super Admin" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_MasterModels dc = new MVC_SYSTEM_MasterModels())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool Admin1(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Admin 1" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_MasterModels dc = new MVC_SYSTEM_MasterModels())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool Admin2(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Admin 2", "Admin 3" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_MasterModels dc = new MVC_SYSTEM_MasterModels())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }
        public bool SuperPowerUser(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super Power User" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_MasterModels dc = new MVC_SYSTEM_MasterModels())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }
        public bool SuperUser(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super User" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_MasterModels dc = new MVC_SYSTEM_MasterModels())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool NormalUser(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Normal User" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_MasterModels dc = new MVC_SYSTEM_MasterModels())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool NegaraSumber(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Resource" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_MasterModels dc = new MVC_SYSTEM_MasterModels())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public int? getRoleID(int? id)
        {
            int? roleid = 0;

            roleid = db.tblUsers.Where(x => x.fldUserID == id).Select(s => s.fldRoleID).SingleOrDefault();

            return roleid;
        }

        public int? getKmplnSyrktID(string username)
        {
            int? KmplnSyrktID = 0;

            KmplnSyrktID = db.tblUsers.Where(u => u.fldUserName.Equals(username)).Select(s => s.fld_KmplnSyrktID).FirstOrDefault();//db.tblUsers.Where(x => x.fldUserID == id).Select(s => s.fldRoleID).SingleOrDefault();

            return KmplnSyrktID;
        }

        public string getCmpnyShortName(int getuserid, string username)
        {
            GetNSWL GetNSWL = new GetNSWL();
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            string cmpnyshortname = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, username);

            cmpnyshortname = db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_NamaPndkSyarikat).FirstOrDefault();

            return cmpnyshortname;
        }

        public bool getExistingUserIDApp(string UserID, string IC, long? fileid, int? ldgid, int? wlyhid, int? syrktid, int? ngraid)
        {
            bool result = false;

            var getworker = db.tblUserIDApps.Where(x => x.fldUserid == UserID && x.fldNoKP == IC && x.fldLadangID == ldgid && x.fldWilayahID == wlyhid && x.fldSyarikatID == syrktid && x.fldNegaraID == ngraid).Count();

            if (getworker > 1)
            {
                result = true;
            }

            return result;
        }

        public string estatename(int userid)
        {
            GetNSWL GetNSWL = new GetNSWL();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;

            string username = Username(userid);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, userid, username);
            //int? ldgid = db.tblUsers.Where(x => x.fldUserID == userid).Select(s => s.fldLadangID).FirstOrDefault();
            string ldgname = db.tbl_Ladang.Where(x => x.fld_ID == LadangID).Select(s => s.fld_LdgName).FirstOrDefault();
            return ldgname;
        }

        public int? RoleID(int? userid)
        {
            int? userRole = db.tblUsers.Where(x => x.fldUserID == userid).Select(s => s.fldRoleID).FirstOrDefault();
            return userRole;
        }

        public string getWilayahName(int userid)
        {
            int? wilayahid = db.tblUsers.Where(x => x.fldUserID == userid).Select(s => s.fldWilayahID).FirstOrDefault();
            string wilayahName = db.tbl_Wilayah.Where(x => x.fld_ID == wilayahid).Select(s => s.fld_WlyhName).FirstOrDefault();
            return wilayahName;
        }

        public bool EstateAdmin(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super Power Admin", "Super Admin", "Admin 1", "Admin 2", "Admin 3", "Super Power User" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_MasterModels dc = new MVC_SYSTEM_MasterModels())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool HQAuth(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super Power Admin", "Super Admin", "Admin 1", "Admin 2", "Admin 3" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_MasterModels dc = new MVC_SYSTEM_MasterModels())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public List<tblUser> GetListUser()
        {
            return db.tblUsers.ToList();
        }

        //public int? GetWilayahID(string username)
        //{
        //    int? wlyh = 0;
        //    var User = db.tblUsers.Where(u => u.fldUserName.Equals(username)).FirstOrDefault();
        //    if(User != null)
        //    {
        //        wlyh = User.fldWilayahID;
        //    }
        //    return wlyh;
        //}

        //Added by Shazana 3/4/2023
        public string getCompanyShortName(int? SyarikatID)
        {

            string cmpnyshortname = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).Select(s => s.fld_NamaPndkSyarikat).FirstOrDefault();
            cmpnyshortname = cmpnyshortname == null ? "" : cmpnyshortname;
            return cmpnyshortname;
        }

        //aini add 24042023
        public string Email(int? userid)
        {
            tblUser User;
            string Email;
            Email = "";
            User = db.tblUsers.Where(u => u.fldUserID == userid).FirstOrDefault();
            if (User != null)
            {
                Email = User.fldUserEmail.ToString();
            }

            return Email;
        }
        //aini add 24042023
        public string Roles(int? roleid)
        {
            tblRole Role;
            string RoleName;
            RoleName = "";
            Role = db.tblRoles.Where(u => u.fldRoleID == roleid).FirstOrDefault();
            if (Role != null)
            {
                //aini update select role desc 20062023
                RoleName = Role.fldDescriptionRole.ToString();
            }

            return RoleName;
        }

        //added by faeza 13.07.2023
        public string RolesName(int? roleid)
        {
            tblRole Role;
            string RoleName;
            RoleName = "";
            Role = db.tblRoles.Where(u => u.fldRoleID == roleid).FirstOrDefault();
            if (Role != null)
            {
                RoleName = Role.fldRoleName.ToString();
            }

            return RoleName;
        }

        //aini add 24042023
        public string LastAccess(int? userid)
        {
            tblUserAuditTrail AuditTrail;
            string LastAccess;
            LastAccess = "";
            //aini update orderby desc 15062023
            AuditTrail = db.tblUserAuditTrails.Where(u => u.fld_CreatedBy == userid).Distinct().OrderByDescending(c => c.fld_CreatedDT).FirstOrDefault();
            if (AuditTrail != null)
            {
                LastAccess = AuditTrail.fld_CreatedDT.ToString();
            }

            return LastAccess;
        }

        //Added by Shazana 15/6/20023
        public string GetKesukaran(string JenisHargaKesukaran, int? NegaraID, int? SyarikatID)
        {
            string NamaJenisHargaKesukaran = db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && (x.fldOptConfFlag2 == "HargaKesukaran" || x.fldOptConfFlag2 == "HargaTambahan") && x.fldOptConfFlag1 == JenisHargaKesukaran && x.fldDeleted == false).Select(x => x.fldOptConfDesc).FirstOrDefault();
            return NamaJenisHargaKesukaran;
        }

        //aini add penjawatan 18072023
        public string Penjawatan(string userName)
        {
            tblUserIDApp Penjawatan;
            string Position;
            Position = "";
            Penjawatan = db.tblUserIDApps.Where(u => u.fldUserid == userName).Distinct().FirstOrDefault();
            if (Penjawatan != null)
            {
                Position = Penjawatan.fldJawatan.ToString();
            }

            return Position;
        }


    }
}