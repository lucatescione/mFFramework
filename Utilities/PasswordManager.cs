using System;
using System.Reflection;
using mFFramework.LogManager;
using mFFramework.Types;

namespace mFFramework.Utilities
{

    /// <summary>
    /// Gestore password casuali
    /// </summary>
    public static class PasswordManager
    {

        /// <summary>
        /// Genera una password casuale
        /// </summary>
        /// <param name="length">Lunghezza della password</param>
        /// <param name="typePassword">La tipologia di password</param>
        /// <returns>La password con le caratteristiche specificate</returns>
        public static string Generate(int length, Password typePassword = Password.AlphaNumeric)
        {

            try
            {

                char minChar = char.MinValue;
                char maxChar = char.MaxValue;
                string password = string.Empty;
                int number = Int32.MinValue;
                Random numberRandom = new Random();


                switch (typePassword)
                {


                    case Password.AlphaNumeric:
                        minChar = '!';
                        maxChar = '}';
                        break;


                    case Password.Numerical:
                        minChar = '0';
                        maxChar = '9';
                        break;


                    case Password.Literal:
                        minChar = 'A';
                        maxChar = 'z';
                        break;

                    case Password.LiteralAndNumerical:
                        minChar = '0';
                        maxChar = 'z';
                        break;

                }


                for (int i = 0; i < length; i++)
                {

                    number = numberRandom.Next((int)minChar, (int)maxChar);


                    if (typePassword == Password.Literal)
                        switch (number)
                        {

                            case 91: number = 71;
                                break;
                            case 92: number = 83;
                                break;
                            case 93: number = 107;
                                break;
                            case 94: number = 119;
                                break;
                            case 95: number = 67;
                                break;
                            case 96: number = 113;
                                break;

                        }
                    else if (typePassword == Password.LiteralAndNumerical)
                        switch (number)
                        {

                            case 58: number = 51;
                                break;
                            case 59: number = 84;
                                break;
                            case 60: number = 102;
                                break;
                            case 61: number = 55;
                                break;
                            case 62: number = 69;
                                break;
                            case 63: number = 117;
                                break;
                            case 64: number = 77;
                                break;
                            case 91: number = 71;
                                break;
                            case 92: number = 83;
                                break;
                            case 93: number = 107;
                                break;
                            case 94: number = 119;
                                break;
                            case 95: number = 67;
                                break;
                            case 96: number = 113;
                                break;

                        }


                    password += Convert.ToChar(number);



                }

                return password;

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return null;

            }
            #endregion Manage Error
        
        }


      

    }
}
