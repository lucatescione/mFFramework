using mFFramework.Conversions;
using mFFramework.LogManager;
using mFFramework.Types;
using System;
using System.Reflection;

namespace mFFramework.Utilities
{

    /// <summary>
    /// Conversione Numerica
    /// </summary>
    public static class NumericConversion
    {
               

        /// <summary>
        /// Codifica, in maniera asimmetrica, una stringa
        /// </summary>
        /// <param name="stringer">Stringa da codificare asimmetricamente</param>
        /// <param name="alphabets">Alfabeti di codifica</param>
        /// <returns>La stringa codificata asimmetricamente</returns>
        private static string CodingAsymmetric(string stringer, string[] alphabets)
        {

            // recupero la lunghezza della stringa da codificare e il numero di alfabeti
            int stringerLength = stringer.Length;
            int numberAlphabets = alphabets.Length;

            // determino le parti della stringa da convertire nei rispetti alfabeti
            int partLength = stringerLength / numberAlphabets;
            // se rimane un resto, questo viene convertito con l'alfabeto di default
            int reminderPart = stringerLength % numberAlphabets;


            string stringerPart = string.Empty;
            string stringerCoding = string.Empty;
            int positionStart = 0;
            int positionEnd = -1;
            for (int a = 0; a < numberAlphabets; a++)
            {
                positionStart = positionEnd + 1;
                positionEnd = positionStart + partLength - 1;
                stringerPart = stringer.Substring(positionStart, partLength);
                stringerCoding += NumericConversion.Convert(stringerPart, Encode.Coding, alphabets[a]) + "-";
            }

            if (reminderPart > 0)
            {
                // codifica, con l'alfabeto di default, la parte di stringa rimanente
                stringerPart = stringer.Substring(positionEnd + 1, reminderPart);
                stringerCoding += NumericConversion.Convert(stringerPart, Encode.Coding);
            }
            else // stringa esatta col numero di alfabeti
                stringerCoding = stringerCoding.Substring(0, stringerCoding.Length - 1);


            return stringerCoding;

        }



        /// <summary>
        /// Decodifica, in maniera asimmetrica, una stringa
        /// </summary>
        /// <param name="stringer">Stringa da decodificare asimmetricamente</param>
        /// <param name="alphabets">Alfabeti di decodifica</param>
        /// <returns>La stringa decodificata asimmetricamente</returns>
        private static string DecodingAsymmetric(string stringer, string[] alphabets)
        {
            // recupero la lunghezza della stringa da codificare e il numero di alfabeti
            string[] stringerParts = stringer.Split('-');
            int numberStringerParts = stringerParts.Length;
            int numberAlphabets = alphabets.Length;

            // correggo se c'è un resto
            int countParts = numberStringerParts > numberAlphabets ? numberStringerParts - 1 : numberStringerParts;

            string stringerDecoding = string.Empty;
            // decodifica tutte le parti con i rispettivi alfabeti
            for (int p = 0; p < countParts; p++)
                stringerDecoding += NumericConversion.Convert(stringerParts[p], Encode.Decoding, alphabets[p]);
  

            // se c'è un resto, decodifica l'ultima parte con l'alfabeto di default
            if (numberStringerParts > numberAlphabets)
                stringerDecoding += NumericConversion.Convert(stringerParts[numberStringerParts -1], Encode.Decoding);


            return stringerDecoding;
        }
               


        /// <summary>
        /// Conversione numerica asimmetrica
        /// </summary>
        /// <param name="stringer">Stringa da convertire</param>
        /// <param name="encoding">Tipologia di conversione numerica</param>
        /// <param name="alphabets">Alfabeti di conversione</param>
        /// <param name="times">Numero di volte che si converte (default = 1)</param>
        /// <returns>Stringa convertita asimmetricamente</returns>
        public static string ConvertAsymmetric(string stringer, Encode encoding, string[] alphabets, int times = 1)
        {
            string result = string.Empty;

            if (!stringer.IsVoid())
            switch (encoding)
            {
                case Encode.Coding:
                    result = CodingAsymmetric(stringer, alphabets);
                    break;

                case Encode.Decoding:
                    result = DecodingAsymmetric(stringer, alphabets);
                    break;
            }


            return result;

        }
        


        /// <summary>
        /// Conversione numerica di una stringa
        /// </summary>
        /// <param name="stringer">La stringa da convertire/deconvertire</param>
        /// <param name="encoding">La tipologia di codifica: Coding/Encoding</param>
        /// <param name="alphabet">L'alfabeto in base al quale codificare la stringa (se null si specifica l'alfabeto di default)</param>
        /// <returns>La stringa convertita/deconvertita</returns>
        public static string Convert(string stringer, Encode encoding = Encode.Coding, string alphabet = null)
        {
            try
            {

                string stringerConverted = string.Empty;
                string character = string.Empty;
                char quotes = '"';
                int index = 0;
                int length = stringer.Length;


                // alfabeto di default
                if (alphabet == null)
                    alphabet = "abc7defghij9klmnopQqrstu\\vw1xyzABCD€EF6GHIJMNìOPRS2TUéVWXYZ034ù8-+.,:;?!£$'%L&/=()^*5#@°<>[]{}|_§àKèò ■" + quotes;


                switch (encoding)
                {

                    default:
                    case Encode.Coding:
                        for (int i = 0; i < length; i++)
                        {

                            character = stringer.Substring(i, 1);
                            index = alphabet.IndexOf(character) + 100;
                            stringerConverted += index.ToString();

                        }
                        break;

                    case Encode.Decoding:
                        for (int i = 0; i < length; i += 3)
                        {

                            index = stringer.Substring(i, 3).ToInt().Value - 100;
                            character = alphabet.Substring(index, 1);
                            stringerConverted = stringerConverted + character;

                        }
                        break;

                }


                return stringerConverted;

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return null;

            }
            #endregion Manage Error

        }



        /// <summary>
        /// Conversione numerica multipla
        /// </summary>
        /// <param name="stringer">Stringa da convertire</param>
        /// <param name="encoding">Tipologia di conversione numerica</param>
        /// <param name="alphabet">Alfabeto di conversione</param>
        /// <param name="times">Numero di volte che si converte (default = 1)</param>
        /// <returns>Stringa convertita</returns>
        public static string ConvertMulti(string stringer, Encode encoding = Encode.Coding, string alphabet = null, int times = 1)
        {
            string stringerConverted = stringer;
            for (int t = 1; t <= times; t++)
                stringerConverted = Convert(stringerConverted, encoding, alphabet);

            return stringerConverted;

        }



    }


}
