using mFFramework.Types;
using System;
using System.Globalization;
using mFFramework.Utilities;


namespace mFFramework.Conversions
{

    /// <summary>
    /// Metodi Estesi per le conversioni
    /// </summary>
    public static class ExtendedConversions
    {

        private static short parseshort;
        private static int parseint;
        private static long parselong;
        private static double parsedouble;
        private static decimal parsedecimal;
        private static DateTime parsedatetime;
        private static bool parsebool;



        #region ----  PUBLIC  ----





        /// <summary>
        /// Converte il testo di una TexBox in intero breve
        /// </summary>
        /// <param name="control">TexBox da convertire</param>
        /// <returns>Il numero intero breve oppure, nel caso che il testo non sia convertibile, null.</returns>
        public static short? ToShort(this string control)
        {

            return (Int16.TryParse(control, out parseshort)) ? (Int16?)parseshort : null;

        }


        /// <summary>
        /// Converte una stringa in intero
        /// </summary>
        /// <param name="control">Stringa da convertire</param>
        /// <returns>Il numero intero oppure, nel caso che il testo non sia convertibile, null.</returns>
        public static int? ToInt(this string control)
        {

            return (Int32.TryParse(control.Replace(".",string.Empty), out parseint)) ? (int?)parseint : null;

        }


        /// <summary>
        /// Converte una enumerazione in intero
        /// </summary>
        /// <typeparam name="TypeEnumeration"></typeparam>
        /// <param name="control"></param>
        /// <returns></returns>
        public static int? ToInt<TypeEnumeration>(this TypeEnumeration control)
        where TypeEnumeration : struct, IComparable, IConvertible, IFormattable
        {

            return Convert.ToInt32(control);

        }


        /// <summary>
        /// Converte una stringa in long
        /// </summary>
        /// <param name="control">Stringa da convertire</param>
        /// <returns>Il numero intero oppure, nel caso che il testo non sia convertibile, null.</returns>
        public static long? ToLong(this string control)
        {

            return (Int64.TryParse(control, out parselong)) ? (long?)parselong : null;

        }


        /// <summary>
        /// Converte una stringa in double
        /// </summary>
        /// <param name="control">Stringa da convertire</param>
        /// <returns>Il numero double oppure, nel caso che il testo non sia convertibile, null.</returns>
        public static double? ToDouble(this string control)
        {
            string control_decimal = control.IndexOf(',') == -1 ? control.Replace(".", ",") : control;
            return (Double.TryParse(control_decimal, out parsedouble)) ? (double?)parsedouble : null;

        }


        /// <summary>
        /// Converte una stringa in datetime
        /// </summary>
        /// <param name="control">Stringa da convertire</param>
        /// <param name="cultureInfo"></param>
        /// <returns>La data oppure, nel caso che il testo non sia convertibile, null.</returns>
        public static DateTime? ToDateTime(this string control, string cultureInfo = "it-IT")
        {
            string[] v_control = new string[3];
            if (cultureInfo == "en-US" && !control.IsVoid())
            {
                v_control = control.Split(' ')[0].Trim().Split('/');
                control = v_control[1] + "/" + v_control[0] + "/" + v_control[2];
            }

            return (DateTime.TryParse(control, out parsedatetime)) ? (DateTime?)parsedatetime : null;

        }


        /// <summary>
        /// Converte una stringa in decimal
        /// </summary>
        /// <param name="control">Stringa da convertire</param>
        /// <returns>Il numero decimal oppure, nel caso che il testo non sia convertibile, null.</returns>
        public static decimal? ToDecimal(this string control)
        {

            return (Decimal.TryParse(control, out parsedecimal)) ? (decimal?)parsedecimal : null;

        }


        /// <summary>
        /// Converte una stringa in boolean
        /// </summary>
        /// <param name="control">Stringa da convertire</param>
        /// <returns>Il valore boolean oppure, nel caso che il testo non sia convertibile, null.</returns>
        public static bool? ToBool(this string control)
        {

            string controlToControl = control == "1"
                                              ? "true"
                                              : control == "0"
                                                        ? "false"
                                                        : control;

            return (bool.TryParse(controlToControl, out parsebool)) ? (bool?)parsebool : null;

        }


        /// <summary>
        /// Converte una stringa in valuta in double
        /// </summary>
        /// <param name="control"></param>
        /// <param name="typeCurrency"></param>
        /// <returns></returns>
        public static double? ToDoubleFromCurrency(this string control, TypeCurrency typeCurrency = TypeCurrency.Euro)
        {

            CultureInfo cultureInfo = new System.Globalization.CultureInfo(typeCurrency.GetCode());

            return !control.IsVoid()
                        ? (Double.TryParse(control, NumberStyles.Currency, cultureInfo, out parsedouble)) ? (double?)parsedouble : null
                        : 0;

        }


        #endregion ----  PUBLIC  ----

    }
}
