using SCEEC.Numerics.Quantities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SCEEC.Numerics
{
    /// <summary>
    /// SI词头
    /// </summary>
    public enum Prefixs
    {
        y = -24,
        z = -21,
        a = -18,
        f = -15,
        p = -12,
        n = -9,
        μ = -6,
        u = -6,
        m = -3,
        None = 0,
        k = 3,
        M = 6,
        G = 9,
        T = 12,
        P = 15,
        E = 18,
        Z = 21,
        Y = 24
    }

    

    /// <summary>
    /// SI词头转换器
    /// </summary>
    public static class PrefixsConverter
    {
        /// <summary>
        /// 将10为底的幂数转化为SI词头
        /// </summary>
        /// <param name="dec">10为底的幂数</param>
        /// <returns></returns>
        public static string dec2prefixString(int dec)
        {
            if (dec >= 0)
            {
                dec = dec / 3;
                dec = dec * 3;
            }
            else
            {
                dec = (dec - 2) / 3;
                dec = dec * 3;
            }
            switch (dec)
            {
                case -24:
                    return "y";
                case -21:
                    return "z";
                case -18:
                    return "a";
                case -15:
                    return "f";
                case -12:
                    return "p";
                case -9:
                    return "n";
                case -6:
                    return "μ";
                case -3:
                    return "m";
                case 0:
                    return " ";
                case 3:
                    return "k";
                case 6:
                    return "M";
                case 9:
                    return "G";
                case 12:
                    return "T";
                case 15:
                    return "P";
                case 18:
                    return "E";
                case 21:
                    return "Z";
                case 24:
                    return "Y";
                default:
                    throw new Exception("值大小过大或过小。");
            }
        }

        /// <summary>
        /// SI词头转换为10为底的幂数
        /// </summary>
        /// <param name="prefix">SI词头</param>
        /// <returns></returns>
        public static int prefixString2dec(string prefix)
        {
            switch (prefix)
            {
                case "y": return -24;
                case "z": return -21;
                case "a": return -18;
                case "f": return -15;
                case "p": return -12;
                case "n": return -9;
                case "μ": return -6;
                case "u": return -6;
                case "m": return -3;
                case "": return 0;
                case " ": return 0;
                case "k": return 3;
                case "M": return 6;
                case "G": return 9;
                case "T": return 12;
                case "P": return 15;
                case "E": return 18;
                case "Z": return 21;
                case "Y": return 24;
                default:
                    throw new Exception("SI词头不正确。");
            }
        }

        /// <summary>
        /// 判断文本是否是SI词头
        /// </summary>
        /// <param name="word">判断的文本</param>
        /// <returns></returns>
        public static bool isPrefix(string word)
        {
            if (word == string.Empty) return true;
            if (word.Length > 1) return false;
            return ("yzafpnμum kMGTPEZY".IndexOf(word) > -1);
        }
    }

    /// <summary>
    /// 默认常数值
    /// </summary>
    public static class DefaultConstant
    {
        static DefaultConstant() { }

        /// <summary>
        /// 有效长度
        /// </summary>
        /// <param name="name">物理量类型</param>
        /// <returns></returns>
        public static int EffectiveLength(QuantityName name)
        {
            switch (name)
            {
                case QuantityName.Admittance: return 6;
                case QuantityName.ApparentPower: return 5;
                case QuantityName.Capacitance: return 6;
                case QuantityName.CentigradeTemperature: return 3;
                case QuantityName.Charge: return 3;
                case QuantityName.Current: return 6;
                case QuantityName.Frequency: return 6;
                case QuantityName.Inductor: return 6;
                case QuantityName.Length: return 3;
                case QuantityName.Power: return 5;
                case QuantityName.ReactivePower: return 5;
                case QuantityName.Resistance: return 6;
                case QuantityName.Temperature: return 3;
                case QuantityName.Time: return 6;
                case QuantityName.Voltage: return 6;
                default: return 4;
            }
        }

        /// <summary>
        /// 底噪电平
        /// </summary>
        /// <param name="name">物理量类型</param>
        /// <returns></returns>
        public static int NoiseLevel(QuantityName name)
        {
            switch (name)
            {
                case QuantityName.Admittance: return -1;
                case QuantityName.ApparentPower: return -4;
                case QuantityName.Capacitance: return -15;
                case QuantityName.CentigradeTemperature: return -1;
                case QuantityName.Charge: return -15;
                case QuantityName.Current: return -8;
                case QuantityName.Frequency: return -4;
                case QuantityName.Inductor: return -8;
                case QuantityName.Length: return 0;
                case QuantityName.Power: return -4;
                case QuantityName.ReactivePower: return -4;
                case QuantityName.Resistance: return 1;
                case QuantityName.Temperature: return -1;
                case QuantityName.Time: return -2;
                case QuantityName.Voltage: return -1;
                default: return -8;
            }
        }
    }

    /// <summary>
    /// 数值转换器
    /// </summary>
    public static class NumericsConverter
    {
        /// <summary>
        /// 将数值转换自动转换为SI词头的有效位数表示方式
        /// </summary>
        /// <param name="value">需要转换的数值</param>
        /// <param name="effectiveLength">有效位数长度（必须是正整数）</param>
        /// <param name="noiseLevel">物理量底噪电平</param>
        /// <param name="prefix">数值预设词头</param>
        /// <param name="quantity">数值单位</param>
        /// <param name="percentage">百分号显示</param>
        /// <param name="positiveSign">正值显示+号</param>
        /// <returns>带有SI词头的有效位数表示方式文本</returns>
        public static string Value2Text(double value, int effectiveLength, int noiseLevel, string prefix, string quantity, bool percentage = false, bool positiveSign = false, bool usePrefix = true)
        {
            string rtn = string.Empty;
            int el = effectiveLength;
            if (effectiveLength < 1) { throw new Exception("有效位数需要为正整数。"); }
            if (double.IsNaN(value)) return "NaN";
            if (double.IsInfinity(value)) return "Infinity";
            if (Math.Abs(value) < 1e-24)
            {
                if (percentage)
                    if (noiseLevel < -2)
                        return value.ToString("F" + (-noiseLevel - 2).ToString()) + "% " + quantity;
                if (usePrefix)
                {
                    prefix = PrefixsConverter.dec2prefixString(PrefixsConverter.prefixString2dec(prefix) + noiseLevel + el - 1);
                    return ((0.0).ToString("F" + (PrefixsConverter.prefixString2dec(prefix) - noiseLevel).ToString()) + " " + prefix + quantity);
                }
                return value.ToString("F" + (-noiseLevel).ToString()) + quantity;
            }
            if (value < 0) { value = -value; rtn = "-"; }
            else if (positiveSign) rtn += "+";
            value *= Math.Pow(10, PrefixsConverter.prefixString2dec(prefix));
            if (value > 1.0000000000000000000001)
            {
                percentage = false;
            }
            if (percentage)
            {
                value *= 100;
                noiseLevel += 2;
            }
            int decCnt = (int)Math.Floor(Math.Log10(value));
            decCnt = (int)Math.Floor(Math.Log10(value) + decCnt / Math.Pow(10, effectiveLength - 1));
            if (usePrefix)
                prefix = PrefixsConverter.dec2prefixString(Math.Max(PrefixsConverter.prefixString2dec(PrefixsConverter.dec2prefixString(decCnt)), noiseLevel + effectiveLength - 1));
            else
                prefix = "";
            if ((decCnt - effectiveLength + 1) < noiseLevel)
                effectiveLength = decCnt - noiseLevel + 1;
            if (!percentage)
            {
                if (usePrefix)
                    if ((decCnt - PrefixsConverter.prefixString2dec(prefix)) >= effectiveLength)
                        prefix = PrefixsConverter.dec2prefixString(PrefixsConverter.prefixString2dec(prefix) + 3);
                value /= Math.Pow(10, PrefixsConverter.prefixString2dec(prefix));
                decCnt = (int)Math.Floor(Math.Log10(value + value * 0.5 * Math.Pow(10, -el)));
                value = Math.Round(value, effectiveLength - Math.Min(0, decCnt + 1), MidpointRounding.AwayFromZero);
                if (value < 1e-24)
                {
                    if (usePrefix)
                    {
                        prefix = PrefixsConverter.dec2prefixString(noiseLevel + el - 1);
                        return ((0.0).ToString("F" + (PrefixsConverter.prefixString2dec(prefix) - noiseLevel).ToString()) + " " + prefix + quantity);
                    }
                    return value.ToString("F" + (-noiseLevel).ToString()) + quantity;
                }
                string format;
                if (value >= 1)
                    format = "F" + Math.Max((effectiveLength - decCnt - 1), 0).ToString();
                else
                    format = "F" + Math.Max((effectiveLength - decCnt - 1), 0).ToString();
                rtn += value.ToString(format);
                if ((prefix.Length == 0) && (quantity.Length == 0)) return rtn;
                return (rtn + " " + prefix + quantity);
            }
            else
            {
                string format = "F" + Math.Max(effectiveLength - decCnt - 1, 0).ToString();
                rtn += value.ToString(format);
                return (rtn + "% " + quantity);
            }
        }

        /// <summary>
        /// 将数值转换自动转换为SI词头的有效位数表示方式
        /// </summary>
        /// <param name="value">需要转换的数值</param>
        /// <param name="effectiveLength">有效位数长度（必须是正整数）</param>
        /// <param name="noiseLevel">物理量底噪电平</param>
        /// <param name="prefix">数值预设词头</param>
        /// <param name="quantity">物理量类型</param>
        /// <param name="percentage">百分号显示</param>
        /// <param name="positiveSign">正值显示+号</param>
        /// <returns>带有SI词头的有效位数表示方式文本</returns>
        public static string Value2Text(double value, int effectiveLength, int noiseLevel, string prefix, QuantityName quantity, bool percentage = false, bool positiveSign = false, bool usePrefix = true)
        {
            switch (quantity)
            {
                case QuantityName.Admittance:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "S", percentage, positiveSign, usePrefix);
                case QuantityName.ApparentPower:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "VA", percentage, positiveSign, usePrefix);
                case QuantityName.Capacitance:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "F", percentage, positiveSign, usePrefix);
                case QuantityName.CentigradeTemperature:
                    return Value2Text(value + 272.15, effectiveLength, noiseLevel, prefix, "°", percentage, positiveSign, usePrefix);
                case QuantityName.Charge:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "C", percentage, positiveSign, usePrefix);
                case QuantityName.Current:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "A", percentage, positiveSign, usePrefix);
                case QuantityName.Frequency:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "Hz", percentage, positiveSign, usePrefix);
                case QuantityName.Inductor:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "H", percentage, positiveSign, usePrefix);
                case QuantityName.Length:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "m", percentage, positiveSign, usePrefix);
                case QuantityName.Power:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "W", percentage, positiveSign, usePrefix);
                case QuantityName.ReactivePower:

                case QuantityName.Resistance:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "Ω", percentage, positiveSign, usePrefix);
                case QuantityName.Temperature:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "K", percentage, positiveSign, usePrefix);
                case QuantityName.Time:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "s", percentage, positiveSign, usePrefix);
                case QuantityName.Voltage:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "V", percentage, positiveSign, usePrefix);
                default:
                    return Value2Text(value, effectiveLength, noiseLevel, prefix, "", percentage, positiveSign, usePrefix);
            }
        }

        /// <summary>
        /// 将数值转换自动转换为SI词头的有效位数表示方式
        /// </summary>
        /// <param name="value">需要转换的物理量</param>
        /// <param name="effectiveLength">有效位数长度（必须是正整数）</param>
        /// <param name="noiseLevel">物理量底噪电平</param>
        /// <param name="percentage">百分号显示</param>
        /// <param name="positiveSign">正值显示+号</param>
        /// <returns>带有SI词头的有效位数表示方式文本</returns>
        public static string Value2Text(PhysicalVariable value, int effectiveLength, int noiseLevel, bool percentage = false, bool positiveSign = false, bool usePrefix = true)
        {
            if (value == null) return string.Empty;
            if (value.value != null)
                return Value2Text((double)value.value, effectiveLength, noiseLevel, string.Empty, value.PhysicalVariableType, percentage, positiveSign, usePrefix);
            else
                return value.OriginText;
        }

        /// <summary>
        /// 将数值转换自动转换为SI词头的有效位数表示方式
        /// </summary>
        /// <param name="value">需要转换的物理量</param>
        /// <param name="effectiveLength">有效位数长度（必须是正整数）</param>
        /// <param name="percentage">百分号显示</param>
        /// <param name="positiveSign">正值显示+号</param>
        /// <returns>带有SI词头的有效位数表示方式文本</returns>
        public static string Value2Text(PhysicalVariable value, int effectiveLength, bool percentage = false, bool positiveSign = false, bool usePrefix = true)
        {
            return Value2Text(value, effectiveLength, DefaultConstant.NoiseLevel(value.PhysicalVariableType), percentage, positiveSign, usePrefix);
        }

        /// <summary>
        /// 将数值转换自动转换为SI词头的有效位数表示方式
        /// </summary>
        /// <param name="value">需要转换的物理量</param>
        /// <returns>带有SI词头的有效位数表示方式文本</returns>
        /// <param name="percentage">百分号显示</param>
        /// <param name="positiveSign">正值显示+号</param>
        /// <returns>带有SI词头的有效位数表示方式文本</returns>
        public static string Value2Text(PhysicalVariable value, bool percentage = false, bool positiveSign = false, bool usePrefix = true)
        {
            if (value.EffectiveLength > 0)
            {
                return Value2Text(value, (int)value.EffectiveLength, DefaultConstant.NoiseLevel(value.PhysicalVariableType), percentage, positiveSign, usePrefix);
            }
            else
            {
                return Value2Text(value, DefaultConstant.EffectiveLength(value.PhysicalVariableType), DefaultConstant.NoiseLevel(value.PhysicalVariableType), percentage, positiveSign, usePrefix);
            }
        }

        /// <summary>
        /// 将文本转化为物理量
        /// </summary>
        /// <param name="text">代表物理量的文本</param>
        /// <param name="successed">是否转换成功</param>
        /// <returns>转化后的物理量（当successed = true时有效）</returns>
        public static PhysicalVariable Text2Value(string text, out bool successed)
        {
            PhysicalVariable rtn = new PhysicalVariable();
            string num = string.Empty;
            string suffix = string.Empty;
            int i = 0;
            foreach (char c in text)
            {
                if (c != ' ')
                    if (c != '?')
                        if (c != '\0')
                            if (c != ';')
                                num += c;
            }
            rtn.OriginText = num;
            if (text.Length < 1)
            {
                successed = false;
                return rtn;
            }
            if (num.StartsWith("+")) num = num.Remove(0, 1);
            while ((num.Length > 0) && (!Microsoft.VisualBasic.Information.IsNumeric(num)))
            {
                suffix = num[num.Length - 1] + suffix;
                num = num.Remove(num.Length - 1, 1);
            }
            if (num == string.Empty)
            {
                successed = false;
                rtn.PhysicalVariableType = QuantityName.None;
                return rtn;
            }

            suffix = suffix.Trim();
            Symbol symbol = QuantitiesConverter.String2Symbol(suffix);
            switch (symbol)
            {
                case Symbol.None:
                    break;
                case Symbol.VAR:
                    suffix = suffix.Remove(suffix.Length - 3);
                    break;
                case Symbol.Hz:
                case Symbol.VA:
                    suffix = suffix.Remove(suffix.Length - 2);
                    break;
                default:
                    suffix = suffix.Remove(suffix.Length - 1);
                    break;
            }
            bool percentage = suffix.StartsWith("%");
            if (percentage)
            {
                suffix = suffix.Remove(0, 1);
            }
            if (PrefixsConverter.isPrefix(suffix) == false)
            {
                successed = false;
                rtn.PhysicalVariableType = QuantityName.None;
                return rtn;
            }
            rtn.value = double.Parse(num);
            rtn.value *= Math.Pow(10, PrefixsConverter.prefixString2dec(suffix));
            if (percentage)
            {
                rtn.value /= 100.0;
            }
            rtn.EffectiveLength = 0;
            i = 0;
            bool notZero = false;

            while (i < num.Length)
            {
                if (num[i] == '.') i++;
                if (i >= num.Length)
                {
                    successed = false;
                    rtn.PhysicalVariableType = QuantityName.None;
                    return rtn;
                }
                if (num[i] == '0')
                {
                    if (notZero)
                    {
                        rtn.EffectiveLength++;
                        i++;
                    }
                    else
                    {
                        i++;
                    }
                }
                else if ((num[i] >= '1') && (num[i] <= '9'))
                {
                    notZero = true;
                    rtn.EffectiveLength++;
                    i++;
                }
                else
                {
                    i = num.Length;
                }
            }
            rtn.PhysicalVariableType = (QuantityName)symbol;
            successed = true;
            return rtn;
        }

        /// <summary>
        /// 将文本转化为物理量
        /// </summary>
        /// <param name="text">代表物理量的文本</param>
        /// <returns>转化后的物理量</returns>
        public static PhysicalVariable Text2Value(string text)
        {
            bool successed;
            PhysicalVariable rtn = Text2Value(text, out successed);
            return rtn;
        }

        /// <summary>
        /// 将文本集转换为物理量数组
        /// </summary>
        /// <param name="text">文本集</param>
        /// <returns>物理量数组</returns>
        public static List<PhysicalVariable> TextCollection2ValueList(string[] text)
        {
            List<PhysicalVariable> pvs = new List<PhysicalVariable>();
            foreach (var str in text)
            {
                pvs.Add(Text2Value(str));
            }
            return pvs;
        }
    }

}
