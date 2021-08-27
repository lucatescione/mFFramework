using System;
using System.Drawing;
using System.Reflection;



namespace mFFramework.Types
{


    /// <summary>
    /// 
    /// </summary>
    public class CodeAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public CodeAttribute(string value)
        {
            this.Code = value;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ColorAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public Color _Color { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public ColorAttribute(Color value)
        {
            this._Color = value;
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public class PatternAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PatternAttribute(string value)
        {
            this.Pattern = value;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public DescriptionAttribute(string value)
        {
            this.Description = value;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class Description2Attribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Description2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Description2Attribute(string value)
        {
            this.Description2 = value;
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class Description3Attribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Description3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Description3Attribute(string value)
        {
            this.Description3 = value;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class Description4Attribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Description4 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Description4Attribute(string value)
        {
            this.Description4 = value;
        }
    }

    /// <summary>
    /// Percorso fisico
    /// </summary>
    public class PathAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PathAttribute(string value)
        {
            this.Path = value;
        }
    }


    ///// <summary>
    ///// 
    ///// </summary>
    //public class SQLTypeAttribute : Attribute
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string SQLType { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="value"></param>
    //    public SQLTypeAttribute(string value)
    //    {
    //        this.SQLType = value;
    //    }

    //}


    //public class CLRTypeAttribute : Attribute
    //{

    //    public string CLRType { get; set; }

    //    public CLRTypeAttribute(string value)
    //    {
    //        this.CLRType = value;
    //    }

    //}




    /// <summary>
    /// 
    /// </summary>
    public static class AttributeExtension
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color GetColor(this Enum value)
        {
            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(value.ToString());

            ColorAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(ColorAttribute), false) as ColorAttribute[];

            return attribs.Length > 0 ? attribs[0]._Color : Color.Black;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetPattern(this Enum value)
        {
            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(value.ToString());

            PatternAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(PatternAttribute), false) as PatternAttribute[];

            return attribs.Length > 0 ? attribs[0].Pattern : null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetCode(this Enum value)
        {
            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(value.ToString());

            CodeAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(CodeAttribute), false) as CodeAttribute[];

            return attribs.Length > 0 ? attribs[0].Code : null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(value.ToString());

            DescriptionAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return attribs.Length > 0 ? attribs[0].Description : null;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription2(this Enum value)
        {
            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(value.ToString());

            Description2Attribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(Description2Attribute), false) as Description2Attribute[];

            return attribs.Length > 0 ? attribs[0].Description2 : null;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription3(this Enum value)
        {
            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(value.ToString());

            Description3Attribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(Description3Attribute), false) as Description3Attribute[];

            return attribs.Length > 0 ? attribs[0].Description3 : null;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription4(this Enum value)
        {
            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(value.ToString());

            Description4Attribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(Description4Attribute), false) as Description4Attribute[];

            return attribs.Length > 0 ? attribs[0].Description4 : null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetPath(this Enum value)
        {
            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(value.ToString());

            PathAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(PathAttribute), false) as PathAttribute[];

            return attribs.Length > 0 ? attribs[0].Path : null;

        }

        //public static string GetSQLType(this Enum value)
        //{
        //    Type type = value.GetType();

        //    FieldInfo fieldInfo = type.GetField(value.ToString());

        //    SQLTypeAttribute[] attribs = fieldInfo.GetCustomAttributes(
        //        typeof(SQLTypeAttribute), false) as SQLTypeAttribute[];

        //    return attribs.Length > 0 ? attribs[0].SQLType : null;
        //}



        //public static string GetCLRType(this Enum value)
        //{
        //    Type type = value.GetType();

        //    FieldInfo fieldInfo = type.GetField(value.ToString());

        //    CLRTypeAttribute[] attribs = fieldInfo.GetCustomAttributes(
        //        typeof(CLRTypeAttribute), false) as CLRTypeAttribute[];

        //    return attribs.Length > 0 ? attribs[0].CLRType : null;
        //}


    }



    //public static class MethodExtension
    //{


    //    public static TypeEnumeration GetValueByCode<TypeEnumeration>(this TypeEnumeration typeEnumeration, string value)
    //     where TypeEnumeration : struct, IComparable, IConvertible, IFormattable
    //    {



    //        object obj = null;
    //        TypeEnumeration findValueEnumeration = default(TypeEnumeration);
    //        Enum findEnum = null;



    //        if (!typeEnumeration.GetType().IsEnum)
    //           return findValueEnumeration;



    //        TypeEnumeration[] valuesEnumeration = (TypeEnumeration[])typeEnumeration.GetType().GetEnumValues();

    //        foreach (TypeEnumeration valueEnumeration in valuesEnumeration)
    //        {

    //            obj = Enum.Parse(typeof(TypeEnumeration), valueEnumeration.ToString(), true);
    //            findEnum = (Enum)obj;

    //            if (findEnum.GetCode().ToLower() == value.ToLower())
    //            {

    //                //findValueEnumeration = (TypeEnumeration)obj;
    //                break;
    //            }

    //        }

            
    //        return (TypeEnumeration)obj;



    //    }


       

    //}


}
