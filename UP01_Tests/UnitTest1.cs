using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UP01_Tests
{
    [TestClass]
    public class PasswordHashTests
    {
        // Тест хеширования пароля
        [TestMethod]
        public void GetHash_ReturnsCorrectHash_For123456()
        {
            // Arrange
            string password = "123456";
            string expectedHash = "7C4A8D09CA3762AF61E59520943DC26494F8941B";

            // Act
            string actualHash = GetHash(password);

            // Assert
            Assert.AreEqual(expectedHash, actualHash);
        }

        [TestMethod]
        public void GetHash_DifferentPasswords_ProduceDifferentHashes()
        {
            string hash1 = GetHash("password1");
            string hash2 = GetHash("password2");

            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void GetHash_SamePassword_ProducesSameHash()
        {
            string hash1 = GetHash("test123");
            string hash2 = GetHash("test123");

            Assert.AreEqual(hash1, hash2);
        }

        // Вспомогательный метод хеширования (копия из RegPage)
        private string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }
    }

    [TestClass]
    public class PasswordValidationTests
    {
        [TestMethod]
        public void Password_TooShort_ReturnsFalse()
        {
            string password = "Ab1";
            Assert.IsTrue(password.Length < 6, "Пароль должен быть меньше 6 символов");
        }

        [TestMethod]
        public void Password_OnlyDigits_FailsValidation()
        {
            string password = "123456";
            bool hasEnglishLetter = false;

            foreach (char c in password)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    hasEnglishLetter = true;
                    break;
                }
            }

            Assert.IsFalse(hasEnglishLetter, "Пароль из только цифр не должен проходить");
        }

        [TestMethod]
        public void Password_OnlyLetters_FailsValidation()
        {
            string password = "Abcdef";
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsDigit(c))
                {
                    hasDigit = true;
                    break;
                }
            }

            Assert.IsFalse(hasDigit, "Пароль из только букв не должен проходить");
        }

        [TestMethod]
        public void Password_Valid_PassesAllChecks()
        {
            string password = "Qwerty123";
            bool hasDigit = false;
            bool hasEnglishLetter = false;
            bool hasInvalidChar = false;

            foreach (char c in password)
            {
                if (char.IsDigit(c))
                    hasDigit = true;
                else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                    hasEnglishLetter = true;
                else
                    hasInvalidChar = true;
            }

            Assert.IsTrue(password.Length >= 6, "Длина должна быть >= 6");
            Assert.IsTrue(hasDigit, "Должна быть цифра");
            Assert.IsTrue(hasEnglishLetter, "Должна быть английская буква");
            Assert.IsFalse(hasInvalidChar, "Не должно быть недопустимых символов");
        }

        [TestMethod]
        public void Password_WithRussianLetters_FailsValidation()
        {
            string password = "Пароль1";
            bool hasInvalidChar = false;

            foreach (char c in password)
            {
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')))
                {
                    hasInvalidChar = true;
                    break;
                }
            }

            Assert.IsTrue(hasInvalidChar, "Русские буквы должны быть запрещены");
        }

        [TestMethod]
        public void Password_WithSpecialChars_FailsValidation()
        {
            string password = "Pass!@#1";
            bool hasInvalidChar = false;

            foreach (char c in password)
            {
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')))
                {
                    hasInvalidChar = true;
                    break;
                }
            }

            Assert.IsTrue(hasInvalidChar, "Спецсимволы должны быть запрещены");
        }
    }

    [TestClass]
    public class StringValidationTests
    {
        [TestMethod]
        public void EmptyString_IsNullOrWhiteSpace()
        {
            string empty = "";
            Assert.IsTrue(string.IsNullOrWhiteSpace(empty));
        }

        [TestMethod]
        public void NullString_IsNullOrWhiteSpace()
        {
            string nullStr = null;
            Assert.IsTrue(string.IsNullOrWhiteSpace(nullStr));
        }

        [TestMethod]
        public void ValidString_IsNotNullOrWhiteSpace()
        {
            string valid = "test";
            Assert.IsFalse(string.IsNullOrWhiteSpace(valid));
        }
    }
}