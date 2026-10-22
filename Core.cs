using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace UP01
{
    public class Core
    {
        public static UP01_CarServiceEntities1 Context = new UP01_CarServiceEntities1();

        // Метод хеширования
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        // Возвращает текст ошибки. Если ошибок нет - возвращает пустую строку.
        public static string CheckPasswordRules(string pwd)
        {
            if (string.IsNullOrEmpty(pwd))
                return "Пароль не может быть пустым!";

            if (pwd.Length < 6)
                return "Пароль должен содержать минимум 6 символов!";

            bool hasDigit = false;
            foreach (char c in pwd)
            {
                if (char.IsDigit(c))
                {
                    hasDigit = true;
                }
                else if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')))
                {
                    return "Пароль должен содержать ТОЛЬКО английские буквы и цифры (без пробелов и спецсимволов)!";
                }
            }

            if (!hasDigit)
                return "Пароль должен содержать хотя бы одну цифру!";

            return "";
        }
    }
}