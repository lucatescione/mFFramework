using System;
using System.Collections.Generic;

namespace mFFramework.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public  class Age
    {
        /// <summary>
        /// 
        /// </summary>
        public int NumberYears { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NumberMonths { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NumberDays { get; set; }

    
    }



    /// <summary>
    /// 
    /// </summary>
    public class TaxCode
    {

        private int year;
        private int month;
        private int day;
        private int years = 0;
        private int months = 0;
        private int days = 0;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CF { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        public string codeBelfiore { get; set; }
        
        private string[] caratteriMesi = { "A", "B", "C", "D", "E", "H", "L", "M", "P", "R", "S", "T" };





        private Age age;
        /// <summary>
        /// Età espressa in numero di anni, di mesi e di giorni
        /// </summary>
        public Age _Age { get { return age; } }


        private DateTime birthDate;
        /// <summary>
        /// Data di nascita
        /// </summary>
        public DateTime BirthDate { get { return birthDate; } }

 
        private string sex;
        /// <summary>
        /// Sesso
        /// </summary>
        public string Sex { get { return sex; } }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ReferenceDate { get; set; }

        internal TabelleCodiceFiscale TabelleCF { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="sex"></param>
        /// <param name="dateBirth"></param>
        /// <param name="codcomune"></param>
        /// <param name="taxCode"></param>
        public TaxCode(string name
            , string surname
            , string sex
            , DateTime dateBirth
            , string codcomune
            , string taxCode)
        {
            this.Name = (name == null) ? string.Empty : name.ToUpper();
            this.Surname = (surname == null) ? string.Empty : surname.ToUpper();
            //this.Sex = (sex == null) ? string.Empty : sex.ToUpper();
            //this.BirthDate = dateBirth;
            this.codeBelfiore = (codcomune == null) ? string.Empty : codcomune.ToUpper();
            this.CF = (taxCode == null) ? string.Empty : taxCode.ToUpper();
            this.TabelleCF = new TabelleCodiceFiscale();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="taxCode"></param>
        public TaxCode(string taxCode)
        {

            this.CF = (taxCode == null) ? string.Empty : taxCode.ToUpper();
            this.TabelleCF = new TabelleCodiceFiscale();

            SetBirthDate();
            SetSex();
            SetCodiceBelfiore();
           
        }



        /// <summary>
        /// Calcola l'età rispetto a una data di riferimento
        /// </summary>
        public Age GetAge(DateTime referenceDate) 
        {

            this.ReferenceDate = referenceDate;

            age = new Age();

            if (!ReferenceDate.HasValue)
            {
                age.NumberDays = age.NumberMonths = age.NumberYears = -999999;
                return age;
            }

            DateTime temporarydate = DateTime.Now;
           
             
            // compute & return the difference of two dates,
            // returning years, months & days
            // d1 should be the larger (newest) of the two dates
            // we want d1 to be the larger (newest) date
            // flip if we need to
            if (ReferenceDate < birthDate)
            {
                temporarydate = birthDate;
                birthDate = ReferenceDate.Value;
                ReferenceDate = temporarydate;
            }

            // compute difference in total months
            months = 12 * (ReferenceDate.Value.Year - birthDate.Year) + (ReferenceDate.Value.Month - birthDate.Month);

            // based upon the 'days',
            // adjust months & compute actual days difference
            if (ReferenceDate.Value.Day < birthDate.Day)
            {
                months--;
                days = DateTime.DaysInMonth(birthDate.Year, birthDate.Month) - birthDate.Day + ReferenceDate.Value.Day;
                //days = ReferenceDate.Value.Day;
            }
            else
                days = ReferenceDate.Value.Day - birthDate.Day;
       

            // compute years & actual months
            years = months / 12;
            months -= years * 12;

            // imposta l'età
            age.NumberDays = days;
            age.NumberMonths = months;
            age.NumberYears = years;

            return age;
        }


        private void SetCodiceBelfiore()
        { 
           codeBelfiore = CF.Substring(11, 4);
        }



        private void SetBirthDate()
        {

            
            // anno di nascita
            year = 1900 + Convert.ToInt32(CF.Substring(6, 2));
           
            // mese di nascita
            switch (CF.Substring(8, 1))
            {

                case "A": month = 1;
                    break;

                case "B": month = 2;
                    break;

                case "C": month = 3;
                    break;

                case "D": month = 4;
                    break;

                case "E": month = 5;
                    break;

                case "H": month = 6;
                    break;

                case "L": month = 7;
                    break;

                case "M": month = 8;
                    break;

                case "P": month = 9;
                    break;

                case "R": month = 10;
                    break;

                case "S": month = 11;
                    break;

                case "T": month = 12;
                    break;
            }

            // giorno di nasciat
            int _day = Convert.ToInt32(CF.Substring(9, 2));
            day = _day > 40 ? _day - 40 : _day;

            // calcola la data di nascita in base alla correzione 1900/2000
            int differenzaAnni = DateTime.Today.Year - year;
            birthDate = differenzaAnni >= 100
                                 ? new DateTime(2000 + Convert.ToInt32(CF.Substring(6, 2)), month, day)
                                 : new DateTime(year, month, day);
            
        
        }

        private void SetSex()
        {

            int _day = Convert.ToInt32(CF.Substring(9, 2));
            
            sex = _day <= 40 ? "Male" : "Female";

        }


        private string GetNameFromTaxCode()
        {
            char[] aNome = this.Name.ToCharArray();
            string strNome = string.Empty;

            //ciclo sui caratteri del surname
            for (int i = 0; i < aNome.Length; i++)
            {
                aNome[i] = this.ReplaceAccentedChar(aNome[i]);

                switch (aNome[i])
                {
                    case 'A':
                    case 'E':
                    case 'I':
                    case 'O':
                    case 'U': break;
                    default:
                        if ((aNome[i] <= 'Z') && (aNome[i] > 'A'))
                        { strNome += aNome[i]; } break;
                }
            }

            if (strNome.Length > 3)
            {
                strNome = strNome.Substring(0, 1) + strNome.Substring(2, 2);
            }
            else
            {
                //Se minore di 3 prendo una vocale altrimento aggiungo una X 
                if (strNome.Length < 3)
                {
                    for (int i = 0; i < aNome.Length; i++)
                    {
                        aNome[i] = this.ReplaceAccentedChar(aNome[i]);
                        switch (aNome[i])
                        {
                            case 'A':
                            case 'E':
                            case 'I':
                            case 'O':
                            case 'U': strNome += aNome[i]; break;
                        }
                    }
                    if (strNome.Length < 3)
                    {
                        for (int i = strNome.Length; i <= 3; i++)
                        { strNome += 'X'; }
                    }
                }
            }
            strNome = strNome.Substring(0, 3);
            return strNome;

        }

        private string GetSurnameFromTaxCode()
        {
            string strCognome = string.Empty;
            char[] aCognome = this.Surname.ToCharArray();

            //ciclo sui caratteri del surname
            for (int i = 0; i < aCognome.Length; i++)
            {
                aCognome[i] = this.ReplaceAccentedChar(aCognome[i]);

                switch (aCognome[i])
                {
                    case 'A':
                    case 'E':
                    case 'I':
                    case 'O':
                    case 'U': break;
                    default:
                        if ((aCognome[i] <= 'Z') && (aCognome[i] > 'A'))
                        { strCognome += aCognome[i]; } break;
                }
            }

            //in questo caso aggiungo una vocale
            if (strCognome.Length < 3)
            {
                //ciclo sui caratteri del surname
                for (int i = 0; i < aCognome.Length; i++)
                {
                    aCognome[i] = this.ReplaceAccentedChar(aCognome[i]);

                    switch (aCognome[i])
                    {
                        case 'A':
                        case 'E':
                        case 'I':
                        case 'O':
                        case 'U': strCognome += aCognome[i]; break;
                    }
                }

                //guardo se risulta sempre minore di 3 aggiungo il carattere X
                if (strCognome.Length < 3)
                {
                    for (int i = strCognome.Length; i <= 3; i++)
                    { strCognome += 'X'; }
                }

            }

            strCognome = strCognome.Substring(0, 3);

            return strCognome;
        }

        private string GetDayBirthFromTaxCode()
        {
            string strGiorno = string.Empty;

            switch (this.Sex)
            {
                case "M": strGiorno = this.BirthDate.Day < 10 ? "0" + Convert.ToString(this.BirthDate.Day) : Convert.ToString(this.BirthDate.Day); break;
                case "F": strGiorno = Convert.ToString(this.BirthDate.Day + 40); break;
            }

            return strGiorno;
        }

        private string GetCheckSumFromTaxCode(string strCFSenzaUltimoCarattere)
        {
            char[] cfArray = strCFSenzaUltimoCarattere.ToCharArray();
            int somma = 0;
            int resto = 0;
            string carattereControllo = string.Empty;

            //Faccio la sum in base alle tabelle dall'Agenzia delle Entrate
            for (int i = 0; i < 15; i++)
            {
                if (((i + 1) % 2) != 0)
                {
                    somma += this.TabelleCF.ListaDispari[cfArray[i]];
                }
                else
                {
                    somma += this.TabelleCF.ListaPari[cfArray[i]];
                }
            }

            //Calcolo il rest della sum % 26 
            resto = somma % 26;
            carattereControllo = Convert.ToString(this.TabelleCF.ListaControllo[resto]);

            return carattereControllo;
        }


        /// <summary>
        /// Sostituisce il carattere accentato con quello normale
        /// </summary>
        /// <param name="ch">Carattere accentato</param>
        /// <returns>Il carattere normale</returns>
        private char ReplaceAccentedChar(char ch)
        {
            switch (ch)
            {
                case 'à':
                case 'Á':
                case 'À':
                    ch = 'A'; break;
                case 'è':
                case 'È':
                case 'é':
                case 'É':
                    ch = 'E'; break;
                case 'ì':
                case 'Ì':
                case 'Í':
                    ch = 'I'; break;
                case 'ò':
                case 'Ò':
                case 'Ó':
                    ch = 'O'; break;
                case 'ù':
                case 'Ù':
                case 'Ú':
                    ch = 'U'; break;
            }
            return ch;
        }



        /// <summary>
        /// Genera il codice fiscale con i parametri passati al costruttore
        /// </summary>
        /// <returns></returns>
        public string Create()
        {
            string strCF, strCognome, strNome, strMese, strAnno, strGiorno, strControllo = string.Empty;

            //calcolo il cognome
            strCognome = this.GetSurnameFromTaxCode();
            //Calcolo il nome
            strNome = this.GetNameFromTaxCode();
            //Calcolo il giorno
            strGiorno = this.GetDayBirthFromTaxCode();
            //Mese 
            strMese = caratteriMesi[(this.BirthDate.Month - 1)];
            //Anno
            strAnno = Convert.ToString(this.BirthDate.Year).Substring(2, 2);
            //Calcolo carattere di controllo
            strCF = strCognome + strNome + strAnno + strMese + strGiorno + this.codeBelfiore;
            strControllo = this.GetCheckSumFromTaxCode(strCF);

            //Termino il codice fiscale
            strCF = strCF + strControllo;

            return strCF.ToUpper();
        }


    }


    

    /// <summary>
    /// 
    /// </summary>
    internal class TabelleCodiceFiscale
    {

        #region Campi

        /// <summary>
        /// Tabella lista pari
        /// </summary>
        private Dictionary<char, int> listaPari;

        /// <summary>
        /// Tabella della lista Dispari
        /// </summary>
        private Dictionary<char, int> listaDispari;
        
        /// <summary>
        /// Tabella lista omocodia
        /// </summary>
        private Dictionary<char, int> listaOmocodia;
        
        /// <summary>
        /// Tabella CheckDigit
        /// </summary>
        private Dictionary<int, char> listaControllo;

        #endregion

        #region Proprietà

        public Dictionary<char, int> ListaPari
        {
            get { return this.listaPari; }
        }

        public Dictionary<char, int> ListaDispari
        {
            get { return listaDispari; }
        }

        public Dictionary<int, char> ListaControllo
        {
            get { return this.listaControllo; }
        }

        public Dictionary<char, int> ListaOmocodia
        {
            get { return this.listaOmocodia; }
        }

        #endregion

        public TabelleCodiceFiscale()
        {
            listaPari = new Dictionary<char, int>();
            listaDispari = new Dictionary<char, int>();
            listaOmocodia = new Dictionary<char, int>();
            listaControllo = new Dictionary<int, char>();

            this.InizializzaListaPari();
            this.InizializzaListaDispari();
            this.InizializzaListaOmocodici();
            this.InizializzaListaControllo();
        }

        #region Tabelle

        /// <summary>
        /// Inizializza la Tabella Pari
        /// </summary>
        private void InizializzaListaPari()
        {
            listaPari.Add('A', 0);
            listaPari.Add('B', 1);
            listaPari.Add('C', 2);
            listaPari.Add('D', 3);
            listaPari.Add('E', 4);
            listaPari.Add('F', 5);
            listaPari.Add('G', 6);
            listaPari.Add('H', 7);
            listaPari.Add('I', 8);
            listaPari.Add('J', 9);
            listaPari.Add('K', 10);
            listaPari.Add('L', 11);
            listaPari.Add('M', 12);
            listaPari.Add('N', 13);
            listaPari.Add('O', 14);
            listaPari.Add('P', 15);
            listaPari.Add('Q', 16);
            listaPari.Add('R', 17);
            listaPari.Add('S', 18);
            listaPari.Add('T', 19);
            listaPari.Add('U', 20);
            listaPari.Add('V', 21);
            listaPari.Add('W', 22);
            listaPari.Add('X', 23);
            listaPari.Add('Y', 24);
            listaPari.Add('Z', 25);
            listaPari.Add('0', 0);
            listaPari.Add('1', 1);
            listaPari.Add('2', 2);
            listaPari.Add('3', 3);
            listaPari.Add('4', 4);
            listaPari.Add('5', 5);
            listaPari.Add('6', 6);
            listaPari.Add('7', 7);
            listaPari.Add('8', 8);
            listaPari.Add('9', 9);
        }

        /// <summary>
        /// Inizializza Tabella Dispari
        /// </summary>
        private void InizializzaListaDispari()
        {
            listaDispari.Add('0', 1);
            listaDispari.Add('1', 0);
            listaDispari.Add('2', 5);
            listaDispari.Add('3', 7);
            listaDispari.Add('4', 9);
            listaDispari.Add('5', 13);
            listaDispari.Add('6', 15);
            listaDispari.Add('7', 17);
            listaDispari.Add('8', 19);
            listaDispari.Add('9', 21);
            listaDispari.Add('A', 1);
            listaDispari.Add('B', 0);
            listaDispari.Add('C', 5);
            listaDispari.Add('D', 7);
            listaDispari.Add('E', 9);
            listaDispari.Add('F', 13);
            listaDispari.Add('G', 15);
            listaDispari.Add('H', 17);
            listaDispari.Add('I', 19);
            listaDispari.Add('J', 21);
            listaDispari.Add('K', 2);
            listaDispari.Add('L', 4);
            listaDispari.Add('M', 18);
            listaDispari.Add('N', 20);
            listaDispari.Add('O', 11);
            listaDispari.Add('P', 3);
            listaDispari.Add('Q', 6);
            listaDispari.Add('R', 8);
            listaDispari.Add('S', 12);
            listaDispari.Add('T', 14);
            listaDispari.Add('U', 16);
            listaDispari.Add('V', 10);
            listaDispari.Add('W', 22);
            listaDispari.Add('X', 25);
            listaDispari.Add('Y', 24);
            listaDispari.Add('Z', 23);
        }

        /// <summary>
        /// Inizializza Tabella Omocodia
        /// </summary>
        private void InizializzaListaOmocodici()
        {
            listaOmocodia.Add('L', 0);
            listaOmocodia.Add('M', 1);
            listaOmocodia.Add('N', 2);
            listaOmocodia.Add('P', 3);
            listaOmocodia.Add('Q', 4);
            listaOmocodia.Add('R', 5);
            listaOmocodia.Add('S', 6);
            listaOmocodia.Add('T', 7);
            listaOmocodia.Add('U', 8);
            listaOmocodia.Add('V', 9);
        }

        /// <summary>
        /// Inizializza Tabella Check Digit
        /// </summary>
        private void InizializzaListaControllo()
        {
            listaControllo.Add(0, 'A');
            listaControllo.Add(1, 'B');
            listaControllo.Add(2, 'C');
            listaControllo.Add(3, 'D');
            listaControllo.Add(4, 'E');
            listaControllo.Add(5, 'F');
            listaControllo.Add(6, 'G');
            listaControllo.Add(7, 'H');
            listaControllo.Add(8, 'I');
            listaControllo.Add(9, 'J');
            listaControllo.Add(10, 'K');
            listaControllo.Add(11, 'L');
            listaControllo.Add(12, 'M');
            listaControllo.Add(13, 'N');
            listaControllo.Add(14, 'O');
            listaControllo.Add(15, 'P');
            listaControllo.Add(16, 'Q');
            listaControllo.Add(17, 'R');
            listaControllo.Add(18, 'S');
            listaControllo.Add(19, 'T');
            listaControllo.Add(20, 'U');
            listaControllo.Add(21, 'V');
            listaControllo.Add(22, 'W');
            listaControllo.Add(23, 'X');
            listaControllo.Add(24, 'Y');
            listaControllo.Add(25, 'Z');
        }

        #endregion

    }



    /// <summary>
    /// 
    /// </summary>
    public static class ControlsTaxCode
    { 


         /// <summary>
        /// Controlla che il codice fiscale sia coerente con il proprio carattere checksum
        /// </summary>
        /// <param name="taxCode">Codice fiscale da controllare se di 16 cifre</param>
        public static bool CheckSum(string taxCode)
        {

            if (taxCode == null || taxCode.Length != 16)
                return true;

            TaxCode c = new TaxCode(string.Empty, string.Empty, string.Empty, DateTime.Today, string.Empty, taxCode);

            if (!CheckFormat(c))
                return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="taxCode"></param>
        /// <returns></returns>
        private static bool CheckFormat(TaxCode taxCode)
        {
            
            char[] taxCodeArray = taxCode.CF.ToCharArray();
            int sum = 0;
            int rest = 0;
            char controlCharacter = ' ';

            // Faccio la somma in base alle tabelle dall'Agenzia delle Entrate
            for (int i = 0; i < (taxCodeArray.Length - 1); i++)
            {
                if (((i + 1) % 2) != 0)
                    sum += taxCode.TabelleCF.ListaDispari[taxCodeArray[i]];
                else
                    sum += taxCode.TabelleCF.ListaPari[taxCodeArray[i]];
                
            }

            // Calcolo il resto della somma % 26 
            rest = sum % 26;
            controlCharacter = taxCode.TabelleCF.ListaControllo[rest];

            return controlCharacter.Equals(taxCodeArray[(taxCodeArray.Length - 1)]);
        }
    
    }


}
