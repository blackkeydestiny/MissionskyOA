using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Security
{
    public interface ICryptology
    {
        string Encrypt(string input);

        string Decrypt(string input);
    }
}
