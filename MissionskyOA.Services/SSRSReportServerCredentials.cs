using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.Reporting.WebForms;

namespace MissionskyOA.Services
{
    public class SSRSReportServerCredentials : IReportServerCredentials
    {
        private string _userName;
        private string _password;
        private string _domain;

        public SSRSReportServerCredentials(string userName, string password, string domain)
        {
            _userName = userName;
            _password = password;
            _domain = domain;
        }

        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                if (string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_password))
                {
                    return null;
                }
                else if (string.IsNullOrEmpty(_domain))
                {
                    return new NetworkCredential(_userName, _password);
                }
                else
                {
                    return new NetworkCredential(_userName, _password, _domain);
                }
            }
        }

        public bool GetFormsCredentials(out System.Net.Cookie authCookie, out string user, out string password, out string authority)
        {
            authCookie = null;
            user = password = authority = null;
            return false;
        }
    }
}
