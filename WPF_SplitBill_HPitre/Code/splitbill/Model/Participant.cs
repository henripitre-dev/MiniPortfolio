using splitbill.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace splitbill.Model
{
    public class Participant
    {
        private readonly string _username;
        private readonly string _passwordHash;

        public string Username => _username;
        public string PasswordHash => _passwordHash;

        public static string Hash(string input)
        {
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        public bool VerifyPassword(string password)
        {
            return PasswordHash == Hash(password);
        }


        public Participant(string username, string pwd, bool pwdHashed)
        {
            _username = username.Trim();
            if (pwdHashed) _passwordHash = pwd;
            else _passwordHash = Hash(pwd);
        }
    }
}
